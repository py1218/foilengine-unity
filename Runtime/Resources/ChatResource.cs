using System;
using System.Collections;
using System.Threading.Tasks;
using FoilEngine.Internal;
using FoilEngine.Models;

namespace FoilEngine
{
    /// <summary>Initialize sessions, send messages, and manage chat state.</summary>
    public class ChatResource
    {
        private readonly FoilHttpClient _http;
        internal ChatResource(FoilHttpClient http) => _http = http;

        private static void Require(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{name} must be a non-empty string", name);
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
        public Task<ChatResponse> SendMessageAsync(
            string personaId,
            string message,
            string userSessionId,
            string machineId = null)
        {
            Require(personaId, nameof(personaId));
            Require(message, nameof(message));
            Require(userSessionId, nameof(userSessionId));
            var qs = machineId != null ? $"machine_id={Uri.EscapeDataString(machineId)}" : null;
            return _http.PostAsync<ChatResponse>(
                $"/api/v1/sdk/chat/{personaId}/message",
                new { message, user_session_id = userSessionId },
                qs
            );
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
            return _http.Post(
                $"/api/v1/sdk/chat/{personaId}/message",
                new { message, user_session_id = userSessionId },
                qs, onSuccess, onError
            );
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
    }
}
