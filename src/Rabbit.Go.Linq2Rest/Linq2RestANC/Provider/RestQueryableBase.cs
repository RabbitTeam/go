// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestQueryableBase.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestQueryableBase type.
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

	internal class RestQueryableBase<T> : IOrderedQueryable<T>, IDisposable
	{
		public RestQueryableBase(IRestClient client, ISerializerFactory serializerFactory, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters)
		{
			CustomContract.Requires<ArgumentException>(client != null);
			CustomContract.Requires<ArgumentException>(serializerFactory != null);
			CustomContract.Requires<ArgumentException>(memberNameResolver != null);
			CustomContract.Requires<ArgumentException>(valueWriters != null);

			Client = client;
			SerializerFactory = serializerFactory;
			MemberNameResolver = memberNameResolver;
			ValueWriters = valueWriters.ToArray();
		}

		/// <summary>
		/// 	<see cref="Type"/> of T in IQueryable of T.
		/// </summary>
		public Type ElementType
		{
			get { return typeof(T); }
		}

		/// <summary>
		/// 	The expression tree.
		/// </summary>
		public Expression Expression { get; protected set; }

		/// <summary>
		/// 	IQueryProvider part of RestQueryable.
		/// </summary>
		public IQueryProvider Provider { get; protected set; }

		internal IRestClient Client { get; private set; }

		internal ISerializerFactory SerializerFactory { get; set; }

		internal IMemberNameResolver MemberNameResolver { get; private set; }

		internal IValueWriter[] ValueWriters { get; private set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IEnumerator<T> GetEnumerator()
		{
			var enumerable = Provider.Execute<IEnumerable<T>>(Expression);
			return (enumerable ?? new T[0]).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Provider.Execute<IEnumerable>(Expression).GetEnumerator();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Client.Dispose();
			}
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(Client != null);
			CustomContract.Invariant(Expression != null);
		}
	}
}