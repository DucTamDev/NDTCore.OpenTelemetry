using NDTCore.OpenTelemetry.Domain.Constants;
using System.Diagnostics;

namespace NDTCore.OpenTelemetry.Contact.Instrumentations
{
    public class TracingInstrumentation
    {
        public ActivitySource ActivitySource { get; }

        public TracingInstrumentation()
        {
            ActivitySource = new ActivitySource(AppTelemetry.APP_OTEL_RESOURCE_SERVICE_NAME, AppTelemetry.APP_OTEL_RESOURCE_SERVICE_VERSION);
        }

        public Activity? StartActivity(Type controllerType, string methodName, Dictionary<string, object>? additionalTags = null)
        {
            var activity = ActivitySource.StartActivity($"{controllerType.Name}.{methodName}");

            if (activity != null)
            {
                var tags = new ActivityTagsCollection
                {
                    { "event.name", $"Start {methodName}" },
                    { "timestamp", DateTime.UtcNow.ToString("o") }
                };

                if (additionalTags != null)
                {
                    foreach (var tag in additionalTags)
                    {
                        tags.Add(tag.Key, tag.Value);
                    }
                }

                activity.AddEvent(new ActivityEvent($"Start {methodName}", tags: tags));
            }

            return activity;
        }

        public void LogEvent(Activity? activity, string eventName, Dictionary<string, object>? additionalTags = null)
        {
            if (activity == null) return;

            var tags = new ActivityTagsCollection
            {
                { "event.name", eventName },
                { "timestamp", DateTime.UtcNow.ToString("o") }
            };

            if (additionalTags != null)
            {
                foreach (var tag in additionalTags)
                {
                    tags.Add(tag.Key, tag.Value);
                }
            }

            activity.AddEvent(new ActivityEvent(eventName, tags: tags));
        }

        public void HandleException(Activity? activity, Exception ex, string eventName)
        {
            if (activity == null) return;

            activity.SetTag("otel.status", "ERROR");
            activity.SetTag("otel.exception", ex.ToString());

            LogEvent(activity, eventName, new Dictionary<string, object>
            {
                { "exception.message", ex.Message },
                { "exception.stacktrace", ex.StackTrace ?? "No stack trace" }
            });
        }

        public void LogCompletion(Activity? activity, DateTime startTime, string eventName)
        {
            if (activity == null) return;

            var duration = DateTime.UtcNow - startTime;
            LogEvent(activity, eventName, new Dictionary<string, object>
            {
                { "duration.ms", duration.TotalMilliseconds }
            });
        }
    }
}
