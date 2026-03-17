using System;
using System.Collections.Generic;
using FoilEngine.Models;

namespace FoilEngine.Events
{
    /// <summary>Data for state_change events.</summary>
    public struct StateChangeEvent
    {
        public string FromState;
        public string ToState;
        public string Decision;
    }

    /// <summary>Data for score_change events.</summary>
    public struct ScoreChangeEvent
    {
        public int OldScore;
        public int NewScore;
        public int Delta;
    }

    /// <summary>Data for machine_completed events.</summary>
    public struct MachineCompletedEvent
    {
        public string Outcome;
        public int FinalScore;
        public string SessionId;
    }

    /// <summary>Data for machines_unlocked events.</summary>
    public struct MachinesUnlockedEvent
    {
        public UnlockedMachine[] Machines;
    }

    /// <summary>Data for session_ended events.</summary>
    public struct SessionEndedEvent
    {
        public string Outcome;
        public int FinalScore;
        public string SessionId;
    }

    /// <summary>
    /// Event emitter for game-relevant chat events.
    ///
    /// Usage:
    ///   client.OnStateChange += (e) => Debug.Log($"State: {e.FromState} -> {e.ToState}");
    ///   client.OnScoreChange += (e) => Debug.Log($"Score: {e.OldScore} -> {e.NewScore}");
    ///   client.OnMachineCompleted += (e) => Debug.Log($"Done: {e.Outcome}");
    /// </summary>
    public class FoilEventEmitter
    {
        public event Action<StateChangeEvent> OnStateChange;
        public event Action<ScoreChangeEvent> OnScoreChange;
        public event Action<MachineCompletedEvent> OnMachineCompleted;
        public event Action<MachinesUnlockedEvent> OnMachinesUnlocked;
        public event Action<SessionEndedEvent> OnSessionEnded;

        // Tracks previous state per persona for diffing
        private readonly Dictionary<string, ChatState> _prevStates = new();

        private class ChatState
        {
            public string CurrentState;
            public int? Score;
            public string Outcome;
        }

        internal bool HasListeners =>
            OnStateChange != null || OnScoreChange != null ||
            OnMachineCompleted != null || OnMachinesUnlocked != null ||
            OnSessionEnded != null;

        /// <summary>Diff a ChatResponse against previous state and fire events.</summary>
        internal void EmitChatEvents(string personaId, ChatResponse response)
        {
            if (!_prevStates.TryGetValue(personaId, out var prev))
            {
                prev = new ChatState();
                _prevStates[personaId] = prev;
            }

            if (!HasListeners)
            {
                prev.CurrentState = response.CurrentState;
                prev.Score = response.Score;
                prev.Outcome = response.Outcome;
                return;
            }

            // State change
            if (prev.CurrentState != null && response.CurrentState != prev.CurrentState)
            {
                OnStateChange?.Invoke(new StateChangeEvent
                {
                    FromState = prev.CurrentState,
                    ToState = response.CurrentState,
                    Decision = response.Decision
                });
            }

            // Score change
            if (prev.Score.HasValue && response.Score != prev.Score.Value)
            {
                OnScoreChange?.Invoke(new ScoreChangeEvent
                {
                    OldScore = prev.Score.Value,
                    NewScore = response.Score,
                    Delta = response.Score - prev.Score.Value
                });
            }

            // Machine completed
            if (response.MachineCompleted)
            {
                OnMachineCompleted?.Invoke(new MachineCompletedEvent
                {
                    Outcome = response.Outcome,
                    FinalScore = response.Score,
                    SessionId = response.SessionId
                });
            }

            // Machines unlocked
            if (response.UnlockedMachines != null && response.UnlockedMachines.Length > 0)
            {
                OnMachinesUnlocked?.Invoke(new MachinesUnlockedEvent
                {
                    Machines = response.UnlockedMachines
                });
            }

            // Session ended
            if (response.Outcome != null && (prev.Outcome == null || prev.Outcome != response.Outcome))
            {
                OnSessionEnded?.Invoke(new SessionEndedEvent
                {
                    Outcome = response.Outcome,
                    FinalScore = response.Score,
                    SessionId = response.SessionId
                });
            }

            prev.CurrentState = response.CurrentState;
            prev.Score = response.Score;
            prev.Outcome = response.Outcome;
        }
    }
}
