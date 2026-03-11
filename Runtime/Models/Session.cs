using System;
using Newtonsoft.Json;

namespace FoilEngine.Models
{
    [Serializable]
    public class SessionInit
    {
        [JsonProperty("session_id")] public string SessionId;
        [JsonProperty("user_session_id")] public string UserSessionId;
        [JsonProperty("persona_name")] public string PersonaName;
        [JsonProperty("persona_description")] public string PersonaDescription;
        [JsonProperty("player_name")] public string PlayerName;
        [JsonProperty("player_gender")] public string PlayerGender;
        [JsonProperty("message")] public string Message;
        [JsonProperty("machine_id")] public string MachineId;
    }

    [Serializable]
    public class SessionStatus
    {
        [JsonProperty("session_id")] public string SessionId;
        [JsonProperty("persona_id")] public string PersonaId;
        [JsonProperty("machine_id")] public string MachineId;
        [JsonProperty("current_state")] public string CurrentState;
        [JsonProperty("score")] public int Score;
        [JsonProperty("outcome")] public string Outcome;
        [JsonProperty("player_name")] public string PlayerName;
        [JsonProperty("player_gender")] public string PlayerGender;
        [JsonProperty("started_at")] public string StartedAt;
        [JsonProperty("last_message_at")] public string LastMessageAt;
    }

    [Serializable]
    public class ResetResult
    {
        [JsonProperty("session_id")] public string SessionId;
        [JsonProperty("message")] public string Message;
    }
}
