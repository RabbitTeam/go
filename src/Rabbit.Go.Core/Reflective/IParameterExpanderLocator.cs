using Microsoft.Extensions.DependencyInjection;
using System;

namespace Rabbit.Go.Core.Reflective
{
    public interface IParameterExpanderLocator
    {
        IParameterExpander Get(Type type);
    }

    public class ParameterExpanderLocator : IParameterExpanderLocator
    {
        private readonly IServiceProvider _services;

        public ParameterExpanderLocator(IServiceProvider services)
        {
            _services = services;
        }

        #region Implementation of IParameterExpanderLocator

        /// <inheritdoc/>
        public IParameterExpander Get(Type type)
        {
            return (IParameterExpander)_services.GetRequiredService(type);
        }

        #endregion Implementation of IParameterExpanderLocator
    }
}