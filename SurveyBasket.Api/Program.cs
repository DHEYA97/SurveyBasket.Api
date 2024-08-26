using Serilog;
using SurveyBasket.Api;
using SurveyBasket.Api.Middleware;
using SurveyBasket.Api.Persistence;

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
    app.UseSwaggerUI();
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

app.MapControllers();

//Handling Exception Before .Net 8
//app.UseMiddleware<ExceptionHandlingMiddleware>();
//Handling Exception After .Net 8
app.UseExceptionHandler();
app.Run();
