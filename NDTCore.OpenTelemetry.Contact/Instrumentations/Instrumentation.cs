namespace NDTCore.OpenTelemetry.Contact.Instrumentations
{
    public class Instrumentation : IDisposable
    {
        public TracingInstrumentation Tracing { get; }
        public MetricsInstrumentation Metrics { get; }

        public Instrumentation()
        {
            Tracing = new TracingInstrumentation();
            Metrics = new MetricsInstrumentation();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Tracing.ActivitySource.Dispose();
            Metrics.Meter.Dispose();
        }

        ~Instrumentation()
        {
            Dispose(false);
        }
    }
}
