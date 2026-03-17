using System;
using NUnit.Framework;
using FoilEngine;

namespace FoilEngine.Tests
{
    public class ValidationTests
    {
        private FoilEngineClient _client;

        [SetUp]
        public void Setup()
        {
            _client = new FoilEngineClient("pk_test_123", llmApiKey: "sk-test");
        }

        // --- InitSessionAsync ---

        [Test]
        public void InitSession_EmptyPersonaId_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.InitSessionAsync("", "session1", "Alex", "male"));
        }

        [Test]
        public void InitSession_EmptySessionId_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.InitSessionAsync("persona1", "", "Alex", "male"));
        }

        [Test]
        public void InitSession_EmptyPlayerName_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.InitSessionAsync("persona1", "session1", "", "male"));
        }

        [Test]
        public void InitSession_EmptyPlayerGender_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.InitSessionAsync("persona1", "session1", "Alex", ""));
        }

        // --- SendMessageAsync ---

        [Test]
        public void SendMessage_EmptyPersonaId_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.SendMessageAsync("", "hello", "session1"));
        }

        [Test]
        public void SendMessage_EmptyMessage_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.SendMessageAsync("persona1", "", "session1"));
        }

        [Test]
        public void SendMessage_EmptySessionId_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.SendMessageAsync("persona1", "hello", ""));
        }

        // --- GetSessionAsync ---

        [Test]
        public void GetSession_EmptyPersonaId_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.GetSessionAsync("", "session1"));
        }

        [Test]
        public void GetSession_EmptySessionId_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.GetSessionAsync("persona1", ""));
        }

        // --- Machines ---

        [Test]
        public void MachinesList_EmptyPersonaId_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Machines.ListAsync("", "session1"));
        }

        [Test]
        public void MachinesList_EmptySessionId_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Machines.ListAsync("persona1", ""));
        }

        // --- SendMessageStreamAsync ---

        [Test]
        public void SendMessageStream_EmptyPersonaId_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.SendMessageStreamAsync("", "hello", "session1"));
        }

        [Test]
        public void SendMessageStream_EmptyMessage_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.SendMessageStreamAsync("persona1", "", "session1"));
        }

        [Test]
        public void SendMessageStream_EmptySessionId_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.SendMessageStreamAsync("persona1", "hello", ""));
        }

        // --- Null values ---

        [Test]
        public void InitSession_NullPersonaId_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.InitSessionAsync(null, "session1", "Alex", "male"));
        }

        [Test]
        public void SendMessage_NullMessage_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.SendMessageAsync("persona1", null, "session1"));
        }

        [Test]
        public void SendMessageStream_NullMessage_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _client.Chat.SendMessageStreamAsync("persona1", null, "session1"));
        }
    }
}
