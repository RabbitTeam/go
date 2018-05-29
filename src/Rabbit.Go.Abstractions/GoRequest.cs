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

    /*public class GoRequest
    {
        public GoRequest(GoContext goContext)
        {
            GoContext = goContext;
            Headers = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
            Query = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
        }

        public GoContext GoContext { get; }
        public string Method { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        private string _path;

        public string Path
        {
            get => _path;
            set
            {
                _path = value;

                if (string.IsNullOrEmpty(_path))
                {
                    _path = "/";
                    return;
                }

                if (!_path.StartsWith("/"))
                    _path = "/" + _path;
            }
        }

        public IDictionary<string, StringValues> Headers { get; }
        public IDictionary<string, StringValues> Query { get; }
        public Stream Body { get; set; }
    }
    public static class GoRequestExtensions
    {
        public static GoRequest Query(this GoRequest request, string name, StringValues values)
        {
            var query = request.Query;
            query[name] = values;

            return request;
        }

        public static GoRequest Header(this GoRequest request, string name, StringValues values)
        {
            var headers = request.Headers;

            headers.Remove(name);
            headers.Add(name, values.ToArray());

            return request;
        }

        public static GoRequest AddQuery(this GoRequest request, string name, StringValues values)
        {
            var query = request.Query;
            if (query.TryGetValue(name, out var current))
                values = StringValues.Concat(current, values);

            query[name] = values;

            return request;
        }

        public static GoRequest AddHeader(this GoRequest request, string name, StringValues values)
        {
            var headers = request.Headers;

            headers.Add(name, values.ToArray());

            return request;
        }

        public static GoRequest Body(this GoRequest request, byte[] data, string contentType = "application/json")
        {
            request.Body = new MemoryStream(data);
            return request.Header("Content-Type", contentType);
        }

        public static GoRequest Body(this GoRequest request, string content, string contentType = "text/plain")
        {
            return request.Body(Encoding.UTF8.GetBytes(content), contentType);
        }
    }*/
}