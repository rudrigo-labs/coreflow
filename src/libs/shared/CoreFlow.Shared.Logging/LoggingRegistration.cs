using CoreFlow.Shared.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFlow.Shared.Logging
{
    public static class LoggingRegistration
    {
        /// <summary>
        /// builder.Host.AddLogger(builder.Configuration);
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="configuration"></param>
        public static void AddLogger(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            hostBuilder.AddSerilogConfiguration(configuration);

            // Adiciona LoggerService no DI
            hostBuilder.ConfigureServices(services =>
            {
                services.AddLoggerService();
            });
        }
        static void AddSerilogConfiguration(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ApplicationConnection");

            var columnOptions = new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn("SourceContext", SqlDbType.NVarChar, dataLength: 200),
                    new SqlColumn("UserName", SqlDbType.NVarChar, dataLength: 100)
                }
            };

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "Logs",
                        AutoCreateSqlTable = true
                    },
                    columnOptions: columnOptions
                )
                .CreateLogger();

            hostBuilder.UseSerilog();
        }

        static void AddLoggerService(this IServiceCollection services)
        {
            services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));
        }
    }
}

