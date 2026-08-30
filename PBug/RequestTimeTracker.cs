using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace PBug;

public class RequestTimeFeature
{
    long startTs;
    public double ElapsedMilliseconds => Stopwatch.GetElapsedTime(startTs).Ticks / TimeSpan.TicksPerMillisecond;
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
