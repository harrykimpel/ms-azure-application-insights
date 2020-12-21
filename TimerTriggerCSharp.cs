using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace Company.Function
{
    public static class TimerTriggerCSharp
    {
        static string MS_APPLICATION_INSIGHTS_APP_ID = "";
        static string MS_APPLICATION_INSIGHTS_APP_KEY = "";
        static string NR_ACCOUNT_ID = "";
        static string NR_INSIGHTS_INSERT_API_KEY = "";

        [FunctionName("TimerTriggerCSharp")]
        public static async void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            MS_APPLICATION_INSIGHTS_APP_ID = configurationBuilder["MS_APPLICATION_INSIGHTS_APP_ID"];
            MS_APPLICATION_INSIGHTS_APP_KEY = configurationBuilder["MS_APPLICATION_INSIGHTS_APP_KEY"];
            NR_ACCOUNT_ID = configurationBuilder["NR_ACCOUNT_ID"];
            NR_INSIGHTS_INSERT_API_KEY = configurationBuilder["NR_INSIGHTS_INSERT_API_KEY"];

            // Requests
            makeMSRequest(log, "msAppInsightsMetricsRequestsSample", "/metrics/", "requests/count", "sum", "end", true);
            makeMSRequest(log, "msAppInsightsMetricsRequestsSample", "/metrics/", "requests/duration", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsMetricsRequestsSample", "/metrics/", "requests/failed", "sum", "end", true);

            // Page Views
            makeMSRequest(log, "msAppInsightsPageViewsSample", "/metrics/", "pageViews/count", "sum", "end", true);
            makeMSRequest(log, "msAppInsightsPageViewsSample", "/metrics/", "pageViews/duration", "avg", "end", true);

            // Browser Timings
            makeMSRequest(log, "msAppInsightsBrowserTimingsSample", "/metrics/", "browserTimings/networkDuration", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsBrowserTimingsSample", "/metrics/", "browserTimings/sendDuration", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsBrowserTimingsSample", "/metrics/", "browserTimings/receiveDuration", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsBrowserTimingsSample", "/metrics/", "browserTimings/processingDuration", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsBrowserTimingsSample", "/metrics/", "browserTimings/totalDuration", "avg", "end", true);

            // Users
            makeMSRequest(log, "msAppInsightsUsersSample", "/metrics/", "users/count", "unique", "end", true);
            makeMSRequest(log, "msAppInsightsUsersSample", "/metrics/", "users/authenticated", "unique", "end", true);

            // Sessions
            makeMSRequest(log, "msAppInsightsSessionsSample", "/metrics/", "sessions/count", "unique", "end", true);

            // Custom Events
            makeMSRequest(log, "msAppInsightsCustomEventsSample", "/metrics/", "customEvents/count", "sum", "end", true);

            // Dependencies
            makeMSRequest(log, "msAppInsightsDependenciesSample", "/metrics/", "dependencies/count", "sum", "end", true);
            makeMSRequest(log, "msAppInsightsDependenciesSample", "/metrics/", "dependencies/failed", "sum", "end", true);
            makeMSRequest(log, "msAppInsightsDependenciesSample", "/metrics/", "dependencies/duration", "avg", "end", true);

            // Exceptions
            makeMSRequest(log, "msAppInsightsExceptionsSample", "/metrics/", "exceptions/count", "sum", "end", true);
            makeMSRequest(log, "msAppInsightsExceptionsSample", "/metrics/", "exceptions/browser", "sum", "end", true);
            makeMSRequest(log, "msAppInsightsExceptionsSample", "/metrics/", "exceptions/server", "sum", "end", true);

            // Availability Results
            makeMSRequest(log, "msAppInsightsAvailabilityResultsSample", "/metrics/", "availabilityResults/count", "sum", "end", true);
            makeMSRequest(log, "msAppInsightsAvailabilityResultsSample", "/metrics/", "availabilityResults/duration", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsAvailabilityResultsSample", "/metrics/", "availabilityResults/availabilityPercentage", "avg", "end", true);

            // Billing Meters
            makeMSRequest(log, "msAppInsightsBillingMetersSample", "/metrics/", "billingMeters/telemetryCount", "sum", "end", true);
            makeMSRequest(log, "msAppInsightsBillingMetersSample", "/metrics/", "billingMeters/telemetrySize", "sum", "end", true);

            // Performance Counters
            makeMSRequest(log, "msAppInsightsPerformanceCountersSample", "/metrics/", "performanceCounters/requestExecutionTime", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsPerformanceCountersSample", "/metrics/", "performanceCounters/requestsPerSecond", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsPerformanceCountersSample", "/metrics/", "performanceCounters/requestsInQueue", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsPerformanceCountersSample", "/metrics/", "performanceCounters/memoryAvailableBytes", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsPerformanceCountersSample", "/metrics/", "performanceCounters/exceptionsPerSecond", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsPerformanceCountersSample", "/metrics/", "performanceCounters/processCpuPercentage", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsPerformanceCountersSample", "/metrics/", "performanceCounters/processCpuPercentageTotal", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsPerformanceCountersSample", "/metrics/", "performanceCounters/processIOBytesPerSecond", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsPerformanceCountersSample", "/metrics/", "performanceCounters/processPrivateBytes", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsPerformanceCountersSample", "/metrics/", "performanceCounters/processorCpuPercentage", "avg", "end", true);

            // Traces
            makeMSRequest(log, "msAppInsightsTracesSample", "/metrics/", "traces/count", "sum", "end", true);

            // Custom Metrics
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CCPU%20Usage", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CWorking%20Set", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CGC%20Heap%20Size", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CGen%200%20GC%20Count", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CGen%201%20GC%20Count", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CGen%202%20GC%20Count", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CException%20Count", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CThreadPool%20Thread%20Count", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CMonitor%20Lock%20Contention%20Count", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CThreadPool%20Queue%20Length", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CThreadPool%20Completed%20Work%20Item%20Count", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7C%25%20Time%20in%20GC%20since%20last%20GC", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CGen%200%20Size", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CGen%201%20Size", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CGen%202%20Size", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CLOH%20Size", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CAllocation%20Rate", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CNumber%20of%20Assemblies%20Loaded", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FSystem.Runtime%7CNumber%20of%20Active%20Timers", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomMetricsSample", "/metrics/", "customMetrics%2FHeartbeatState", "avg", "end", true);

            // Custom Events
            makeMSRequest(log, "msAppInsightsCustomEventsSample", "/metrics/", "customEvents%2Fcustom%2FSkippedExceptions", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomEventsSample", "/metrics/", "customEvents%2Fcustom%2FCannotSnapshotDueToMemoryUsage", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomEventsSample", "/metrics/", "customEvents%2Fcustom%2FFirstChanceExceptions", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomEventsSample", "/metrics/", "customEvents%2Fcustom%2FSnapshotDailyRateLimitReached", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomEventsSample", "/metrics/", "customEvents%2Fcustom%2FSnapshotRateLimitExceeded", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomEventsSample", "/metrics/", "customEvents%2Fcustom%2FSnappointMatchExceptions", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomEventsSample", "/metrics/", "customEvents%2Fcustom%2FCollectionPlanComplete", "avg", "end", true);
            makeMSRequest(log, "msAppInsightsCustomEventsSample", "/metrics/", "customEvents%2Fcustom%2FTrackExceptionCalls", "avg", "end", true);

            // Event Requests
            makeMSRequest(log, "msAppInsightsEventRequestsSample", "/events/", "traces", "sum", "timestamp", true);
            makeMSRequest(log, "msAppInsightsEventRequestsSample", "/events/", "customEvents", "sum", "timestamp", true);
            makeMSRequest(log, "msAppInsightsEventRequestsSample", "/events/", "pageViews", "sum", "timestamp", true);
            makeMSRequest(log, "msAppInsightsEventRequestsSample", "/events/", "browserTimings", "sum", "timestamp", true);
            makeMSRequest(log, "msAppInsightsEventRequestsSample", "/events/", "requests", "sum", "timestamp", true);
            makeMSRequest(log, "msAppInsightsEventRequestsSample", "/events/", "dependencies", "sum", "timestamp", true);
            makeMSRequest(log, "msAppInsightsEventRequestsSample", "/events/", "exceptions", "sum", "timestamp", true);
        }

        private static async Task makeMSRequest(ILogger log, string nrEventType, string dataType, string metricValue, string aggregationFunction, string timestampField, bool isLoggingEnabled)
        {
            var app_insights_base_url = "https://api.applicationinsights.io";
            var app_insights_api = "/v1/apps/";
            var app_insights_timespan = "PT5M"; // return data for the last 5 minutes
            var app_id = MS_APPLICATION_INSIGHTS_APP_ID;
            var api_key = MS_APPLICATION_INSIGHTS_APP_KEY;

            var urlAppInsights = app_insights_base_url + app_insights_api + app_id + dataType + metricValue + "?timespan=" + app_insights_timespan;

            if (isLoggingEnabled)
            {
                log.LogInformation("urlAppInsights: " + urlAppInsights);
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-api-key", api_key);
            var response = await client.GetAsync(urlAppInsights);

            string result = await response.Content.ReadAsStringAsync();
            if (isLoggingEnabled)
            {
                log.LogInformation("MS result: " + result);
            }
            JObject o = JObject.Parse(result);
            JToken jt = o["value"];

            if (dataType != "/events/")
            {
                string endTS = (string)jt[timestampField];
                if (isLoggingEnabled)
                {
                    log.LogInformation("MS end: " + endTS);
                }

                var date = DateTime.Parse(endTS, null, System.Globalization.DateTimeStyles.RoundtripKind);
                var unixTimestamp = (long)(date.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

                var nrJObject = new JObject();
                JProperty eventType = new JProperty("eventType", nrEventType);
                nrJObject.Add(eventType);
                JProperty timestamp = new JProperty("timestamp", unixTimestamp);
                nrJObject.Add(timestamp);
                JProperty end = new JProperty(timestampField, jt[timestampField]);
                nrJObject.Add(end);
                JProperty start = new JProperty("start", jt["start"]);
                nrJObject.Add(start);
                string propKey = metricValue.Replace("/", "");
                propKey += aggregationFunction;
                JProperty requestsCountSum = new JProperty(propKey, jt[metricValue][aggregationFunction]);
                nrJObject.Add(requestsCountSum);

                await makeNRRequest(log, nrJObject.ToString(), isLoggingEnabled);
            }
            else
            {
                JArray ja = new JArray();

                foreach (JToken jtValue in jt)
                {
                    string endTS = (string)jtValue[timestampField];

                    var date = DateTime.Parse(endTS, null, System.Globalization.DateTimeStyles.RoundtripKind);
                    var unixTimestamp = (long)(date.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

                    var nrJObject = new JObject();
                    JProperty eventType = new JProperty("eventType", nrEventType);
                    nrJObject.Add(eventType);
                    JProperty timestamp = new JProperty("timestamp", unixTimestamp);
                    nrJObject.Add(timestamp);
                    JProperty requestId = new JProperty("requestId", jtValue["request"]["id"]);
                    nrJObject.Add(requestId);
                    JProperty requestName = new JProperty("requestName", jtValue["request"]["name"]);
                    nrJObject.Add(requestName);
                    JProperty requestSuccess = new JProperty("requestSuccess", jtValue["request"]["success"]);
                    nrJObject.Add(requestSuccess);
                    JProperty requestDuration = new JProperty("requestDuration", jtValue["request"]["duration"]);
                    nrJObject.Add(requestDuration);
                    JProperty requestPerformanceBucket = new JProperty("requestPerformanceBucket", jtValue["request"]["performanceBucket"]);
                    nrJObject.Add(requestPerformanceBucket);
                    JProperty requestResultCode = new JProperty("requestResultCode", jtValue["request"]["resultCode"]);
                    nrJObject.Add(requestResultCode);
                    JProperty requestUrl = new JProperty("requestUrl", jtValue["request"]["url"]);
                    nrJObject.Add(requestUrl);
                    JProperty requestSource = new JProperty("requestSource", jtValue["request"]["source"]);
                    nrJObject.Add(requestSource);
                    JProperty clientType = new JProperty("clientType", jtValue["client"]["type"]);
                    nrJObject.Add(clientType);
                    JProperty clientModel = new JProperty("clientModel", jtValue["client"]["model"]);
                    nrJObject.Add(clientModel);
                    JProperty clientOS = new JProperty("clientOS", jtValue["client"]["os"]);
                    nrJObject.Add(clientOS);
                    JProperty clientIP = new JProperty("clientIP", jtValue["client"]["ip"]);
                    nrJObject.Add(clientIP);
                    JProperty clientCity = new JProperty("clientCity", jtValue["client"]["city"]);
                    nrJObject.Add(clientCity);
                    JProperty clientCountryOrRegion = new JProperty("clientCountryOrRegion", jtValue["client"]["countryOrRegion"]);
                    nrJObject.Add(clientCountryOrRegion);
                    JProperty clientBrowser = new JProperty("clientBrowser", jtValue["client"]["browser"]);
                    nrJObject.Add(clientBrowser);

                    ja.Add(nrJObject);
                }

                await makeNRRequest(log, ja.ToString(), isLoggingEnabled);
            }
        }

        private static async Task makeNRRequest(ILogger log, string json, bool isLoggingEnabled)
        {
            if (isLoggingEnabled)
            {
                log.LogInformation("NR payload: " + json);
            }

            var data = new StringContent(json);

            data.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var url = "https://insights-collector.newrelic.com/v1/accounts/" + NR_ACCOUNT_ID + "/events";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Insert-Key", NR_INSIGHTS_INSERT_API_KEY);
            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;

            // write output as summary
            if (isLoggingEnabled)
            {
                log.LogInformation("NR result: " + result);
            }
        }
    }
}
