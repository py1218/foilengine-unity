# Foil Engine Unity SDK

Unity SDK for the [Foil Engine](https://foilengine.com) NPC API. Add AI-powered NPC conversations to your Unity game.

## Installation

Install via Unity Package Manager using a git URL:

**Window > Package Manager > + > Add package from git URL:**

```
https://github.com/py1218/foilengine-unity.git#v0.1.0
```

Requires Unity 2021.3 or later.

## Quick Start (Async/Await)

```csharp
using FoilEngine;
using FoilEngine.Models;

var client = new FoilEngineClient("pk_live_...");

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

## Documentation

Full documentation at [foilengine.com/docs/sdk/unity](https://foilengine.com/docs/sdk/unity)

## License

MIT
