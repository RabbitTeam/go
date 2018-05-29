using Rabbit.Go.Abstractions.Features;
using Rabbit.Go.Features;
using System;
using System.IO;

namespace Rabbit.Go
{
    public abstract class GoRequest
    {
        public abstract GoContext GoContext { get; }
        public abstract string Method { get; set; }
        public abstract string Scheme { get; set; }
        public abstract string Host { get; set; }
        public abstract string Path { get; set; }
        public abstract QueryString QueryString { get; set; }
        public abstract IQueryCollection Query { get; set; }
        public abstract IHeaderDictionary Headers { get; }
        public abstract int? Port { get; set; }
        public abstract Stream Body { get; set; }
    }

    public class DefaultGoRequest : GoRequest
    {
        private static readonly Func<IFeatureCollection, IGoRequestFeature> _nullRequestFeature = f => null;
        private static readonly Func<IFeatureCollection, IQueryFeature> _newQueryFeature = f => new QueryFeature(f);
        private GoContext _context;
        private FeatureReferences<FeatureInterfaces> _features;

        public DefaultGoRequest(GoContext context)
        {
            Initialize(context);
        }

        #region Public Method

        public virtual void Initialize(GoContext context)
        {
            _context = context;
            _features = new FeatureReferences<FeatureInterfaces>(context.Features);
        }

        public virtual void Uninitialize()
        {
            _context = null;
            _features = default(FeatureReferences<FeatureInterfaces>);
        }

        #endregion Public Method

        #region Property

        private IGoRequestFeature GoRequestFeature =>
            _features.Fetch(ref _features.Cache.Request, _nullRequestFeature);

        private IQueryFeature QueryFeature =>
            _features.Fetch(ref _features.Cache.Query, _newQueryFeature);

        #endregion Property

        #region Overrides of GoRequest

        /// <inheritdoc/>
        public override GoContext GoContext => _context;

        /// <inheritdoc/>
        public override string Method
        {
            get => GoRequestFeature.Method;
            set => GoRequestFeature.Method = value;
        }

        /// <inheritdoc/>
        public override string Scheme
        {
            get => GoRequestFeature.Scheme;
            set => GoRequestFeature.Scheme = value;
        }

        /// <inheritdoc/>
        public override string Host
        {
            get => GoRequestFeature.Host;
            set => GoRequestFeature.Host = value;
        }

        /// <inheritdoc/>
        public override string Path
        {
            get => GoRequestFeature.Path;
            set => GoRequestFeature.Path = value;
        }

        /// <inheritdoc/>
        public override QueryString QueryString
        {
            get => new QueryString(GoRequestFeature.QueryString);
            set => GoRequestFeature.QueryString = value.Value;
        }

        /// <inheritdoc/>
        public override IQueryCollection Query
        {
            get => QueryFeature.Query;
            set => QueryFeature.Query = value;
        }

        /// <inheritdoc/>
        public override IHeaderDictionary Headers => GoRequestFeature.Headers;

        /// <inheritdoc/>
        public override int? Port
        {
            get => GoRequestFeature.Port;
            set => GoRequestFeature.Port = value;
        }

        /// <inheritdoc/>
        public override Stream Body
        {
            get => GoRequestFeature.Body;
            set => GoRequestFeature.Body = value;
        }

        #endregion Overrides of GoRequest

        private struct FeatureInterfaces
        {
            public IGoRequestFeature Request;
            public IQueryFeature Query;
        }
    }
}