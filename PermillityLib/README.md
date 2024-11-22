# Permillity

Permillity is a lightweight endpoint performance tracker designed to monitor each request in your application. It is simple to integrate, making it ideal for small to medium-sized projects or applications. With support for in-memory, SQL Server, and MySQL backends, Permillity helps you gain insights into your application's performance effortlessly.

## Overview

Permillity tracks and logs requests in your application, offering a dashboard for visualizing performance data and plain statistics for custom usage. The library is lightweight, flexible, and easy to configure, ensuring minimal overhead while providing meaningful insights.

### Key Features:
- **Lightweight**: Optimized for small and medium-sized projects.
- **Database Options**: Supports in-memory, SQL Server, and MySQL.
- **Customizable**: Includes options to ignore specific endpoints or batch process data.
- **Dashboard and Data**: View a pre-built dashboard or access raw performance data.


## Installation

Permillity is available as a NuGet package. You can install it using the NuGet Package Console window:

```
PM> Install-Package Permillity
```

## Usage

To register Permillity add the following lines in Program.cs class:

```
// Register Permillity services
builder.Services.AddPermillity();
```

```
// Add Permillity middleware
app.AddPermillityMiddleware();
```

You can customize Permillity during servise registration. AddPermillity() method accepts options builder as a parameter.

### Configuration options:
- **UseInMemory()**: Permillity will use in-memory storage. This option is default.
- **UseSqlServer(conString)**: Permillity will use ms sql server as a storage. Requires connection string as a parameter.
- **UseMySql(conString)**: Permillity will use my sql as a storage. Requires connection string as a parameter.
- **UseLogger()**: Permillity will use logger if available to log errors. Default is false.
- **SetBatchSize(50)**: Permillity will sync data with storage each N requests. Default is 50. For real-time sync use 0.
- **SetIgnoreEndpoints(["WeatherForecast"])**: Permillity will ignore listed endpoints.

Example registering Permillity to MS SQL Server with logger, batch size and list of endpoints to ignore:

```
var conString = "ConnectionString";

builder.Services.AddPermillity(x =>
{
    x.UseSqlServer(conString); //use sql server 
    x.UseLogger(); //use logger
    x.SetBatchSize(0); //set batch size to 0 for real time tracking
    x.SetIgnoreEndpoints(["api/sign-up", "api/employees"]); //ignore tracking of sign-up and employees enpoints
});
```

You can also ignore specific endpoints by adding a [PermillityAvoid] attribute:

```
[PermillityAvoid]
[HttpGet()]
public IActionResult TestEndpoint()
{
    return Ok("This endpoint will not be logged.");
}
```

### Dashboard

To render the Permillity dashboard in an endpoint, inject IPermillityService and use the GetDashboardAsync method:

```
[HttpGet("dashboard")]
public async Task<IActionResult> GetDashboard([FromServices] IPermillityService permillityService)
{
    var html = await permillityService.GetDashboardAsync();
    return Content(html, "text/html");
}
```

For raw statistics, use the GetStatisticsDataAsync method:

```
[HttpGet()]
public async Task<IActionResult> GetStatistics([FromServices] IPermillityService permillityService)
{
    var data = await permillityService.GetStatisticsDataAsync();
    return Ok(data);
}
```

## Technical info

When using sql server or mysql databases Permillity will create one table for tracking data: ApiStats. Statistics are kept by endpoint by week and year. If you want to clear statistics when it become burdensome just delete data from the table. There is possibility that some request will not be logged (for example, if app crashes or is shut down before current batch is synced with database.) Statistics numbers should be used as reference only. 
