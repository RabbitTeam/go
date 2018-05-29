// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterBuilder.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ParameterBuilder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;

	internal class ParameterBuilder
	{
		private readonly Uri _serviceBase;

		public ParameterBuilder(Uri serviceBase, Type sourceType)
		{
			CustomContract.Requires(serviceBase != null);
			CustomContract.Requires(sourceType != null);
			CustomContract.Requires(serviceBase.Scheme == HttpUtility.UriSchemeHttp || serviceBase.Scheme == HttpUtility.UriSchemeHttps);
			//CustomContract.Ensures(((System.Collections.ICollection)this.OrderByParameter).Count == 0);
			//CustomContract.Ensures(serviceBase == this._serviceBase);

			_serviceBase = serviceBase;
			SourceType = sourceType;
			OrderByParameter = new List<string>();
		}

		public string FilterParameter { get; set; }

		public IList<string> OrderByParameter { get; private set; }

		public string SelectParameter { get; set; }

		public string SkipParameter { get; set; }

		public string TakeParameter { get; set; }

		public string ExpandParameter { get; set; }

		public Type SourceType { get; private set; }

		public Uri GetFullUri()
		{
			CustomContract.Ensures(CustomContract.Result<Uri>() != null);
			//CustomContract.Ensures(CustomContract.Result<Uri>().Scheme == HttpUtility.UriSchemeHttp || CustomContract.Result<Uri>().Scheme == HttpUtility.UriSchemeHttps);

			var parameters = new List<string>();
			if (!string.IsNullOrWhiteSpace(FilterParameter))
			{
				parameters.Add(BuildParameter(StringConstants.FilterParameter, HttpUtility.UrlEncode(FilterParameter)));
			}

			if (!string.IsNullOrWhiteSpace(SelectParameter))
			{
				parameters.Add(BuildParameter(StringConstants.SelectParameter, SelectParameter));
			}

			if (!string.IsNullOrWhiteSpace(SkipParameter))
			{
				parameters.Add(BuildParameter(StringConstants.SkipParameter, SkipParameter));
			}

			if (!string.IsNullOrWhiteSpace(TakeParameter))
			{
				parameters.Add(BuildParameter(StringConstants.TopParameter, TakeParameter));
			}

			if (OrderByParameter.Any())
			{
				parameters.Add(BuildParameter(StringConstants.OrderByParameter, string.Join(",", OrderByParameter)));
			}

			if (!string.IsNullOrWhiteSpace(ExpandParameter))
			{
				parameters.Add(BuildParameter(StringConstants.ExpandParameter, ExpandParameter));
			}

			var builder = new UriBuilder(_serviceBase);
			builder.Query = (string.IsNullOrEmpty(builder.Query) ? string.Empty : builder.Query.Substring(1) + "&") + string.Join("&", parameters);

			var resultUri = builder.Uri;

			CustomContract.Assume(_serviceBase.Scheme == HttpUtility.UriSchemeHttp || _serviceBase.Scheme == HttpUtility.UriSchemeHttps);
			CustomContract.Assume(resultUri.Scheme == HttpUtility.UriSchemeHttp || resultUri.Scheme == HttpUtility.UriSchemeHttps);

			return resultUri;
		}

		private static string BuildParameter(string name, string value)
		{
			CustomContract.Ensures(CustomContract.Result<string>() != null);
			//CustomContract.Ensures(0 <= CustomContract.Result<string>().Length); 

			return name + "=" + value;
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(_serviceBase != null);
			CustomContract.Invariant(_serviceBase.Scheme == HttpUtility.UriSchemeHttp || _serviceBase.Scheme == HttpUtility.UriSchemeHttps);
			CustomContract.Invariant(OrderByParameter != null);
		}
	}
}