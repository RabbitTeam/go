namespace Rabbit.Go.Core
{
    public class ToStringParameterExpander : IParameterExpander
    {
        #region Implementation of IParameterExpander

        /// <inheritdoc/>
        public string Expand(object value)
        {
            return value?.ToString();
        }

        #endregion Implementation of IParameterExpander
    }
}