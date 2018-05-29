// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestGetQueryable.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestGetQueryable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq.Expressions;
	using Linq2Rest.Provider.Writers;

	internal class RestGetQueryable<T> : RestQueryableBase<T>
	{
		private readonly RestGetQueryProvider<T> _restGetQueryProvider;

		public RestGetQueryable(IRestClient client, ISerializerFactory serializerFactory, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters, Type sourceType)
			: base(client, serializerFactory, memberNameResolver, valueWriters)
		{
			CustomContract.Requires<ArgumentNullException>(client != null);
			CustomContract.Requires<ArgumentNullException>(serializerFactory != null);
			CustomContract.Requires<ArgumentNullException>(memberNameResolver != null);
			CustomContract.Requires<ArgumentNullException>(valueWriters != null);

			_restGetQueryProvider = new RestGetQueryProvider<T>(client, serializerFactory, new ExpressionProcessor(new ExpressionWriter(MemberNameResolver, ValueWriters), MemberNameResolver), MemberNameResolver, ValueWriters, sourceType);
			Provider = _restGetQueryProvider;
			Expression = Expression.Constant(this);
		}

		public RestGetQueryable(IRestClient client, ISerializerFactory serializerFactory, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters, Type sourceType, Expression expression)
			: base(client, serializerFactory, memberNameResolver, valueWriters)
		{
			CustomContract.Requires<ArgumentNullException>(client != null);
			CustomContract.Requires<ArgumentNullException>(serializerFactory != null);
			CustomContract.Requires<ArgumentNullException>(memberNameResolver != null);
			CustomContract.Requires<ArgumentNullException>(valueWriters != null);
			CustomContract.Requires<ArgumentNullException>(expression != null);

			_restGetQueryProvider = new RestGetQueryProvider<T>(client, serializerFactory, new ExpressionProcessor(new ExpressionWriter(MemberNameResolver, ValueWriters), MemberNameResolver), MemberNameResolver, ValueWriters, sourceType);
			Provider = _restGetQueryProvider;
			Expression = expression;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					_restGetQueryProvider.Dispose();
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
			CustomContract.Invariant(_restGetQueryProvider != null);
		}
	}
}