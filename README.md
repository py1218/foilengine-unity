# Foil Engine Unity SDK

Unity SDK for the [Foil Engine](https://foilengine.io) NPC API. Add AI-powered NPC conversations to your Unity game.

## Installation

Install via Unity Package Manager using a git URL:

**Window > Package Manager > + > Add package from git URL:**

```
https://github.com/py1218/foilengine-unity.git#v0.4.0
```

Requires Unity 2021.3 or later.

## Quick Start (Async/Await)

```csharp
using FoilEngine;
using FoilEngine.Models;

var client = new FoilEngineClient(
    "pk_live_...",
    llmApiKey: "sk-...",   // your LLM provider API key
    llmModel: "gpt-4o"    // any LiteLLM-supported model
);

Persona[] personas = await client.Personas.ListAsync();

SessionInit session = await client.Chat.InitSessionAsync(
    personaId: personas[0].Id,
    userSessionId: "player-001",
    playerName: "Alex",
    playerGender: "non-binary"
);
Debug.Log(session.Message);

ChatResponse response = await client.Chat.SendMessageAsync(
    personaId: personas[0].Id,
    message: "What do you recommend?",
    userSessionId: "player-001"
);
Debug.Log(response.Message);
```

## Quick Start (Coroutines)

```csharp
StartCoroutine(client.Chat.SendMessage(
    personaId: personaId,
    message: "Hello!",
    userSessionId: "player-001",
    onSuccess: response => Debug.Log(response.Message),
    onError: err => Debug.LogError(err.Message)
));
```

## Streaming Responses

Stream NPC responses token-by-token for a typing effect. Metadata (state, score, decision) arrives via `onMetadata` before any text.

```csharp
await client.Chat.SendMessageStreamAsync(
    personaId: personaId,
    message: "Tell me about your potions.",
    userSessionId: "player-001",
    onMetadata: meta => {
        // All game state available before text starts
        Debug.Log($"State: {meta.CurrentState}, Score: {meta.Score}");
    },
    onTextDelta: text => {
        // Progressive text chunks — append to UI
        dialogueText.text += text;
    },
    onDone: response => {
        // Full ChatResponse for final reconciliation
        Debug.Log($"Final: {response.Message}");
    },
    onError: error => Debug.LogError(error)
);
```

A coroutine version `SendMessageStream()` is also available with the same callback signature.

## Events

Register callbacks to react to game-relevant events like state transitions, score changes, or machine completion. Events fire automatically after each `SendMessageAsync()` call.

```csharp
client.OnStateChange += e =>
    Debug.Log($"State: {e.FromState} -> {e.ToState}");

client.OnScoreChange += e =>
    Debug.Log($"Score: {e.OldScore} -> {e.NewScore} (delta: {e.Delta})");

client.OnMachineCompleted += e =>
    Debug.Log($"Done! Outcome: {e.Outcome}");

client.OnMachinesUnlocked += e =>
    Debug.Log($"Unlocked {e.Machines.Length} machine(s)");

client.OnSessionEnded += e =>
    Debug.Log($"Session ended: {e.Outcome}");
```

| Event | Data | When |
|-------|------|------|
| `OnStateChange` | FromState, ToState, Decision | State machine transitions |
| `OnScoreChange` | OldScore, NewScore, Delta | Score changes |
| `OnMachineCompleted` | Outcome, FinalScore, SessionId | Terminal state reached |
| `OnMachinesUnlocked` | Machines[] | New machines available |
| `OnSessionEnded` | Outcome, FinalScore, SessionId | Session gets an outcome |

## Request Hooks

Add custom logging, analytics, or telemetry with request lifecycle hooks.

```csharp
public class MyHook : IRequestHook
{
    public void BeforeRequest(string method, string url)
        => Debug.Log($"-> {method} {url}");

    public void AfterResponse(string method, string url, int status, float elapsedMs)
        => Debug.Log($"<- {status} in {elapsedMs:F0}ms");

    public void OnError(string method, string url, System.Exception error)
        => Debug.LogError($"!! {error.Message}");
}

var client = new FoilEngineClient(
    "pk_live_...",
    llmApiKey: "sk-...",
    hooks: new IRequestHook[] { new MyHook() }
);
```

## Documentation

Full documentation at [foilengine.io/docs/sdk/unity](https://foilengine.io/docs/sdk/unity)

## License

MIT
