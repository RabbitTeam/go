// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestQueryProviderBase.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestQueryProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	internal abstract class RestQueryProviderBase : IQueryProvider, IDisposable
	{
		protected readonly static MethodInfo CreateMethodInfo = typeof(ISerializerFactory).GetMethods().First(x => x.Name == "Create" && x.GetGenericArguments().Length == 1).GetGenericMethodDefinition();
		protected readonly static MethodInfo AliasCreateMethodInfo = typeof(ISerializerFactory).GetMethods().First(x => x.Name == "Create" && x.GetGenericArguments().Length == 2).GetGenericMethodDefinition();
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public abstract IQueryable CreateQuery(Expression expression);

		public abstract IQueryable<TElement> CreateQuery<TElement>(Expression expression);

		public abstract object Execute(Expression expression);

		public TResult Execute<TResult>(Expression expression)
		{
			CustomContract.Assume(expression != null);
			return (TResult)Execute(expression);
		}

		protected abstract void Dispose(bool disposing);
	}
}