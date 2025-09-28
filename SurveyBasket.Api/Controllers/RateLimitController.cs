using Microsoft.AspNetCore.RateLimiting;

namespace SurveyBasket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RateLimitController : ControllerBase
    {

        //[DisableRateLimiting]

        [HttpGet("test-rate-concurrency")]
        [EnableRateLimiting(RateLimitConst.concurrency)]
        public IActionResult ConcurrencyRate()
        {
            Thread.Sleep(6000);
            return Ok();
        }
        [HttpGet("test-rate-fixed-window")]
        [EnableRateLimiting(RateLimitConst.fixedWindow)]
        public IActionResult FixedWindowRate()
        {
            Thread.Sleep(6000);
            return Ok();
        }
        [HttpGet("test-rate-token-bucket")]
        [EnableRateLimiting(RateLimitConst.tokenBucket)]
        public IActionResult TokenBucketRate()
        {
            Thread.Sleep(6000);
            return Ok();
        }
        [HttpGet("test-rate-sliding-window")]
        [EnableRateLimiting(RateLimitConst.slidingWindow)]
        public IActionResult SlidingWindowRate()
        {
            Thread.Sleep(6000);
            return Ok();
        }

        [HttpGet("test-rate-ip-adress")]
        [EnableRateLimiting(RateLimitConst.ipAddress)]
        public IActionResult IpAdressRate()
        {
            Thread.Sleep(6000);
            return Ok();
        }

        [HasPermission(Permissions.GetQuestions)]
        [HttpGet("test-rate-uset")]
        [EnableRateLimiting(RateLimitConst.userLimit)]
        public IActionResult UserRate()
        {
            Thread.Sleep(6000);
            return Ok();
        }
    }
}
