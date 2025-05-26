using Castle.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.DataAccess;
using Pcf.ReceivingFromPartner.DataAccess.Data;
using Pcf.ReceivingFromPartner.DataAccess.Repositories;
using Pcf.ReceivingFromPartner.Integration;
using Pcf.ReceivingFromPartner.Integration.Services;
using System;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Pcf.ReceivingFromPartner.WebHost;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddMvcOptions(x => x.SuppressAsyncSuffixInActionNames = false);
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<INotificationGateway, NotificationGateway>();
        services.AddScoped<IDbInitializer, EfDbInitializer>();

        services.AddScoped<IGivingPromoCodeToCustomerGateway, GivingPromoCodeToCustomerGateway>();
        services.AddScoped<IPromocodeService>(sp =>
        {
            var url = Configuration.GetSection("IntegrationSettings").GetValue<string>("GivingToCustomerApiGrpcUrl");
            return new PromocodeService(new Uri(url));
        });
        services.AddHttpClient<IAdministrationGateway, AdministrationGateway>(c =>
        {
            c.BaseAddress = new Uri(Configuration.GetSection("IntegrationSettings").GetValue<string>("AdministrationApiUrl"));
        });

        services.AddDbContext<DataContext>(x =>
        {
            //x.UseSqlite("Filename=PromocodeFactoryReceivingFromPartnerDb.sqlite");
            x.UseNpgsql(Configuration.GetSection("ConnectionStrings").GetValue<string>("PromocodeFactoryReceivingFromPartnerDb"));
            x.UseSnakeCaseNamingConvention();
            x.UseLazyLoadingProxies();
        });

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddOpenApiDocument(options =>
        {
            options.Title = "PromoCode Factory Receiving From Partner API Doc";
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