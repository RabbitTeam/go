// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleValueWriter.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DoubleValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	internal class DoubleValueWriter : RationalValueWriter<double>
	{
		protected override string Suffix
		{
			get
			{
				return string.Empty;
			}
		}
	}
}