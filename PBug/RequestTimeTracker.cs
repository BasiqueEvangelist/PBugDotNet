using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PBug;

public class RequestTimeFeature
{
    long startTs;
    public TimeSpan Elapsed => Stopwatch.GetElapsedTime(startTs);
    public RequestTimeFeature(long startTs)
    {
        this.startTs = startTs;
    }

    public static Task Middleware(HttpContext ctx, RequestDelegate next)
    {
        ctx.Features.Set(new RequestTimeFeature(Stopwatch.GetTimestamp()));
        return next(ctx);
    }
}
