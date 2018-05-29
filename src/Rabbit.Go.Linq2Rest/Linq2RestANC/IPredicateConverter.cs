// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPredicateConverter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IPredicateConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	internal interface IPredicateConverter
	{
		Type SourceType { get; }

		Type TargetType { get; }

		IDictionary<MemberInfo, MemberInfo> Substitutions { get; }
	}
}