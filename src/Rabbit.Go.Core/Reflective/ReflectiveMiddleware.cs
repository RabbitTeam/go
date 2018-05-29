using Rabbit.Go.Core;
using Rabbit.Go.Core.Reflective;
using Rabbit.Go.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rabbit.Go.Http
{
    public class ReflectiveMiddleware
    {
        private readonly GoRequestDelegate _next;
        private readonly ITemplateParser _templateParser;
        private readonly IParameterExpanderLocator _parameterExpanderLocator;

        public ReflectiveMiddleware(GoRequestDelegate next, ITemplateParser templateParser, IParameterExpanderLocator parameterExpanderLocator)
        {
            _next = next;
            _templateParser = templateParser;
            _parameterExpanderLocator = parameterExpanderLocator;
        }

        private IDictionary<string, string> GetTemplateArguments(IReflectiveFeature reflectiveFeature)
        {
            var methodDescriptor = reflectiveFeature.MethodDescriptor;
            var arguments = reflectiveFeature.Arguments;

            var templateVariables = new Dictionary<string, string>(methodDescriptor.Parameters.Length, StringComparer.OrdinalIgnoreCase);

            foreach (var parameterDescriptor in methodDescriptor.Parameters)
            {
                var name = parameterDescriptor.Name ?? parameterDescriptor.ParameterName;

                var goParameterAttribute = parameterDescriptor.Attributes.OfType<GoParameterAttribute>().FirstOrDefault();
                if (goParameterAttribute == null)
                    continue;
                var value = arguments[parameterDescriptor.ParameterName];
                templateVariables[name] = GetParameterExpander(goParameterAttribute.Expander).Expand(value);
            }

            return templateVariables;
        }

        private void SetRequestLine(GoRequest request, MethodDescriptor methodDescriptor, IDictionary<string, string> templateArguments)
        {
            var requestLineTemplate = methodDescriptor.RequestLine;

            var requestLine =
                requestLineTemplate.NeedParse ?
                _templateParser.Parse(requestLineTemplate.Template, templateArguments)
                : requestLineTemplate.Template;

            var querStartIndex = requestLine.IndexOf('?');

            if (querStartIndex == -1)
                request.Path = requestLine;
            else
            {
                request.Path = requestLine.Substring(0, querStartIndex);
                request.QueryString = new QueryString(requestLine.Substring(querStartIndex));
            }
        }

        private void SetHeaders(GoRequest request, MethodDescriptor methodDescriptor,
            IDictionary<string, string> templateArguments)
        {
            foreach (var headerItem in methodDescriptor.Headers)
            {
                var name = headerItem.Key;
                var headerTemplate = headerItem.Value;
                var value = headerTemplate.NeedParse
                    ? _templateParser.Parse(headerTemplate.Template, templateArguments)
                    : headerTemplate.Template;

                request.Headers[name] = value;
            }
        }

        public async Task Invoke(GoContext context)
        {
            var reflectiveFeature = context.Features.Get<IReflectiveFeature>();

            var methodDescriptor = reflectiveFeature.MethodDescriptor;
            var templateArguments = GetTemplateArguments(reflectiveFeature);

            var request = context.Request;

            request.Method = methodDescriptor.RequestMethod;

            SetRequestLine(request, methodDescriptor, templateArguments);
            SetHeaders(request, methodDescriptor, templateArguments);

            var bodyParameterDescriptor = methodDescriptor.Parameters.SingleOrDefault(i => i.Attributes.OfType<GoBodyAttribute>().Any());

            if (bodyParameterDescriptor != null)
            {
                var body = reflectiveFeature.Arguments[bodyParameterDescriptor.ParameterName];
                var goFeature = context.Features.Get<IGoFeature>();
                goFeature.RequestInstance = body;
                goFeature.RequestType = bodyParameterDescriptor.ParameterType;
            }

            await _next(context);
        }

        private IParameterExpander GetParameterExpander(Type expanderType)
        {
            return _parameterExpanderLocator.Get(expanderType);
        }
    }
}