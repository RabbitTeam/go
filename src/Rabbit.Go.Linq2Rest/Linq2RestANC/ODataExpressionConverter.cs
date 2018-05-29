// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ODataExpressionConverter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Converts LINQ expressions to OData queries.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Linq.Expressions;
	using Linq2Rest.Parser;
	using Linq2Rest.Parser.Readers;
	using Linq2Rest.Provider;
	using Linq2Rest.Provider.Writers;

	/// <summary>
	/// Converts LINQ expressions to OData queries.
	/// </summary>
	public class ODataExpressionConverter
	{
		private readonly IExpressionWriter _writer;
		private readonly FilterExpressionFactory _parser;

		/// <summary>
		/// Initializes a new instance of the <see cref="ODataExpressionConverter"/> class.
		/// </summary>
		public ODataExpressionConverter()
			: this(new IValueWriter[0], new IValueExpressionFactory[0])
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ODataExpressionConverter"/> class.
		/// </summary>
		/// <param name="valueWriters">The custom value writers to use.</param>
		/// <param name="valueExpressionFactories">The custom expression writers to use.</param>
		/// <param name="memberNameResolver">The custom <see cref="IMemberNameResolver"/> to use.</param>
		public ODataExpressionConverter(IEnumerable<IValueWriter> valueWriters, IEnumerable<IValueExpressionFactory> valueExpressionFactories, IMemberNameResolver memberNameResolver = null)
		{
			var writers = (valueWriters ?? Enumerable.Empty<IValueWriter>()).ToArray();
			var expressionFactories = (valueExpressionFactories ?? Enumerable.Empty<IValueExpressionFactory>()).ToArray();
			var nameResolver = memberNameResolver ?? new MemberNameResolver();
			_writer = new ExpressionWriter(nameResolver, writers);
			_parser = new FilterExpressionFactory(nameResolver, expressionFactories);
		}

		/// <summary>
		/// Converts an expression into an OData formatted query.
		/// </summary>
		/// <param name="expression">The expression to convert.</param>
		/// <typeparam name="T">The parameter type.</typeparam>
		/// <returns>An OData <see cref="string"/> representation.</returns>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Restriction is intended.")]
		public string Convert<T>(Expression<Func<T, bool>> expression)
		{
			return _writer.Write(expression, typeof(T));
		}

		/// <summary>
		/// Converts an OData formatted query into an expression.
		/// </summary>
		/// <param name="filter">The query to convert.</param>
		/// <typeparam name="T">The parameter type.</typeparam>
		/// <returns>An expression tree for the passed query.</returns>
		public Expression<Func<T, bool>> Convert<T>(string filter)
		{
			return _parser.Create<T>(filter);
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(_writer != null);
		}
	}
}
