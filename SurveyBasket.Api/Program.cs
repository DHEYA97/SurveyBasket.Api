using Hangfire;
using HangfireBasicAuthenticationFilter;
using Serilog;
using SurveyBasket.Api;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
var builder = WebApplication.CreateBuilder(args);

//Add serilog
builder.Host.UseSerilog((context, configration) =>
{
    configration.ReadFrom.Configuration(context.Configuration);
});
#region Cache
//// Add ResponseCaching
//builder.Services.AddResponseCaching();

//// Add AddOutputCache
//builder.Services.AddOutputCache(option =>
//{
//    option.AddPolicy("CachePolicy", x =>
//    {
//        x.Cache()
//         .Expire(TimeSpan.FromSeconds(120))
//         .Tag("CacheTag");
//    });
//});

////Add In Memory Cache
//builder.Services.AddMemoryCache();
//Add DistributedMemoryCache
//builder.Services.AddDistributedMemoryCache();
#endregion

//Add Custom AddDependency
builder.Services.AddDependency(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SurveyBasket API V1");
    });
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseCors(); //app.UseCors("Policy Name");
app.UseAuthorization();

#region Cache
////AddResponseCaching
//app.UseResponseCaching();
////Add OutputCache
//app.UseOutputCache();
#endregion
app.UseHangfireDashboard("/jobs",new DashboardOptions
{
    Authorization =
    [
        new HangfireCustomBasicAuthenticationFilter
        {
            User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
            Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
    ],
    DashboardTitle = "Survey Basket Dashboard",
});

//var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
//using var scope = scopeFactory.CreateScope();
//var notificationService = scope.ServiceProvider.GetService<INotificationService>();
//RecurringJob.AddOrUpdate("SendEmailInBackgroundJob", () => notificationService!.SendEmailInBackgroundJob(null), Cron.Daily);

app.MapControllers();

//Handling Exception Before .Net 8
//app.UseMiddleware<ExceptionHandlingMiddleware>();
//Handling Exception After .Net 8
app.UseExceptionHandler();

//HealthChecks
app.MapHealthChecks("health",new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
//Optional HealthChecks
app.MapHealthChecks("health-api", new HealthCheckOptions
{
    Predicate = x=> x.Tags.Contains("api"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});


//Rate Limit
app.UseRateLimiter();

app.Run();
