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
        public PersonasResource Personas { get; }
        public MachinesResource Machines { get; }
        public ChatResource Chat { get; }

        public FoilEngineClient(
            string apiKey,
            string baseUrl = "https://api.foilengine.com",
            int timeout = 30,
            int maxRetries = 3)
        {
            var http = new FoilHttpClient(apiKey, baseUrl, timeout, maxRetries);
            Personas = new PersonasResource(http);
            Machines = new MachinesResource(http);
            Chat = new ChatResource(http);
        }
    }
}
