# Microservices: Distributed Tracing with Application Insights
This demonstrates all of the techniques that should be learned to succesfully set up distributed tracing using Application Insights.

### What you will learn
* Distributed Tracing
* Dependency Tracking
* Logging Tracing / Exceptions
* Events
* Metrics
* Sampling Rates


#### Application Flow
The application will receive `Order` data on the API and then distribute that information to `Consumer1`, which will process and distribute the message to `Consumer2`, which will then process and distribute the message to `Consumer3`.   

Api -> Consumer1 -> Consumer2 -> Consumer3   

#### Blog Post


## Setup
In Azure you will need to have set up Application Insights and Azure Service Bus.

Azure Service Bus (Topic / Subscription)
* topic1 / Consumer2
* topic2 / Consumer3ciscoc
