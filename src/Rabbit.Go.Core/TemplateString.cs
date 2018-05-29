using Rabbit.Go.Utilities;
using System.Linq;

namespace Rabbit.Go
{
    public struct TemplateString
    {
        public TemplateString(string template)
        {
            Template = template;
            Variables = TemplateUtilities.GetVariables(template);
            NeedParse = Variables != null && Variables.Any();
        }

        public string Template { get; }
        public string[] Variables { get; }

        public bool NeedParse { get; }
    }
}