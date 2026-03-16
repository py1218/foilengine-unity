using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace FoilEngine.Internal
{
    /// <summary>
    /// Error response shape from the API: {"error": {"message": "..."}}
    /// </summary>
    [Serializable]
    internal class ApiErrorResponse
    {
        [JsonProperty("error")] public ApiErrorDetail Error;
    }

    [Serializable]
    internal class ApiErrorDetail
    {
        [JsonProperty("message")] public string Message;
    }

    /// <summary>
    /// HTTP client using UnityWebRequest for platform compatibility.
    /// Supports both coroutine callbacks and async/await.
    /// </summary>
    internal class FoilHttpClient
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly int _timeout;
        private readonly int _maxRetries;
        private readonly string _llmApiKey;
        private readonly string _llmModel;
        private readonly string _llmEvalModel;
        private readonly string _llmResponseModel;
        private readonly string _llmSummarizationModel;

        public FoilHttpClient(
            string apiKey, string baseUrl, int timeout = 30, int maxRetries = 3,
            string llmApiKey = null, string llmModel = null,
            string llmEvalModel = null, string llmResponseModel = null,
            string llmSummarizationModel = null)
        {
            _apiKey = apiKey;
            _baseUrl = baseUrl.TrimEnd('/');
            _timeout = timeout;
            _maxRetries = maxRetries;
            _llmApiKey = llmApiKey;
            _llmModel = llmModel;
            _llmEvalModel = llmEvalModel;
            _llmResponseModel = llmResponseModel;
            _llmSummarizationModel = llmSummarizationModel;
        }

        // ---- Async/Await (Unity 2023+) ----

        public async Task<T> GetAsync<T>(string path, string queryString = null)
        {
            var url = BuildUrl(path, queryString);
            return await RequestAsync<T>("GET", url, null);
        }

        public async Task<T> PostAsync<T>(string path, object body, string queryString = null)
        {
            var url = BuildUrl(path, queryString);
            var json = body != null ? JsonConvert.SerializeObject(body) : null;
            return await RequestAsync<T>("POST", url, json);
        }

        private async Task<T> RequestAsync<T>(string method, string url, string bodyJson)
        {
            Exception lastError = null;

            for (int attempt = 0; attempt <= _maxRetries; attempt++)
            {
                using var request = CreateRequest(method, url, bodyJson);
                request.timeout = _timeout;

                var operation = request.SendWebRequest();

                // Await the async operation
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                }

                var statusCode = (int)request.responseCode;

                if (IsRetryable(statusCode) && attempt < _maxRetries)
                {
                    var wait = GetRetryDelay(request, attempt);
                    await Task.Delay((int)(wait * 1000));
                    continue;
                }

                ThrowForStatus(statusCode, request.downloadHandler?.text);
            }

            throw new FoilEngineException("Max retries exceeded");
        }

        // ---- Coroutine-based (all Unity versions) ----

        public IEnumerator Get<T>(string path, string queryString, Action<T> onSuccess, Action<FoilEngineException> onError)
        {
            var url = BuildUrl(path, queryString);
            yield return DoRequest("GET", url, null, onSuccess, onError);
        }

        public IEnumerator Post<T>(string path, object body, string queryString, Action<T> onSuccess, Action<FoilEngineException> onError)
        {
            var url = BuildUrl(path, queryString);
            var json = body != null ? JsonConvert.SerializeObject(body) : null;
            yield return DoRequest("POST", url, json, onSuccess, onError);
        }

        private IEnumerator DoRequest<T>(string method, string url, string bodyJson, Action<T> onSuccess, Action<FoilEngineException> onError)
        {
            for (int attempt = 0; attempt <= _maxRetries; attempt++)
            {
                using var request = CreateRequest(method, url, bodyJson);
                request.timeout = _timeout;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var result = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                    onSuccess?.Invoke(result);
                    yield break;
                }

                var statusCode = (int)request.responseCode;

                if (IsRetryable(statusCode) && attempt < _maxRetries)
                {
                    var wait = GetRetryDelay(request, attempt);
                    yield return new WaitForSeconds(wait);
                    continue;
                }

                try
                {
                    ThrowForStatus(statusCode, request.downloadHandler?.text);
                }
                catch (FoilEngineException ex)
                {
                    onError?.Invoke(ex);
                    yield break;
                }
            }

            onError?.Invoke(new FoilEngineException("Max retries exceeded"));
        }

        // ---- Helpers ----

        private UnityWebRequest CreateRequest(string method, string url, string bodyJson)
        {
            UnityWebRequest request;

            if (method == "GET")
            {
                request = UnityWebRequest.Get(url);
            }
            else
            {
                request = new UnityWebRequest(url, method);
                if (bodyJson != null)
                {
                    request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyJson));
                }
                request.downloadHandler = new DownloadHandlerBuffer();
            }

            request.SetRequestHeader("X-API-Key", _apiKey);
            request.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(_llmApiKey))
                request.SetRequestHeader("X-LLM-API-Key", _llmApiKey);
            if (!string.IsNullOrEmpty(_llmModel))
                request.SetRequestHeader("X-LLM-Model", _llmModel);
            if (!string.IsNullOrEmpty(_llmEvalModel))
                request.SetRequestHeader("X-LLM-Eval-Model", _llmEvalModel);
            if (!string.IsNullOrEmpty(_llmResponseModel))
                request.SetRequestHeader("X-LLM-Response-Model", _llmResponseModel);
            if (!string.IsNullOrEmpty(_llmSummarizationModel))
                request.SetRequestHeader("X-LLM-Summarization-Model", _llmSummarizationModel);
            return request;
        }

        private string BuildUrl(string path, string queryString)
        {
            var url = $"{_baseUrl}{path}";
            if (!string.IsNullOrEmpty(queryString))
                url += $"?{queryString}";
            return url;
        }

        private static bool IsRetryable(int statusCode)
        {
            return statusCode == 429 || statusCode >= 500;
        }

        private static float GetRetryDelay(UnityWebRequest request, int attempt)
        {
            var retryAfter = request.GetResponseHeader("Retry-After");
            if (!string.IsNullOrEmpty(retryAfter) && float.TryParse(retryAfter, out var seconds))
                return seconds;
            return Mathf.Pow(2, attempt) * 0.5f;
        }

        private static void ThrowForStatus(int statusCode, string responseBody)
        {
            var message = "Unknown error";

            if (!string.IsNullOrEmpty(responseBody))
            {
                try
                {
                    var error = JsonConvert.DeserializeObject<ApiErrorResponse>(responseBody);
                    if (error?.Error?.Message != null)
                        message = error.Error.Message;
                }
                catch
                {
                    message = responseBody;
                }
            }

            switch (statusCode)
            {
                case 400: throw new BadRequestException(message);
                case 401: throw new AuthenticationException(message);
                case 403: throw new ForbiddenException(message);
                case 404: throw new NotFoundException(message);
                case 429: throw new RateLimitException(message);
                default:
                    if (statusCode >= 500) throw new ServerException(message);
                    throw new FoilEngineException(message, statusCode);
            }
        }
    }
}
