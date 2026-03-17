using NUnit.Framework;
using FoilEngine;
using FoilEngine.Events;
using FoilEngine.Internal;

namespace FoilEngine.Tests
{
    public class FoilEngineClientTests
    {
        [Test]
        public void Constructor_CreatesResources()
        {
            var client = new FoilEngineClient("pk_test_123");
            Assert.IsNotNull(client.Personas);
            Assert.IsNotNull(client.Machines);
            Assert.IsNotNull(client.Chat);
        }

        [Test]
        public void Constructor_WithAllParams_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new FoilEngineClient(
                apiKey: "pk_test_123",
                baseUrl: "https://custom.api.io",
                timeout: 60,
                maxRetries: 5,
                llmApiKey: "sk-test",
                llmModel: "gpt-4o",
                llmEvalModel: "gpt-4o-mini",
                llmResponseModel: "gpt-4o",
                llmSummarizationModel: "gpt-4o-mini",
                llmEvalApiKey: "sk-eval",
                llmResponseApiKey: "sk-response",
                llmSummarizationApiKey: "sk-summarize",
                debug: true,
                cacheTtl: 120,
                hooks: new IRequestHook[] { new TestHook() }
            ));
        }

        [Test]
        public void Constructor_WithNullHooks_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new FoilEngineClient(
                apiKey: "pk_test_123",
                hooks: null
            ));
        }

        [Test]
        public void Events_CanSubscribeAndUnsubscribe()
        {
            var client = new FoilEngineClient("pk_test_123");
            var fired = false;

            void Handler(StateChangeEvent e) => fired = true;

            client.OnStateChange += Handler;
            client.OnStateChange -= Handler;

            Assert.IsFalse(fired);
        }

        [Test]
        public void Events_AllEventTypes_CanSubscribe()
        {
            var client = new FoilEngineClient("pk_test_123");

            Assert.DoesNotThrow(() =>
            {
                client.OnStateChange += e => { };
                client.OnScoreChange += e => { };
                client.OnMachineCompleted += e => { };
                client.OnMachinesUnlocked += e => { };
                client.OnSessionEnded += e => { };
            });
        }

        private class TestHook : IRequestHook
        {
            public void BeforeRequest(string method, string url) { }
            public void AfterResponse(string method, string url, int statusCode, float elapsedMs) { }
            public void OnError(string method, string url, System.Exception error) { }
        }
    }
}
