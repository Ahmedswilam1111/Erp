using ERPtask.models;
using ERPtask.Repositrories;
using ERPtask.servcies;
using Microsoft.Extensions.Configuration;

namespace ERPtask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure database connection
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Register repositories
            builder.Services.AddScoped(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                return new ClientRepository(config);
            });

            builder.Services.AddScoped(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                return new CostEntryRepository(config);
            });

            builder.Services.AddScoped(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                return new InvoiceRepository(config);
            });

            builder.Services.AddScoped(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                return new TaxRuleRepository(config);
            });

            builder.Services.AddScoped(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                return new NotificationRepository(config);
            });

            // Register services
            builder.Services.AddScoped<ClientService>();
            builder.Services.AddScoped<CostEntryService>();
            builder.Services.AddScoped<InvoiceService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TaxRuleRepository>();

            // Configure logging
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
                loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
            });

            // Build the application
            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cost Management API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Initialize database with sample tax rules (optional)
            InitializeTaxRules(app.Services);

            app.Run();
        }

        private static void InitializeTaxRules(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var taxRepo = scope.ServiceProvider.GetRequiredService<TaxRuleRepository>();

            
            if (!taxRepo.GetAllTaxRules().Result.Any())
            {
                var defaultRules = new List<TaxRule>
            {
                new TaxRule { Region = "US-CA", TaxRate = 0.0725m },
                new TaxRule { Region = "EU", TaxRate = 0.20m },
                new TaxRule { Region = "UK", TaxRate = 0.20m }
            };

                foreach (var rule in defaultRules)
                {
                    taxRepo.CreateTaxRule(rule).Wait();
                }
            }
        }
    }
}
