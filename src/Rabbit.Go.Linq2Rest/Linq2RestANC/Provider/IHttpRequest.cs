// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHttpRequest.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the public enumeration of supported HTTP methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;

	/// <summary>
    /// Defines the public enumeration of supported HTTP methods.
    /// </summary>
    public enum HttpMethod
    {
        /// <summary>
        /// Shouldn't ever be explicitly used. Here as a default.
        /// </summary>
        None = 0, 

        /// <summary>
        /// Represents the GET HTTP method.
        /// </summary>
        Get, 

        /// <summary>
        /// Represents the PUT HTTP method.
        /// </summary>
        Put, 

        /// <summary>
        /// Represents the POST HTTP method.
        /// </summary>
        Post, 

        /// <summary>
        /// Represents the DELETE HTTP method.
        /// </summary>
        Delete
    }

    /// <summary>
    /// Defines the public interface for an HttpRequest.
    /// </summary>
    [ContractClass(typeof(HttpRequestContracts))]
    public interface IHttpRequest
    {
        /// <summary>
        /// Gets a System.IO.Stream object to write request data.
        /// </summary>
        Stream GetRequestStream();

        /// <summary>
        /// Gets a System.IO.Stream object containing the response to the request.
        /// </summary>
        Stream GetResponseStream();
    }

    [ContractClassFor(typeof(IHttpRequest))]
    internal abstract class HttpRequestContracts : IHttpRequest
    {
        public Stream GetRequestStream()
        {
            CustomContract.Ensures(CustomContract.Result<Stream>() != null);
            //CustomContract.Ensures(CustomContract.Result<Stream>().CanWrite);

            throw new NotImplementedException();
        }

        public Stream GetResponseStream()
        {
            CustomContract.Ensures(CustomContract.Result<Stream>() != null);
            //CustomContract.Ensures(CustomContract.Result<Stream>().CanRead);

            throw new NotImplementedException();
        }
    }
}
