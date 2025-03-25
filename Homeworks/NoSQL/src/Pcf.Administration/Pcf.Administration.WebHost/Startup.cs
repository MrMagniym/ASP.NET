using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.DataAccess;
using Pcf.Administration.DataAccess.Contexts;
using Pcf.Administration.DataAccess.Data;
using Pcf.Administration.DataAccess.Repositories;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Pcf.Administration.WebHost;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddMvcOptions(x =>
            x.SuppressAsyncSuffixInActionNames = false);

        BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));

        services.Configure<MongoDbSettings>(Configuration.GetSection(nameof(MongoDbSettings)));

        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.Connection);
        });

        services.AddScoped(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoDbContext(client, settings.DatabaseName);
        });

        services.AddScoped<IDbInitializer, MongoDbInitializer>();
        services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));

        services.AddOpenApiDocument(options =>
        {
            options.Title = "PromoCode Factory Administration API Doc";
            options.Version = "1.0";
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseOpenApi();
        app.UseSwaggerUi(x =>
        {
            x.DocExpansion = "list";
        });

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        dbInitializer.InitializeDb();
    }
}