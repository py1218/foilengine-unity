using System;
using Newtonsoft.Json;

namespace FoilEngine.Models
{
    [Serializable]
    public class Persona
    {
        [JsonProperty("id")] public string Id;
        [JsonProperty("name")] public string Name;
        [JsonProperty("description")] public string Description;
        [JsonProperty("created_at")] public string CreatedAt;
        [JsonProperty("updated_at")] public string UpdatedAt;
    }

    [Serializable]
    public class MachineInfo
    {
        [JsonProperty("machine_id")] public string MachineId;
        [JsonProperty("machine_key")] public string MachineKey;
        [JsonProperty("name")] public string Name;
        [JsonProperty("description")] public string Description;
        [JsonProperty("has_session")] public bool HasSession;
        [JsonProperty("session_outcome")] public string SessionOutcome;
        [JsonProperty("is_linked")] public bool IsLinked;
    }
}
