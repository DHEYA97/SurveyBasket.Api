using SurveyBasket.Api;
using SurveyBasket.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

//Add Custom AddDependency
builder.Services.AddDependency(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(); //app.UseCors("Policy Name");
app.UseAuthorization();
app.MapControllers();

app.Run();
