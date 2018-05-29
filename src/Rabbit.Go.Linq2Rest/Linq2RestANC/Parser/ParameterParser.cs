// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterParser.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the default implementation of a parameter parser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using Linq2Rest.Parser.Readers;

	/// <summary>
	/// Defines the default implementation of a parameter parser.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> of item to create parser for.</typeparam>
	public class ParameterParser<T> : IParameterParser<T>
	{
		private readonly IFilterExpressionFactory _filterExpressionFactory;
		private readonly ISelectExpressionFactory<T> _selectExpressionFactory;
		private readonly ISortExpressionFactory _sortExpressionFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterParser{T}"/> class.
		/// </summary>
		public ParameterParser()
			: this(new MemberNameResolver())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterParser{T}"/> class.
		/// </summary>
		/// <param name="memberNameResolver">The <see cref="IMemberNameResolver"/> to use for name resolution.</param>
		public ParameterParser(IMemberNameResolver memberNameResolver)
			: this(new FilterExpressionFactory(memberNameResolver, Enumerable.Empty<IValueExpressionFactory>()), new SortExpressionFactory(memberNameResolver), new SelectExpressionFactory<T>(memberNameResolver, new RuntimeTypeProvider(memberNameResolver)))
		{
			CustomContract.Requires<ArgumentNullException>(memberNameResolver != null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterParser{T}"/> class.
		/// </summary>
		/// <param name="memberNameResolver">The <see cref="IMemberNameResolver"/> to use for name resolution.</param>
		/// <param name="valueExpressionFactories">The custom <see cref="IValueExpressionFactory"/> to use for value conversion.</param>
		public ParameterParser(IMemberNameResolver memberNameResolver, IEnumerable<IValueExpressionFactory> valueExpressionFactories)
			: this(new FilterExpressionFactory(memberNameResolver, valueExpressionFactories), new SortExpressionFactory(memberNameResolver), new SelectExpressionFactory<T>(memberNameResolver, new RuntimeTypeProvider(memberNameResolver)))
		{
			CustomContract.Requires<ArgumentNullException>(memberNameResolver != null);
			CustomContract.Requires<ArgumentNullException>(valueExpressionFactories != null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterParser{T}"/> class.
		/// </summary>
		/// <param name="filterExpressionFactory">The <see cref="IFilterExpressionFactory"/> to use.</param>
		/// <param name="sortExpressionFactory">The <see cref="ISortExpressionFactory"/> to use.</param>
		/// <param name="selectExpressionFactory">The <see cref="ISelectExpressionFactory{T}"/> to use.</param>
		public ParameterParser(
			IFilterExpressionFactory filterExpressionFactory,
			ISortExpressionFactory sortExpressionFactory,
			ISelectExpressionFactory<T> selectExpressionFactory)
		{
			CustomContract.Requires<ArgumentNullException>(filterExpressionFactory != null);
			CustomContract.Requires<ArgumentNullException>(sortExpressionFactory != null);
			CustomContract.Requires<ArgumentNullException>(selectExpressionFactory != null);

			_filterExpressionFactory = filterExpressionFactory;
			_sortExpressionFactory = sortExpressionFactory;
			_selectExpressionFactory = selectExpressionFactory;
		}

		/// <summary>
		/// Parses the passes query parameters to a <see cref="ModelFilter{T}"/>.
		/// </summary>
		/// <param name="queryParameters"></param>
		/// <returns></returns>
		public IModelFilter<T> Parse(NameValueCollection queryParameters)
		{
			var orderbyField = queryParameters[StringConstants.OrderByParameter];
			var selects = queryParameters[StringConstants.SelectParameter];
			var filter = queryParameters[StringConstants.FilterParameter];
			var skip = queryParameters[StringConstants.SkipParameter];
			var top = queryParameters[StringConstants.TopParameter];

			var filterExpression = _filterExpressionFactory.Create<T>(filter);
			var sortDescriptions = _sortExpressionFactory.Create<T>(orderbyField);
			var selectFunction = _selectExpressionFactory.Create(selects);

			var modelFilter = new ModelFilter<T>(
				filterExpression,
				selectFunction,
				sortDescriptions,
				string.IsNullOrWhiteSpace(skip) ? -1 : Convert.ToInt32(skip),
				string.IsNullOrWhiteSpace(top) ? -1 : Convert.ToInt32(top));
			return modelFilter;
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(_filterExpressionFactory != null);
			CustomContract.Invariant(_sortExpressionFactory != null);
			CustomContract.Invariant(_selectExpressionFactory != null);
		}
	}
}