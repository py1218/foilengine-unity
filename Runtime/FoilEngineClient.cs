using System;
using System.Threading.Tasks;
using FoilEngine.Events;
using FoilEngine.Internal;

namespace FoilEngine
{
    /// <summary>
    /// Foil Engine API client for Unity.
    ///
    /// Usage (async):
    ///   var client = new FoilEngineClient("pk_live_...");
    ///   client.OnStateChange += (e) => Debug.Log($"State: {e.FromState} -> {e.ToState}");
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
        private readonly FoilEventEmitter _events;

        public PersonasResource Personas { get; }
        public MachinesResource Machines { get; }
        public ChatResource Chat { get; }

        /// <summary>Fired when the state machine transitions to a new state.</summary>
        public event Action<StateChangeEvent> OnStateChange
        {
            add => _events.OnStateChange += value;
            remove => _events.OnStateChange -= value;
        }

        /// <summary>Fired when the session score changes.</summary>
        public event Action<ScoreChangeEvent> OnScoreChange
        {
            add => _events.OnScoreChange += value;
            remove => _events.OnScoreChange -= value;
        }

        /// <summary>Fired when a conversation reaches a terminal state.</summary>
        public event Action<MachineCompletedEvent> OnMachineCompleted
        {
            add => _events.OnMachineCompleted += value;
            remove => _events.OnMachineCompleted -= value;
        }

        /// <summary>Fired when new machines become available after completion.</summary>
        public event Action<MachinesUnlockedEvent> OnMachinesUnlocked
        {
            add => _events.OnMachinesUnlocked += value;
            remove => _events.OnMachinesUnlocked -= value;
        }

        /// <summary>Fired when a session receives an outcome (ACCEPT, REJECT, KICK_OUT).</summary>
        public event Action<SessionEndedEvent> OnSessionEnded
        {
            add => _events.OnSessionEnded += value;
            remove => _events.OnSessionEnded -= value;
        }

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
            float cacheTtl = 60,
            IRequestHook[] hooks = null)
        {
            _http = new FoilHttpClient(
                apiKey, baseUrl, timeout, maxRetries,
                llmApiKey, llmModel, llmEvalModel, llmResponseModel, llmSummarizationModel,
                llmEvalApiKey, llmResponseApiKey, llmSummarizationApiKey,
                debug, hooks);
            _events = new FoilEventEmitter();
            Personas = new PersonasResource(_http, cacheTtl);
            Machines = new MachinesResource(_http);
            Chat = new ChatResource(_http, _events);
        }

        /// <summary>Validate that the configured LLM API key works.</summary>
        public Task<Models.ValidateLlmKeyResult> ValidateLlmKeyAsync(string model = null)
        {
            var body = model != null ? new { model } : null;
            return _http.PostAsync<Models.ValidateLlmKeyResult>("/api/v1/sdk/validate-llm-key", body);
        }
    }
}
