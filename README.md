# Microsoft Azure Application Insights integration for New Relic

Microsoft provides an easy way to query and integrate with the performance, availability and usage data collected by Application Insights for your application. You can access all your app's event and metric data with a powerful and simple [REST API](https://dev.applicationinsights.io/reference).

In this repo, I will provide two options on how you could integrate events and metrics from Application Insights into [New Relic's Telemetry Data Platform](https://newrelic.com/platform/telemetry-data-platform).

## Azure Function
An Azure Function is probably the easiest way to continuously gather data from Application Insights API and send the data to New Relic. The example assumes that the Azure Function will be time triggered on a 5 minute basis. The timespan configured in the requests to Application Insights API is configured to also reflect the last 5 minutes.

The following application settings need to be provided in the Azure Function configuration:

```
MS_APPLICATION_INSIGHTS_APP_ID: Application ID from the Application Insights resource for your application
MS_APPLICATION_INSIGHTS_APP_KEY: Application API key from the Application Insights resource for your application
NR_INSIGHTS_INSERT_API_KEY: New Relic Insights Insert API Key
```

## New Relic Flex Config
Another option to integrate with a [New Relic Flex](https://github.com/newrelic/nri-flex) configuration. An example configuration is provided [here](https://github.com/newrelic/nri-flex/blob/master/examples/microsoft-app-insights.yml).
