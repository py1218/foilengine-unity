using System.Threading.Tasks;
using FoilEngine.Internal;

namespace FoilEngine
{
    /// <summary>
    /// Foil Engine API client for Unity.
    ///
    /// Usage (async):
    ///   var client = new FoilEngineClient("pk_live_...");
    ///   var personas = await client.Personas.ListAsync();
    ///
    /// Usage (coroutine):
    ///   StartCoroutine(client.Chat.SendMessage(personaId, "Hello!", sessionId,
    ///       onSuccess: r => Debug.Log(r.Message),
    ///       onError: e => Debug.LogError(e.Message)));
    /// </summary>
    public class FoilEngineClient
    {
        private readonly FoilHttpClient _http;
        public PersonasResource Personas { get; }
        public MachinesResource Machines { get; }
        public ChatResource Chat { get; }

        public FoilEngineClient(
            string apiKey,
            string baseUrl = "https://api.foilengine.io",
            int timeout = 30,
            int maxRetries = 3,
            string llmApiKey = null,
            string llmModel = null,
            string llmEvalModel = null,
            string llmResponseModel = null,
            string llmSummarizationModel = null,
            string llmEvalApiKey = null,
            string llmResponseApiKey = null,
            string llmSummarizationApiKey = null,
            bool debug = false,
            float cacheTtl = 60)
        {
            _http = new FoilHttpClient(
                apiKey, baseUrl, timeout, maxRetries,
                llmApiKey, llmModel, llmEvalModel, llmResponseModel, llmSummarizationModel,
                llmEvalApiKey, llmResponseApiKey, llmSummarizationApiKey,
                debug);
            Personas = new PersonasResource(_http, cacheTtl);
            Machines = new MachinesResource(_http);
            Chat = new ChatResource(_http);
        }

        /// <summary>Validate that the configured LLM API key works.</summary>
        public Task<Models.ValidateLlmKeyResult> ValidateLlmKeyAsync(string model = null)
        {
            var body = model != null ? new { model } : null;
            return _http.PostAsync<Models.ValidateLlmKeyResult>("/api/v1/sdk/validate-llm-key", body);
        }
    }
}
