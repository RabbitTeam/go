namespace Rabbit.Go.Core
{
    public interface IParameterExpander
    {
        string Expand(object value);
    }
}