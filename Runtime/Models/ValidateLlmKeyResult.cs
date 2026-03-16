using Newtonsoft.Json;

namespace FoilEngine.Models
{
    public class ValidateLlmKeyResult
    {
        [JsonProperty("valid")] public bool Valid;
        [JsonProperty("model")] public string Model;
        [JsonProperty("error")] public string Error;
    }
}
