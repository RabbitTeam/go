using System.Collections.Generic;

namespace Rabbit.Go.Core.Reflective
{
    public interface IReflectiveFeature
    {
        MethodDescriptor MethodDescriptor { get; set; }
        IDictionary<string, object> Arguments { get; set; }
    }

    public class ReflectiveFeature : IReflectiveFeature
    {
        #region Implementation of IReflectiveFeature

        /// <inheritdoc/>
        public MethodDescriptor MethodDescriptor { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, object> Arguments { get; set; }

        #endregion Implementation of IReflectiveFeature
    }
}