namespace SurveyBasket.Api.Abstractions.Consts
{
    public static class RateLimitConst
    {
        public const string concurrency = nameof(concurrency);
        public const string fixedWindow = nameof(fixedWindow);
        public const string tokenBucket = nameof(tokenBucket);
        public const string slidingWindow = nameof(slidingWindow);
        public const string ipAddress = nameof(ipAddress);
        public const string userLimit = nameof(userLimit);
    }
}
