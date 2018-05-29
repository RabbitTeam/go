// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestQueryProvider.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestQueryProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Linq.Expressions;
	using Linq2Rest.Provider.Writers;

	[ContractClass(typeof(RestQueryProviderContracts<>))]
	internal abstract class RestQueryProvider<T> : RestQueryProviderBase
	{
		private readonly ISerializerFactory _serializerFactory;
		private readonly IExpressionProcessor _expressionProcessor;
		private readonly IMemberNameResolver _memberNameResolver;
		private readonly IEnumerable<IValueWriter> _valueWriters;
		private readonly ParameterBuilder _parameterBuilder;

		public RestQueryProvider(IRestClient client, ISerializerFactory serializerFactory, IExpressionProcessor expressionProcessor, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters, Type sourceType)
		{
			CustomContract.Requires(client != null);
			CustomContract.Requires(serializerFactory != null);
			CustomContract.Requires(expressionProcessor != null);
			CustomContract.Requires(valueWriters != null);

			Client = client;
			_serializerFactory = serializerFactory;
			_expressionProcessor = expressionProcessor;
			_memberNameResolver = memberNameResolver;
			_valueWriters = valueWriters;
			_parameterBuilder = new ParameterBuilder(client.ServiceBase, sourceType ?? typeof(T));
		}

		protected IRestClient Client { get; private set; }

		protected abstract Func<IRestClient, ISerializerFactory, IMemberNameResolver, IEnumerable<IValueWriter>, Expression, Type, IQueryable<TResult>> CreateQueryable<TResult>();

		protected ISerializer<T> GetSerializer(Type aliasType)
		{
			if (aliasType == null)
			{
				return _serializerFactory.Create<T>();
			}

			var method = AliasCreateMethodInfo.MakeGenericMethod(typeof(T), aliasType);

			return (ISerializer<T>)method.Invoke(_serializerFactory, null);
		}

		protected object GetSerializer(Type itemType, Type aliasType)
		{
			if (aliasType == null)
			{
				var method = CreateMethodInfo.MakeGenericMethod(itemType);

				return method.Invoke(_serializerFactory, null);
			}

			var aliasMethod = AliasCreateMethodInfo.MakeGenericMethod(itemType, aliasType);

			return aliasMethod.Invoke(_serializerFactory, null);
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot dispose here.")]
		public override IQueryable CreateQuery(Expression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}

			return CreateQueryable<T>()(Client, _serializerFactory, _memberNameResolver, _valueWriters, expression, _parameterBuilder.SourceType);
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot dispose here.")]
		public override IQueryable<TResult> CreateQuery<TResult>(Expression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}

			return CreateQueryable<TResult>()(Client, _serializerFactory, _memberNameResolver, _valueWriters, expression, _parameterBuilder.SourceType); // new RestGetQueryable<TResult>(Client, _serializerFactory, expression);
		}

		public override object Execute(Expression expression)
		{
			CustomContract.Assume(expression != null);

			var methodCallExpression = expression as MethodCallExpression;
			var resultsLoaded = false;
			Func<ParameterBuilder, IEnumerable<T>> loadFunc = p =>
																  {
																	  resultsLoaded = true;
																	  return GetResults(p);
																  };
			return (methodCallExpression != null
						? _expressionProcessor.ProcessMethodCall(methodCallExpression, _parameterBuilder, loadFunc, GetIntermediateResults)
						: GetResults(_parameterBuilder))
				   ?? (resultsLoaded
						? null
						: GetResults(_parameterBuilder));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Client.Dispose();
			}
		}

		protected abstract IEnumerable<T> GetResults(ParameterBuilder builder);

		protected abstract IEnumerable GetIntermediateResults(Type type, ParameterBuilder builder);

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(Client != null);
			CustomContract.Invariant(_serializerFactory != null);
			CustomContract.Invariant(_expressionProcessor != null);
			CustomContract.Invariant(_parameterBuilder != null);
		}
	}

	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Keeping contracts with class.")]
	[ContractClassFor(typeof(RestQueryProvider<>))]
	internal abstract class RestQueryProviderContracts<T> : RestQueryProvider<T>
	{
		protected RestQueryProviderContracts(IRestClient client, ISerializerFactory serializerFactory, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters, IExpressionProcessor expressionProcessor, Type sourceType)
			: base(client, serializerFactory, expressionProcessor, memberNameResolver, valueWriters, sourceType)
		{
		}

		protected override IEnumerable<T> GetResults(ParameterBuilder builder)
		{
			CustomContract.Requires(builder != null);
			CustomContract.Ensures(CustomContract.Result<IEnumerable<T>>() != null);

			throw new NotImplementedException();
		}

		protected override IEnumerable GetIntermediateResults(Type type, ParameterBuilder builder)
		{
			CustomContract.Requires(builder != null);

			throw new NotImplementedException();
		}
	}
}