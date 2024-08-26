﻿namespace SurveyBasket.Api.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken=default) where T : class;
        Task SetAsync<T>(string key, T Value,CancellationToken cancellationToken) where T : class;
        Task RemoveAsync(string key, CancellationToken cancellationToken);
    }
}
