using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FoilEngine.Events;
using FoilEngine.Internal;
using FoilEngine.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace FoilEngine
{
    /// <summary>Initialize sessions, send messages, and manage chat state.</summary>
    public class ChatResource
    {
        private readonly FoilHttpClient _http;
        private readonly FoilEventEmitter _events;

        internal ChatResource(FoilHttpClient http, FoilEventEmitter events = null)
        {
            _http = http;
            _events = events;
        }

        private static void Require(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{name} must be a non-empty string", name);
        }

        private void EmitEvents(string personaId, ChatResponse response)
        {
            _events?.EmitChatEvents(personaId, response);
        }

        // ---- Async methods ----

        /// <summary>Initialize a new chat session (async).</summary>
        public Task<SessionInit> InitSessionAsync(
            string personaId,
            string userSessionId,
            string playerName,
            string playerGender,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(userSessionId, nameof(userSessionId));
            Require(playerName, nameof(playerName));
            Require(playerGender, nameof(playerGender));
            var qs = machineId != null ? $"machine_id={Uri.EscapeDataString(machineId)}" : null;
            return _http.PostAsync<SessionInit>(
                $"/api/v1/sdk/chat/{personaId}/init-session",
                new { user_session_id = userSessionId, player_name = playerName, player_gender = playerGender },
                qs
            );
        }

        /// <summary>Send a player message (async).</summary>
        public async Task<ChatResponse> SendMessageAsync(
            string personaId,
            string message,
            string userSessionId,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(message, nameof(message));
            Require(userSessionId, nameof(userSessionId));
            var qs = machineId != null ? $"machine_id={Uri.EscapeDataString(machineId)}" : null;
            var response = await _http.PostAsync<ChatResponse>(
                $"/api/v1/sdk/chat/{personaId}/message",
                new { message, user_session_id = userSessionId },
                qs
            );
            EmitEvents(personaId, response);
            return response;
        }

        /// <summary>
        /// Send a player message and stream the NPC response via SSE (async).
        /// Calls onMetadata once with game state, onTextDelta for each text chunk,
        /// and onDone with the final ChatResponse.
        /// </summary>
        public async Task SendMessageStreamAsync(
            string personaId,
            string message,
            string userSessionId,
            Action<ChatStreamMetadata> onMetadata = null,
            Action<string> onTextDelta = null,
            Action<ChatResponse> onDone = null,
            Action<string> onError = null,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(message, nameof(message));
            Require(userSessionId, nameof(userSessionId));

            var path = $"/api/v1/sdk/chat/{personaId}/message/stream";
            if (machineId != null) path += $"?machine_id={Uri.EscapeDataString(machineId)}";

            var bodyJson = JsonConvert.SerializeObject(new { message, user_session_id = userSessionId });

            using var request = new UnityWebRequest(_http.BuildUrlPublic(path, null), "POST");
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyJson));
            request.downloadHandler = new DownloadHandlerBuffer();
            _http.SetHeaders(request);
            request.timeout = 120;

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.downloadHandler?.text ?? request.error);
                return;
            }

            ParseSSEResponse(request.downloadHandler.text, personaId, onMetadata, onTextDelta, onDone, onError);
        }

        /// <summary>Check if a player session exists (async).</summary>
        public Task<SessionStatus> GetSessionAsync(
            string personaId,
            string userSessionId,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(userSessionId, nameof(userSessionId));
            var qs = $"user_session_id={Uri.EscapeDataString(userSessionId)}";
            if (machineId != null) qs += $"&machine_id={Uri.EscapeDataString(machineId)}";
            return _http.GetAsync<SessionStatus>($"/api/v1/sdk/chat/{personaId}/session", qs);
        }

        /// <summary>Get full chat history (async).</summary>
        public Task<ChatHistory> GetHistoryAsync(
            string personaId,
            string userSessionId,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(userSessionId, nameof(userSessionId));
            var qs = $"user_session_id={Uri.EscapeDataString(userSessionId)}";
            if (machineId != null) qs += $"&machine_id={Uri.EscapeDataString(machineId)}";
            return _http.GetAsync<ChatHistory>($"/api/v1/sdk/chat/{personaId}/history", qs);
        }

        /// <summary>Reset/delete a chat session (async).</summary>
        public Task<ResetResult> ResetAsync(
            string personaId,
            string userSessionId,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(userSessionId, nameof(userSessionId));
            var qs = $"user_session_id={Uri.EscapeDataString(userSessionId)}";
            if (machineId != null) qs += $"&machine_id={Uri.EscapeDataString(machineId)}";
            return _http.PostAsync<ResetResult>($"/api/v1/sdk/chat/{personaId}/reset", null, qs);
        }

        // ---- Coroutine methods ----

        /// <summary>Initialize a new chat session (coroutine).</summary>
        public IEnumerator InitSession(
            string personaId,
            string userSessionId,
            string playerName,
            string playerGender,
            Action<SessionInit> onSuccess,
            Action<FoilEngineException> onError = null,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(userSessionId, nameof(userSessionId));
            Require(playerName, nameof(playerName));
            Require(playerGender, nameof(playerGender));
            var qs = machineId != null ? $"machine_id={Uri.EscapeDataString(machineId)}" : null;
            return _http.Post(
                $"/api/v1/sdk/chat/{personaId}/init-session",
                new { user_session_id = userSessionId, player_name = playerName, player_gender = playerGender },
                qs, onSuccess, onError
            );
        }

        /// <summary>Send a player message (coroutine).</summary>
        public IEnumerator SendMessage(
            string personaId,
            string message,
            string userSessionId,
            Action<ChatResponse> onSuccess,
            Action<FoilEngineException> onError = null,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(message, nameof(message));
            Require(userSessionId, nameof(userSessionId));
            var qs = machineId != null ? $"machine_id={Uri.EscapeDataString(machineId)}" : null;
            return _http.Post<ChatResponse>(
                $"/api/v1/sdk/chat/{personaId}/message",
                new { message, user_session_id = userSessionId },
                qs,
                response =>
                {
                    EmitEvents(personaId, response);
                    onSuccess?.Invoke(response);
                },
                onError
            );
        }

        /// <summary>
        /// Send a player message and stream the NPC response via SSE (coroutine).
        /// Note: Due to Unity coroutine limitations, the full response is buffered
        /// before parsing SSE events. For true progressive streaming, use SendMessageStreamAsync.
        /// </summary>
        public IEnumerator SendMessageStream(
            string personaId,
            string message,
            string userSessionId,
            Action<ChatStreamMetadata> onMetadata = null,
            Action<string> onTextDelta = null,
            Action<ChatResponse> onDone = null,
            Action<string> onError = null,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(message, nameof(message));
            Require(userSessionId, nameof(userSessionId));

            var path = $"/api/v1/sdk/chat/{personaId}/message/stream";
            if (machineId != null) path += $"?machine_id={Uri.EscapeDataString(machineId)}";

            var bodyJson = JsonConvert.SerializeObject(new { message, user_session_id = userSessionId });

            using var request = new UnityWebRequest(_http.BuildUrlPublic(path, null), "POST");
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyJson));
            request.downloadHandler = new DownloadHandlerBuffer();
            _http.SetHeaders(request);
            request.timeout = 120;

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.downloadHandler?.text ?? request.error);
                yield break;
            }

            ParseSSEResponse(request.downloadHandler.text, personaId, onMetadata, onTextDelta, onDone, onError);
        }

        /// <summary>Check if a player session exists (coroutine).</summary>
        public IEnumerator GetSession(
            string personaId,
            string userSessionId,
            Action<SessionStatus> onSuccess,
            Action<FoilEngineException> onError = null,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(userSessionId, nameof(userSessionId));
            var qs = $"user_session_id={Uri.EscapeDataString(userSessionId)}";
            if (machineId != null) qs += $"&machine_id={Uri.EscapeDataString(machineId)}";
            return _http.Get($"/api/v1/sdk/chat/{personaId}/session", qs, onSuccess, onError);
        }

        /// <summary>Get full chat history (coroutine).</summary>
        public IEnumerator GetHistory(
            string personaId,
            string userSessionId,
            Action<ChatHistory> onSuccess,
            Action<FoilEngineException> onError = null,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(userSessionId, nameof(userSessionId));
            var qs = $"user_session_id={Uri.EscapeDataString(userSessionId)}";
            if (machineId != null) qs += $"&machine_id={Uri.EscapeDataString(machineId)}";
            return _http.Get($"/api/v1/sdk/chat/{personaId}/history", qs, onSuccess, onError);
        }

        /// <summary>Reset/delete a chat session (coroutine).</summary>
        public IEnumerator Reset(
            string personaId,
            string userSessionId,
            Action<ResetResult> onSuccess,
            Action<FoilEngineException> onError = null,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(userSessionId, nameof(userSessionId));
            var qs = $"user_session_id={Uri.EscapeDataString(userSessionId)}";
            if (machineId != null) qs += $"&machine_id={Uri.EscapeDataString(machineId)}";
            return _http.Post($"/api/v1/sdk/chat/{personaId}/reset", null, qs, onSuccess, onError);
        }

        // ---- SSE parsing ----

        private void ParseSSEResponse(
            string responseText,
            string personaId,
            Action<ChatStreamMetadata> onMetadata,
            Action<string> onTextDelta,
            Action<ChatResponse> onDone,
            Action<string> onError)
        {
            var eventType = "";
            var lines = responseText.Split('\n');

            foreach (var line in lines)
            {
                if (line.StartsWith("event: "))
                {
                    eventType = line.Substring(7).Trim();
                }
                else if (line.StartsWith("data: "))
                {
                    var data = line.Substring(6);
                    switch (eventType)
                    {
                        case "metadata":
                            var meta = JsonConvert.DeserializeObject<ChatStreamMetadata>(data);
                            onMetadata?.Invoke(meta);
                            break;
                        case "text_delta":
                            var delta = JsonConvert.DeserializeObject<ChatStreamTextDelta>(data);
                            onTextDelta?.Invoke(delta?.Text ?? "");
                            break;
                        case "done":
                            var response = JsonConvert.DeserializeObject<ChatResponse>(data);
                            EmitEvents(personaId, response);
                            onDone?.Invoke(response);
                            break;
                        case "error":
                            onError?.Invoke(data);
                            break;
                    }
                    eventType = "";
                }
            }
        }
    }
}
