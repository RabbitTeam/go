// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenSet.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TokenSet type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser
{
	using System;
	using System.Diagnostics.Contracts;

	internal class TokenSet
	{
		private string _left;
		private string _operation;
		private string _right;

		public TokenSet()
		{
			_left = string.Empty;
			_right = string.Empty;
			_operation = string.Empty;
		}

		public string Left
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<string>() != null);
				return _left;
			}

			set
			{
				CustomContract.Requires<ArgumentNullException>(value != null);
				_left = value;
			}
		}

		public string Operation
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<string>() != null);
				return _operation;
			}

			set
			{
				CustomContract.Requires<ArgumentNullException>(value != null);
				_operation = value;
			}
		}

		public string Right
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<string>() != null);
				return _right;
			}

			set
			{
				CustomContract.Requires<ArgumentNullException>(value != null);
				_right = value;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2}", Left, Operation, Right);
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(_left != null);
			CustomContract.Invariant(_right != null);
			CustomContract.Invariant(_operation != null);
		}
	}
}