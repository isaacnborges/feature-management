using feature_flags;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

ConfigureService(builder);
Configure(builder);

static void ConfigureService(WebApplicationBuilder builder)
{
    if (!builder.Environment.IsDevelopment())
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options
                .Connect(builder.Configuration["ConnectionStrings:AzureAppConfig"])
                .UseFeatureFlags(options => options.CacheExpirationInterval = TimeSpan.FromSeconds(10));
        });
    }

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddAzureAppConfiguration();

    builder.Services
        .AddFeatureManagement()
        .AddFeatureFilter<PercentageFilter>()
        .AddFeatureFilter<BrowserFilter>();

    builder.Services
        .AddControllers()
        .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

static void Configure(WebApplicationBuilder builder)
{
    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    if (!app.Environment.IsDevelopment())
        app.UseAzureAppConfiguration();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}