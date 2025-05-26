using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.DataAccess;
using Pcf.GivingToCustomer.DataAccess.Data;
using Pcf.GivingToCustomer.DataAccess.Repositories;
using Pcf.GivingToCustomer.Integration;
using Pcf.GivingToCustomer.WebHost.Services;
using System;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Pcf.GivingToCustomer.WebHost;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();
        services.AddControllers().AddMvcOptions(x => x.SuppressAsyncSuffixInActionNames = false);
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<INotificationGateway, NotificationGateway>();
        services.AddScoped<IDbInitializer, EfDbInitializer>();
        services.AddDbContext<DataContext>(x =>
        {
            //x.UseSqlite("Filename=PromocodeFactoryGivingToCustomerDb.sqlite");            
            x.UseNpgsql(Configuration.GetSection("ConnectionStrings").GetValue<string>("PromocodeFactoryGivingToCustomerDb"));
            x.UseSnakeCaseNamingConvention();
            x.UseLazyLoadingProxies();
        });

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddOpenApiDocument(options =>
        {
            options.Title = "PromoCode Factory Giving To Customer API Doc";
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
            endpoints.MapGrpcService<PromocodeService>();
        });

        dbInitializer.InitializeDb();
    }
}