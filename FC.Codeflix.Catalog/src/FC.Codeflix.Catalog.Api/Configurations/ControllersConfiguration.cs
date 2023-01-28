using FC.Codeflix.Catalog.Api.Configurations.Policies;
using FC.Codeflix.Catalog.Api.Filters;
using System.Text.Json;

namespace FC.Codeflix.Catalog.Api.Configurations
{
    public static class ControllersConfiguration
    {
        public static IServiceCollection AddAndConfigureControllers(this IServiceCollection services)
        {
            services
                .AddControllers(options => options.Filters.Add(typeof(ApiGlobalExceptionFilter)))
                .AddJsonOptions(jsonOptions => jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCasePolicy());
            
            services.AddDocumentation();

            return services;
        }

        private static IServiceCollection AddDocumentation(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        public static WebApplication UseDocumentation(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            return app;
        }
    }
}
