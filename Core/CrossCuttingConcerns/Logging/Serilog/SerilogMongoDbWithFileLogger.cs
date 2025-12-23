using Core.CrossCuttingConcerns.Logging.Configurations;
using Serilog.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Logging.Serilog;

public class SerilogMongoDbWithFileLogger : SerilogLoggerServiceBase
{
    public SerilogMongoDbWithFileLogger(MongoDbLogConfiguration mongoDbConfiguration, FileLogConfiguration fileLogConfiguration) : base(logger: null!)
    {
        Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                path: $"{Directory.GetCurrentDirectory() + fileLogConfiguration.FolderPath}.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: null,
                fileSizeLimitBytes: 5000000,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
            )
            .WriteTo.MongoDB(
                databaseUrl: $"{mongoDbConfiguration.ConnectionString}",
                collectionName: mongoDbConfiguration.Collection,
                restrictedToMinimumLevel: LogEventLevel.Error
            )
            .CreateLogger();
    }
}
