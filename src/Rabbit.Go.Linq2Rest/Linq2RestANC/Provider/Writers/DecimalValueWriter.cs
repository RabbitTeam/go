// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecimalValueWriter.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DecimalValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;

	internal class DecimalValueWriter : RationalValueWriter<decimal>
	{
		protected override string Suffix
		{
			get
			{
				return "m";
			}
		}
	}
}