using System.Collections;
using System.Threading.Tasks;
using System;
using FoilEngine.Internal;
using FoilEngine.Models;

namespace FoilEngine
{
    /// <summary>List machines available to a player.</summary>
    public class MachinesResource
    {
        private readonly FoilHttpClient _http;
        internal MachinesResource(FoilHttpClient http) => _http = http;

        private static void Require(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{name} must be a non-empty string", name);
        }

        /// <summary>List all machines available to a player for a persona (async).</summary>
        public Task<MachineInfo[]> ListAsync(string personaId, string userSessionId)
        {
            Require(personaId, nameof(personaId));
            Require(userSessionId, nameof(userSessionId));
            return _http.GetAsync<MachineInfo[]>(
                $"/api/v1/sdk/chat/{personaId}/machines",
                $"user_session_id={Uri.EscapeDataString(userSessionId)}"
            );
        }

        /// <summary>List all machines available to a player for a persona (coroutine).</summary>
        public IEnumerator List(string personaId, string userSessionId, Action<MachineInfo[]> onSuccess, Action<FoilEngineException> onError = null)
        {
            Require(personaId, nameof(personaId));
            Require(userSessionId, nameof(userSessionId));
            return _http.Get(
                $"/api/v1/sdk/chat/{personaId}/machines",
                $"user_session_id={Uri.EscapeDataString(userSessionId)}",
                onSuccess, onError
            );
        }
    }
}
