﻿namespace NDTCore.OpenTelemetry.Infrastructure.ExternalServices.MongoDb
{
    public class MongoDbOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
