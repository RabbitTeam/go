using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Rabbit.Go.Core.Reflective
{
    public class ParameterExpanderLocator : IParameterExpanderLocator
    {
        private readonly IServiceProvider _services;

        private readonly Dictionary<Type, IParameterExpander> _cacheds = new Dictionary<Type, IParameterExpander>();

        public ParameterExpanderLocator(IServiceProvider services)
        {
            _services = services;
        }

        #region Implementation of IParameterExpanderLocator

        /// <inheritdoc/>
        public IParameterExpander Get(Type type)
        {
            if (!typeof(IParameterExpander).IsAssignableFrom(type))
                throw new ArgumentException($"{type.FullName} is not {typeof(IParameterExpander).FullName} type.", nameof(type));

            if (_services.GetService(type) is IParameterExpander expander)
                return expander;

            if (_cacheds.TryGetValue(type, out expander))
                return expander;

            return _cacheds[type] = (IParameterExpander)ActivatorUtilities.CreateInstance(_services, type);
        }

        #endregion Implementation of IParameterExpanderLocator
    }
}