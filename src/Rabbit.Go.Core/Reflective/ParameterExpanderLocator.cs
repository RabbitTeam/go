using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace Rabbit.Go.Core.Reflective
{
    public class ParameterExpanderLocator : IParameterExpanderLocator
    {
        private readonly IServiceProvider _services;

        private readonly ConcurrentDictionary<Type, IParameterExpander> _cacheds = new ConcurrentDictionary<Type, IParameterExpander>();

        public ParameterExpanderLocator(IServiceProvider services)
        {
            _services = services;
        }

        #region Implementation of IParameterExpanderLocator

        /// <inheritdoc/>
        public IParameterExpander Get(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!typeof(IParameterExpander).IsAssignableFrom(type))
                throw new ArgumentException($"{type.FullName} is not {typeof(IParameterExpander).FullName} type.", nameof(type));

            if (_services.GetService(type) is IParameterExpander expander)
                return expander;

            return _cacheds.GetOrAdd(type,
                key => (IParameterExpander)ActivatorUtilities.CreateInstance(_services, type));
        }

        #endregion Implementation of IParameterExpanderLocator
    }
}