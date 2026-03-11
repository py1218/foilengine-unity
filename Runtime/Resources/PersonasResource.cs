using System.Collections;
using System.Threading.Tasks;
using System;
using FoilEngine.Internal;
using FoilEngine.Models;

namespace FoilEngine
{
    /// <summary>List published personas owned by your API key.</summary>
    public class PersonasResource
    {
        private readonly FoilHttpClient _http;
        internal PersonasResource(FoilHttpClient http) => _http = http;

        /// <summary>List all published personas (async).</summary>
        public Task<Persona[]> ListAsync()
        {
            return _http.GetAsync<Persona[]>("/api/v1/sdk/personas");
        }

        /// <summary>List all published personas (coroutine).</summary>
        public IEnumerator List(Action<Persona[]> onSuccess, Action<FoilEngineException> onError = null)
        {
            return _http.Get("/api/v1/sdk/personas", null, onSuccess, onError);
        }
    }
}
