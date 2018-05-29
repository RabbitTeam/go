// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestDeleteQueryable.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestDeleteQueryable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;
	using Linq2Rest.Provider.Writers;

	internal class RestDeleteQueryable<T> : RestQueryableBase<T>
	{
		private readonly RestDeleteQueryProvider<T> _restDeleteQueryProvider;

		public RestDeleteQueryable(IRestClient client, ISerializerFactory serializerFactory, IEnumerable<IValueWriter> valueWriters, Type sourceType)
			: this(client, serializerFactory, new MemberNameResolver(), valueWriters, sourceType)
		{
			CustomContract.Requires(client != null);
			CustomContract.Requires(serializerFactory != null);
			CustomContract.Requires(valueWriters != null);
		}

		public RestDeleteQueryable(IRestClient client, ISerializerFactory serializerFactory, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters, Type sourceType)
			: base(client, serializerFactory, memberNameResolver, valueWriters)
		{
			CustomContract.Requires(client != null);
			CustomContract.Requires(serializerFactory != null);
			CustomContract.Requires(memberNameResolver != null);
			CustomContract.Requires(valueWriters != null);

			_restDeleteQueryProvider = new RestDeleteQueryProvider<T>(client, serializerFactory, new ExpressionProcessor(new ExpressionWriter(MemberNameResolver, ValueWriters), MemberNameResolver), MemberNameResolver, ValueWriters, sourceType);
			Provider = _restDeleteQueryProvider;
			Expression = Expression.Constant(this);
		}

		public RestDeleteQueryable(IRestClient client, ISerializerFactory serializerFactory, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters, Expression expression, Type sourceType)
			: this(client, serializerFactory, memberNameResolver, valueWriters, sourceType)
		{
			CustomContract.Requires(client != null);
			CustomContract.Requires(serializerFactory != null);
			CustomContract.Requires(memberNameResolver != null);
			CustomContract.Requires(valueWriters != null);
			CustomContract.Requires(expression != null);

			Expression = expression;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					_restDeleteQueryProvider.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(_restDeleteQueryProvider != null);
		}
	}
}