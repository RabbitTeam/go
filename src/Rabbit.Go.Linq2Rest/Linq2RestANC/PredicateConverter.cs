// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredicateConverter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PredicateConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	internal class PredicateConverter<TSource, TResult> : IPredicateConverter
	{
		private readonly IDictionary<MemberInfo, MemberInfo> _substitutions = new Dictionary<MemberInfo, MemberInfo>();

		public Type SourceType
		{
			get
			{
				return typeof(TSource);
			}
		}

		public Type TargetType
		{
			get
			{
				return typeof(TResult);
			}
		}

		public IDictionary<MemberInfo, MemberInfo> Substitutions
		{
			get
			{
				return _substitutions;
			}
		}
	}
}