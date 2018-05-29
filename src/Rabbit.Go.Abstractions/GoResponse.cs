using Rabbit.Go.Features;
using System;
using System.IO;

namespace Rabbit.Go
{
    public abstract class GoResponse
    {
        public abstract GoContext GoContext { get; }
        public abstract Stream Body { get; set; }
        public abstract IHeaderDictionary Headers { get; }
        public abstract int StatusCode { get; set; }
    }

    public class DefaultGoResponse : GoResponse
    {
        // Lambdas hoisted to static readonly fields to improve inlining https://github.com/dotnet/roslyn/issues/13624
        private static readonly Func<IFeatureCollection, IGoResponseFeature> _nullResponseFeature = f => null;

        private GoContext _context;
        private FeatureReferences<FeatureInterfaces> _features;

        public DefaultGoResponse(GoContext context)
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

        private IGoResponseFeature GoResponseFeature =>
            _features.Fetch(ref _features.Cache.Response, _nullResponseFeature);

        #endregion Property

        #region Overrides of GoResponse

        /// <inheritdoc/>
        public override GoContext GoContext => _context;

        /// <inheritdoc/>
        public override Stream Body
        {
            get => GoResponseFeature.Body;
            set => GoResponseFeature.Body = value;
        }

        /// <inheritdoc/>
        public override IHeaderDictionary Headers => GoResponseFeature.Headers;

        /// <inheritdoc/>
        public override int StatusCode
        {
            get => GoResponseFeature.StatusCode;
            set => GoResponseFeature.StatusCode = value;
        }

        #endregion Overrides of GoResponse

        private struct FeatureInterfaces
        {
            public IGoResponseFeature Response;
        }
    }
}