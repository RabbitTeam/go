using Microsoft.Extensions.DependencyInjection;
using Rabbit.Go.Core.Reflective;
using Rabbit.Go.Internal;

namespace Rabbit.Go
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddGo(this IServiceCollection services)
        {
            return services
                .AddSingleton<ITemplateParser, TemplateParser>()
                .AddSingleton<IParameterExpanderLocator, ParameterExpanderLocator>();
        }
    }
}