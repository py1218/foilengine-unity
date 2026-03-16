using NUnit.Framework;
using FoilEngine;

namespace FoilEngine.Tests
{
    public class ErrorTests
    {
        [Test]
        public void FoilEngineException_StoresMessage()
        {
            var ex = new FoilEngineException("test error");
            Assert.AreEqual("test error", ex.Message);
        }

        [Test]
        public void FoilEngineException_StoresStatusCode()
        {
            var ex = new FoilEngineException("test", 418);
            Assert.AreEqual(418, ex.StatusCode);
        }

        [Test]
        public void AuthenticationException_IsFoilEngineException()
        {
            var ex = new AuthenticationException("bad key");
            Assert.IsInstanceOf<FoilEngineException>(ex);
            Assert.AreEqual("bad key", ex.Message);
        }

        [Test]
        public void NotFoundExceptio_IsFoilEngineException()
        {
            var ex = new NotFoundException("missing");
            Assert.IsInstanceOf<FoilEngineException>(ex);
        }

        [Test]
        public void BadRequestException_IsFoilEngineException()
        {
            var ex = new BadRequestException("invalid");
            Assert.IsInstanceOf<FoilEngineException>(ex);
        }

        [Test]
        public void ForbiddenException_IsFoilEngineException()
        {
            var ex = new ForbiddenException("denied");
            Assert.IsInstanceOf<FoilEngineException>(ex);
        }

        [Test]
        public void RateLimitException_IsFoilEngineException()
        {
            var ex = new RateLimitException("slow down");
            Assert.IsInstanceOf<FoilEngineException>(ex);
        }

        [Test]
        public void ServerException_IsFoilEngineException()
        {
            var ex = new ServerException("oops");
            Assert.IsInstanceOf<FoilEngineException>(ex);
        }
    }
}
