using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Rabbit.Go.Core.Reflective
{
    public class GoInterceptor : IInterceptor
    {
        private readonly Func<GoContext> _goContextFactory;
        private readonly GoRequestDelegate _invoker;
        private readonly IMethodDescriptorTable _methodDescriptorTable;

        public GoInterceptor(Func<GoContext> goContextFactory, GoRequestDelegate invoker, IMethodDescriptorTable methodDescriptorTable)
        {
            _goContextFactory = goContextFactory;
            _invoker = invoker;
            _methodDescriptorTable = methodDescriptorTable;
        }

        #region Implementation of IInterceptor

        /// <inheritdoc/>
        public void Intercept(IInvocation invocation)
        {
            var returnType = invocation.Method.ReturnType;
            if (typeof(Task).IsAssignableFrom(returnType))
            {
                if (returnType.IsGenericType)
                {
                    var method = typeof(GoInterceptor).GetMethod(nameof(HandleAsync), BindingFlags.NonPublic | BindingFlags.Instance)
                        .MakeGenericMethod(invocation.Method.ReturnType.GenericTypeArguments[0]);
                    invocation.ReturnValue = method.Invoke(this, new object[] { invocation });
                }
                else
                {
                    invocation.ReturnValue = HandleTaskAsync(invocation);
                }
            }
            else
            {
                invocation.ReturnValue = Handle(invocation);
            }
        }

        #endregion Implementation of IInterceptor

        private async Task HandleTaskAsync(IInvocation invocation)
        {
            await DoHandleAsync(invocation);
        }

        private async Task<T> HandleAsync<T>(IInvocation invocation)
        {
            var value = await DoHandleAsync(invocation);

            switch (value)
            {
                case null:
                    return default(T);

                case Task<T> task:
                    return await task;
            }

            return (T)value;
        }

        private object Handle(IInvocation invocation)
        {
            return DoHandleAsync(invocation).GetAwaiter().GetResult();
        }

        private async Task<object> DoHandleAsync(IInvocation invocation)
        {
            var arguments = invocation.Arguments;
            var method = invocation.Method;
            var returnType = method.ReturnType;
            var realReturnType = returnType.IsGenericType ? returnType.GenericTypeArguments[0] : null;

            var goContext = _goContextFactory();

            var goFeature = goContext.Features.Get<IGoFeature>();
            var reflectiveFeature = goContext.Features.Get<IReflectiveFeature>();

            reflectiveFeature.MethodDescriptor = _methodDescriptorTable.Get(method);

            var dict = new Dictionary<string, object>();
            var parameters = method.GetParameters();
            for (int j = 0; j < parameters.Length; j++)
            {
                dict[parameters[j].Name] = arguments[j];
            }

            reflectiveFeature.Arguments = dict;
            // reflectiveFeature.MethodDescriptor = _methodDescriptors.SingleOrDefault(i =>
            // i.RequestMethod == "GET"); reflectiveFeature.Arguments = new Dictionary<string,
            // object> { { "appId", appId } };
            goFeature.ResponseType = realReturnType;

            await _invoker(goContext);
            return goFeature.ResponseInstance;
        }
    }
}