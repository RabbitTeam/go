// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestContext.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestContext.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using Linq2Rest.Provider.Writers;

	/// <summary>
	/// Defines the RestContext.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> of object to query.</typeparam>
	public class RestContext<T> : IDisposable
	{
		private readonly RestGetQueryable<T> _getQueryable;

		/// <summary>
		/// Initializes a new instance of the <see cref="RestContext{T}"/> class.
		/// </summary>
		/// <param name="client">The <see cref="IRestClient"/> to use for requests.</param>
		/// <param name="serializerFactory">The <see cref="ISerializerFactory"/> to create <see cref="ISerializer{T}"/> to handling responses.</param>
		public RestContext(IRestClient client, ISerializerFactory serializerFactory)
			: this(client, serializerFactory, new MemberNameResolver(), new IntValueWriter[0])
		{
			CustomContract.Requires<ArgumentNullException>(client != null);
			CustomContract.Requires<ArgumentNullException>(serializerFactory != null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RestContext{T}"/> class.
		/// </summary>
		/// <param name="client">The <see cref="IRestClient"/> to use for requests.</param>
		/// <param name="serializerFactory">The <see cref="ISerializerFactory"/> to create <see cref="ISerializer{T}"/> to handling responses.</param>
		/// <param name="valueWriters">The <see cref="IEnumerable{IValueWriter}"/> for writing custom values.</param>
		public RestContext(IRestClient client, ISerializerFactory serializerFactory, IEnumerable<IValueWriter> valueWriters)
			: this(client, serializerFactory, new MemberNameResolver(), valueWriters)
		{
			CustomContract.Requires<ArgumentNullException>(client != null);
			CustomContract.Requires<ArgumentNullException>(serializerFactory != null);
			CustomContract.Requires<ArgumentNullException>(valueWriters != null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RestContext{T}"/> class.
		/// </summary>
		/// <param name="client">The <see cref="IRestClient"/> to use for requests.</param>
		/// <param name="serializerFactory">The <see cref="ISerializerFactory"/> to create <see cref="ISerializer{T}"/> to handling responses.</param>
		/// <param name="memberNameResolver">The <see cref="IMemberNameResolver"/> to use for alias resolution.</param>
		/// <param name="valueWriters">The <see cref="IEnumerable{IValueWriter}"/> for writing custom values.</param>
		public RestContext(IRestClient client, ISerializerFactory serializerFactory, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters)
		{
			CustomContract.Requires<ArgumentNullException>(client != null);
			CustomContract.Requires<ArgumentNullException>(serializerFactory != null);
			CustomContract.Requires<ArgumentNullException>(memberNameResolver != null);
			CustomContract.Requires<ArgumentNullException>(valueWriters != null);

			_getQueryable = new RestGetQueryable<T>(client, serializerFactory, memberNameResolver, valueWriters, typeof(T));
		}

		/// <summary>
		/// Gets the context query.
		/// </summary>
		public IQueryable<T> Query
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<IQueryable<T>>() != null);

				return _getQueryable;
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				_getQueryable.Dispose();
			}
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(_getQueryable != null);
		}
	}
}