using NDTCore.OpenTelemetry.Domain.Constants;
using System.Diagnostics.Metrics;

namespace NDTCore.OpenTelemetry.Contact.Instrumentations
{
    public class MetricsInstrumentation
    {
        public Meter Meter { get; }
        private Counter<int> _requestCounter;
        private Histogram<double> _requestDuration;

        public MetricsInstrumentation()
        {
            Meter = new Meter(AppTelemetry.APP_OTEL_RESOURCE_SERVICE_NAME, AppTelemetry.APP_OTEL_RESOURCE_SERVICE_VERSION);

            _requestCounter = Meter.CreateCounter<int>("api.request.count", "requests", "Counts API requests");
            _requestDuration = Meter.CreateHistogram<double>("api.request.duration", "ms", "Tracks API response times");
        }

        public void IncrementRequestCount(string route)
        {
            _requestCounter.Add(1, new KeyValuePair<string, object>("route", route));
        }

        public void RecordRequestDuration(string route, double durationMs)
        {
            _requestDuration.Record(durationMs, new KeyValuePair<string, object>("route", route));
        }
    }

}
