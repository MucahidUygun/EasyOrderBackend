using Core.CrossCuttingConcerns.Logging.Configurations;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Logging.Serilog;

public class SerilogMongoDbLogger : SerilogLoggerServiceBase
{
    public SerilogMongoDbLogger(MongoDbLogConfiguration mongoDbConfiguration) : base(logger: null!)
    {
        Logger = new LoggerConfiguration()
            .WriteTo.MongoDB(
                databaseUrl: $"{mongoDbConfiguration.ConnectionString}/{mongoDbConfiguration.Database}",
                collectionName: mongoDbConfiguration.Collection,
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            .CreateLogger();
    }
}
