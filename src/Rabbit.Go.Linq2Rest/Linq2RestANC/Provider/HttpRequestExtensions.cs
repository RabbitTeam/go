// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Extensions on the IHttpRequest interface (aka Extension Interface Pattern).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System.Diagnostics.Contracts;
	using System.IO;

	/// <summary>
	/// Extensions on the IHttpRequest interface (aka Extension Interface Pattern). 
	/// </summary>
	public static class HttpRequestExtensions
	{
		/// <summary>
		/// Writes a stream to the request stream of an IHttpRequest implementation.
		/// </summary>
		/// <param name="httpRequest">The request we are writing our stream to.</param>
		/// <param name="inputStream">The stream we want to write to our request.</param>
		public static void WriteRequestStream(this IHttpRequest httpRequest, Stream inputStream)
		{
			CustomContract.Requires(httpRequest != null);
			CustomContract.Requires(inputStream != null);

            using (var requestStream = httpRequest.GetRequestStream())
            {
                inputStream.CopyTo(requestStream);
            }
		}
	}
}
