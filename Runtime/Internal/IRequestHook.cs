namespace FoilEngine.Internal
{
    /// <summary>
    /// Hook into the request/response lifecycle for logging, analytics, or telemetry.
    ///
    /// Usage:
    ///   public class MyHook : IRequestHook
    ///   {
    ///       public void BeforeRequest(string method, string url) =>
    ///           Debug.Log($"-> {method} {url}");
    ///       public void AfterResponse(string method, string url, int status, float elapsedMs) =>
    ///           Debug.Log($"<- {status} in {elapsedMs:F0}ms");
    ///       public void OnError(string method, string url, System.Exception error) =>
    ///           Debug.LogError($"!! {error.Message}");
    ///   }
    ///
    ///   var client = new FoilEngineClient("pk_live_...", hooks: new IRequestHook[] { new MyHook() });
    /// </summary>
    public interface IRequestHook
    {
        /// <summary>Called before each request.</summary>
        void BeforeRequest(string method, string url);

        /// <summary>Called after a successful response (status &lt; 400).</summary>
        void AfterResponse(string method, string url, int statusCode, float elapsedMs);

        /// <summary>Called when a request fails.</summary>
        void OnError(string method, string url, System.Exception error);
    }
}
