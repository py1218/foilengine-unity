using System;
using Newtonsoft.Json;

namespace FoilEngine.Models
{
    [Serializable]
    public class UnlockedMachine
    {
        [JsonProperty("machine_id")] public string MachineId;
        [JsonProperty("machine_key")] public string MachineKey;
        [JsonProperty("name")] public string Name;
        [JsonProperty("is_auto_advance")] public bool IsAutoAdvance;
    }

    [Serializable]
    public class ChatResponse
    {
        [JsonProperty("message")] public string Message;
        [JsonProperty("current_state")] public string CurrentState;
        [JsonProperty("score")] public int Score;
        [JsonProperty("outcome")] public string Outcome;
        [JsonProperty("decision")] public string Decision;
        [JsonProperty("follow_up")] public bool FollowUp;
        [JsonProperty("session_id")] public string SessionId;
        [JsonProperty("scoring_mode")] public string ScoringMode;
        [JsonProperty("redirect_count")] public int? RedirectCount;
        [JsonProperty("machine_completed")] public bool MachineCompleted;
        [JsonProperty("unlocked_machines")] public UnlockedMachine[] UnlockedMachines;
    }

    [Serializable]
    public class ChatMessage
    {
        [JsonProperty("id")] public string Id;
        [JsonProperty("role")] public string Role;
        [JsonProperty("content")] public string Content;
        [JsonProperty("state_key_at_time")] public string StateKeyAtTime;
        [JsonProperty("score_at_time")] public int? ScoreAtTime;
        [JsonProperty("decision")] public string Decision;
        [JsonProperty("follow_up_triggered")] public bool FollowUpTriggered;
        [JsonProperty("created_at")] public string CreatedAt;
    }

    [Serializable]
    public class ChatHistory
    {
        [JsonProperty("session_id")] public string SessionId;
        [JsonProperty("persona_id")] public string PersonaId;
        [JsonProperty("messages")] public ChatMessage[] Messages;
        [JsonProperty("current_state")] public string CurrentState;
        [JsonProperty("score")] public int Score;
        [JsonProperty("outcome")] public string Outcome;
        [JsonProperty("player_name")] public string PlayerName;
        [JsonProperty("player_gender")] public string PlayerGender;
        [JsonProperty("started_at")] public string StartedAt;
        [JsonProperty("last_message_at")] public string LastMessageAt;
    }
}
