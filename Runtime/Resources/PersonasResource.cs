using System.Collections;
using System.Threading.Tasks;
using System;
using UnityEngine;
using FoilEngine.Internal;
using FoilEngine.Models;

namespace FoilEngine
{
    /// <summary>List published personas owned by your API key.</summary>
    public class PersonasResource
    {
        private readonly FoilHttpClient _http;
        private Persona[] _cache;
        private float _cacheTime;
        private readonly float _cacheTtl;

        internal PersonasResource(FoilHttpClient http, float cacheTtl = 60f)
        {
            _http = http;
            _cacheTtl = cacheTtl;
        }

        /// <summary>List all published personas (async).</summary>
        public async Task<Persona[]> ListAsync()
        {
            if (_cacheTtl > 0 && _cache != null && Time.realtimeSinceStartup - _cacheTime < _cacheTtl)
                return _cache;

            var result = await _http.GetAsync<Persona[]>("/api/v1/sdk/personas");
            _cache = result;
            _cacheTime = Time.realtimeSinceStartup;
            return result;
        }

        /// <summary>List all published personas (coroutine).</summary>
        public IEnumerator List(Action<Persona[]> onSuccess, Action<FoilEngineException> onError = null)
        {
            if (_cacheTtl > 0 && _cache != null && Time.realtimeSinceStartup - _cacheTime < _cacheTtl)
            {
                onSuccess?.Invoke(_cache);
                yield break;
            }

            yield return _http.Get<Persona[]>("/api/v1/sdk/personas", null, result =>
            {
                _cache = result;
                _cacheTime = Time.realtimeSinceStartup;
                onSuccess?.Invoke(result);
            }, onError);
        }
    }
}
