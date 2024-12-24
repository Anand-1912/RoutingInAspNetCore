
namespace RoutingInAspNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddHealthChecks();

            var app = builder.Build();

            app.Use(
                async (context, next)
                =>
                {
                    Console.WriteLine(context.Request.Path);
                    await next(context);
                });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Location 1: before routing runs, endpoint is always null here.
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"1. Endpoint: {context.GetEndpoint()?.DisplayName ?? "(null)"}");
                await next(context);
            });

            app.UseRouting();

            // Location 2: after routing runs, endpoint will be non-null if routing found a match.
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"2. Endpoint: {context.GetEndpoint()?.DisplayName ?? "(null)"}");
                await next(context);
            });

            // Location 3: runs when this endpoint matches
            app.MapGet("/", (HttpContext context) =>
            {
                Console.WriteLine($"3. Endpoint: {context.GetEndpoint()?.DisplayName ?? "(null)"}");
                return "Hello World!";
            }).WithDisplayName("Hello");

            app.MapGet("/products/{id:int}", (int id) => $"products with {id}");
            
            app.Use(async (context, next) =>
            {
                var currentEndpoint = context.GetEndpoint();

                if (currentEndpoint is null)
                {
                    await next(context);
                    return;
                }

                Console.WriteLine($"Endpoint: {currentEndpoint.DisplayName}");

                if (currentEndpoint is RouteEndpoint routeEndpoint)
                {
                    Console.WriteLine($"  - Route Pattern: {routeEndpoint.RoutePattern}");
                }

                foreach (var endpointMetadata in currentEndpoint.Metadata)
                {
                    Console.WriteLine($"  - Metadata: {endpointMetadata}");
                }

                await next(context);
            });

            app.UseEndpoints(_ => { });

            // Location 4: runs after UseEndpoints - will only run if there was no match.
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"4. Endpoint: {context.GetEndpoint()?.DisplayName ?? "(null)"}");
                await next(context);
            });
            app.MapHealthChecks("/healthz").WithDisplayName("/HealthEndpoint");

            app.Run();
        }
    }
}
