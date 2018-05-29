// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestFactoryCertified.cs">
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the public interface for an HTTP request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Implementations
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Provider;
    using System.Net.Http;

    /// <summary>
    /// Creates an IHttpRequest with the given certificate attached to it
    /// </summary>
    class HttpRequestFactoryCertified : IHttpRequestFactory
    {
        private readonly X509Certificate _clientCertificate;

        public HttpRequestFactoryCertified(X509Certificate clientCertificate)
        {
            _clientCertificate = clientCertificate;
        }

        public IHttpRequest Create(Uri uri, Provider.HttpMethod method, string acceptMimeType, string requestMimeType = null)
        {
            var httpClientAdapter = HttpClientAdapter.CreateHttpClientAdapter(uri, method, acceptMimeType, requestMimeType);

            // TODO httpWebRequest.ClientCertificates.Add(_clientCertificate);

            return httpClientAdapter;
        }
    }
}
