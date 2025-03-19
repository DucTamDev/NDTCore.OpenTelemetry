using NDTCore.OpenTelemetry.Domain.Constants;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;

namespace NDTCore.OpenTelemetry.Contract.Instrumentations
{
    public class MetricsInstrumentation
    {
        private Meter Meter { get; }
        private readonly ConcurrentDictionary<string, Counter<int>> _counters = new();
        private readonly ConcurrentDictionary<string, Histogram<double>> _histograms = new();

        public MetricsInstrumentation()
        {
            Meter = new Meter(AppTelemetry.APP_OTEL_RESOURCE_SERVICE_NAME, AppTelemetry.APP_OTEL_RESOURCE_SERVICE_VERSION);
        }

        public Meter GetMeter() => this.Meter;

        /// <summary>
        /// Cleans the metric name by:
        /// - Converting to lowercase
        /// - Replacing non-alphanumeric characters with underscores
        /// - Removing leading and trailing underscores
        /// </summary>
        private static string CleanMetricName(string name)
        {
            return Regex.Replace(name.ToLower(), @"[^a-z0-9]+", "_").Trim('_');
        }

        /// <summary>
        /// Records the number of times a function is called.
        /// </summary>
        public void RecordFunctionCall(string functionName)
        {
            var cleanName = CleanMetricName($"function.{functionName}.count");

            var counter = _counters.GetOrAdd(cleanName, name =>
                Meter.CreateCounter<int>(name, "calls", $"Total calls for {functionName}"));

            counter.Add(1);
        }

        /// <summary>
        /// Records the execution duration of a function in milliseconds.
        /// </summary>
        public void RecordExecutionTime(string functionName, double milliseconds)
        {
            var cleanName = CleanMetricName($"function.{functionName}.duration");

            var histogram = _histograms.GetOrAdd(cleanName, name =>
                Meter.CreateHistogram<double>(name, "ms", $"Execution time for {functionName}"));

            histogram.Record(milliseconds);
        }

        /// <summary>
        /// Records the number of errors encountered in a function.
        /// </summary>
        public void RecordError(string functionName)
        {
            var cleanName = CleanMetricName($"function.{functionName}.error.count");

            var counter = _counters.GetOrAdd(cleanName, name =>
                Meter.CreateCounter<int>(name, "errors", $"Total errors for {functionName}"));

            counter.Add(1);
        }

        /// <summary>
        /// Records a custom metric with a dynamic name, value, unit, and description.
        /// </summary>
        public void RecordCustomMetric(string metricName, double value, string unit = "", string description = "")
        {
            var cleanName = CleanMetricName(metricName);

            var histogram = _histograms.GetOrAdd(cleanName, name =>
                Meter.CreateHistogram<double>(name, unit, description));

            histogram.Record(value);
        }
    }
}
