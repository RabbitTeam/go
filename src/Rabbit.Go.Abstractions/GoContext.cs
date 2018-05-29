using Rabbit.Go.Abstractions.Features;
using Rabbit.Go.Features;
using System;
using System.Collections.Generic;

namespace Rabbit.Go
{
    public abstract class GoContext
    {
        public abstract IFeatureCollection Features { get; }
        public abstract GoRequest Request { get; }
        public abstract GoResponse Response { get; }
        public abstract IDictionary<object, object> Items { get; set; }
        public abstract IServiceProvider RequestServices { get; set; }
    }

    public class DefaultGoContext : GoContext
    {
        private static readonly Func<IFeatureCollection, IItemsFeature> _newItemsFeature = f => new ItemsFeature();
        private static readonly Func<IFeatureCollection, IServiceProvidersFeature> _newServiceProvidersFeature = f => new ServiceProvidersFeature();

        private FeatureReferences<FeatureInterfaces> _features;

        private GoRequest _request;
        private GoResponse _response;

        public DefaultGoContext(IFeatureCollection features)
        {
            Initialize(features);
        }

        public virtual void Initialize(IFeatureCollection features)
        {
            _features = new FeatureReferences<FeatureInterfaces>(features);
            _request = InitializeGoRequest();
            _response = InitializeGoResponse();
        }

        public virtual void Uninitialize()
        {
            _features = default(FeatureReferences<FeatureInterfaces>);
            if (_request != null)
            {
                UninitializeGoRequest(_request);
                _request = null;
            }
            if (_response != null)
            {
                UninitializeGoResponse(_response);
                _response = null;
            }
        }

        #region Property

        private IItemsFeature ItemsFeature =>
            _features.Fetch(ref _features.Cache.Items, _newItemsFeature);

        private IServiceProvidersFeature ServiceProvidersFeature =>
            _features.Fetch(ref _features.Cache.ServiceProviders, _newServiceProvidersFeature);

        #endregion Property

        #region Overrides of GoContext

        /// <inheritdoc/>
        public override IFeatureCollection Features => _features.Collection;

        /// <inheritdoc/>
        public override GoRequest Request => _request;

        /// <inheritdoc/>
        public override GoResponse Response => _response;

        /// <inheritdoc/>
        public override IDictionary<object, object> Items
        {
            get => ItemsFeature.Items;
            set => ItemsFeature.Items = value;
        }

        /// <inheritdoc/>
        public override IServiceProvider RequestServices
        {
            get => ServiceProvidersFeature.RequestServices;
            set => ServiceProvidersFeature.RequestServices = value;
        }

        #endregion Overrides of GoContext

        #region Protected Method

        protected virtual GoRequest InitializeGoRequest() => new DefaultGoRequest(this);

        protected virtual void UninitializeGoRequest(GoRequest instance)
        {
        }

        protected virtual GoResponse InitializeGoResponse() => new DefaultGoResponse(this);

        protected virtual void UninitializeGoResponse(GoResponse instance)
        {
        }

        #endregion Protected Method

        private struct FeatureInterfaces
        {
            public IItemsFeature Items;
            public IServiceProvidersFeature ServiceProviders;
        }
    }
}