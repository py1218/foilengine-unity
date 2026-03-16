using NUnit.Framework;
using FoilEngine;

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
                cacheTtl: 120
            ));
        }
    }
}
