using System;
using System.Threading.Tasks;

namespace Rabbit.Go.Builder
{
    public static class UseExtensions
    {
        public static IGoApplicationBuilder Use(this IGoApplicationBuilder app, Func<GoContext, Func<Task>, Task> middleware)
        {
            return app.Use(next =>
            {
                return context =>
                {
                    Func<Task> simpleNext = () => next(context);
                    return middleware(context, simpleNext);
                };
            });
        }
    }
}