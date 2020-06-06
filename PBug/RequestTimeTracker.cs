using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace PBug
{
    public class RequestTimeFeature
    {
        public Stopwatch Stopwatch { get; set; }
    }

    public class RequestTimeTracker
    {
        public RequestDelegate Use(RequestDelegate next)
        {
            return async (ctx) =>
            {
                var f = new RequestTimeFeature() { Stopwatch = new Stopwatch() };
                ctx.Features.Set(f);
                f.Stopwatch.Start();
                await next(ctx);
            };
        }
    }
}