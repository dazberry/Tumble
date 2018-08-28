# Tumble 
#### Pipeline + Handlers + Web API middleware = .NET Core Web API Proxy
Tumble is a pipeline, a series of handlers and a .NET core Web API middleware component that allow the creation of proxy servers in the Web API middleware pipeline.
#### The Pipeline
The pipeline is a simple pipeline similar to that used by Web API Middleware. Each pipeline handler is involved in the order they are added.
```C#
 var req = new PipelineRequest()
    .AddHandler<VersionHandler>()
    .AddHandler<RedirectHandler>();
 var ctx = new PipelineContext();
 await req.InvokeAsync(ctx);
```
#### The Handlers
Handlers can decorated or modify data in the PipelineContext. Each handler calls Invoke() to continue up the pipeline, or just exits the InvokeAsync method to return back down the pipeline.
```C# 
 public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
 {
    // going up the pipeline
    await next.Invoke();            
    // return down the pipeline
 }
```

#### The Middleware







