// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestDeleteQueryProvider.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestDeleteQueryProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Linq.Expressions;
	using Linq2Rest.Provider.Writers;

	internal class RestDeleteQueryProvider<T> : RestQueryProvider<T>
	{
		public RestDeleteQueryProvider(IRestClient client, ISerializerFactory serializerFactory, IExpressionProcessor expressionProcessor, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters, Type sourceType)
			: base(client, serializerFactory, expressionProcessor, memberNameResolver, valueWriters, sourceType)
		{
			CustomContract.Requires(client != null);
			CustomContract.Requires(serializerFactory != null);
			CustomContract.Requires(expressionProcessor != null);
			CustomContract.Requires(valueWriters != null);
		}

		protected override Func<IRestClient, ISerializerFactory, IMemberNameResolver, IEnumerable<IValueWriter>, Expression, Type, IQueryable<TResult>> CreateQueryable<TResult>()
		{
			return InnerCreateQueryable<TResult>;
		}

		protected override IEnumerable<T> GetResults(ParameterBuilder builder)
		{
			var fullUri = builder.GetFullUri();
			var response = Client.Delete(fullUri);
			var serializer = GetSerializer(builder.SourceType);
			var resultSet = serializer.DeserializeList(response);

			CustomContract.Assume(resultSet != null);

			return resultSet;
		}

		protected override IEnumerable GetIntermediateResults(Type type, ParameterBuilder builder)
		{
			var fullUri = builder.GetFullUri();
			var response = Client.Delete(fullUri);

			dynamic serializer = GetSerializer(type, builder.SourceType);
			var resultSet = serializer.DeserializeList(response);

			return resultSet;
		}

		private IQueryable<TResult> InnerCreateQueryable<TResult>(IRestClient client, ISerializerFactory serializerFactory, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters, Expression expression, Type sourceType)
		{
			CustomContract.Requires(client != null);
			CustomContract.Requires(serializerFactory != null);
			CustomContract.Requires(memberNameResolver != null);
			CustomContract.Requires(valueWriters != null);
			CustomContract.Requires(expression != null);
			CustomContract.Requires(sourceType != null);

			return new RestDeleteQueryable<TResult>(client, serializerFactory, memberNameResolver, valueWriters, expression, sourceType);
		}
	}
}