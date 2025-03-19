using NDTCore.OpenTelemetry.Domain.Constants;
using System.Diagnostics;

namespace NDTCore.OpenTelemetry.Contract.Instrumentations
{
    public class TracingInstrumentation
    {
        private ActivitySource ActivitySource { get; }

        public TracingInstrumentation()
        {
            ActivitySource = new(AppTelemetry.APP_OTEL_RESOURCE_SERVICE_NAME, AppTelemetry.APP_OTEL_RESOURCE_SERVICE_VERSION);
        }

        public ActivitySource GetActivitySource() => this.ActivitySource;

        public Activity? StartActivity(Type classType, string methodName, Dictionary<string, object>? additionalTags = null)
        {
            string activityName = $"{classType.FullName}.{methodName}";

            var activity = ActivitySource.StartActivity(activityName);
            if (activity == null) return null;

            var tags = new ActivityTagsCollection
            {
                { "event.name", $"Start {methodName}" },
                { "timestamp", DateTime.UtcNow.ToString("o") }
            };

            AddTagsToCollection(tags, additionalTags);
            activity.AddEvent(new ActivityEvent($"Start {methodName}", tags: tags));

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

            AddTagsToCollection(tags, additionalTags);
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

        public void LogCompletion(Activity? activity, double duration, string eventName)
        {
            if (activity == null) return;

            LogEvent(activity, eventName, new Dictionary<string, object>
            {
                { "duration.ms", duration }
            });
        }

        private static void AddTagsToCollection(ActivityTagsCollection tags, Dictionary<string, object>? additionalTags)
        {
            if (additionalTags == null) return;

            foreach (var tag in additionalTags)
            {
                tags.Add(tag.Key, tag.Value);
            }
        }
    }
}
