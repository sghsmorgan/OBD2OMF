using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;

namespace OBD2OMF
{
    public class OMFClient
    {

        #region Private Fields
        private readonly HttpClient _httpClient;
        private readonly bool _useCompression;
        private readonly string _relayUrl;
        private readonly string _producerToken;
        #endregion Private Fields

        #region Public Constructors

        public OMFClient(string relayUrl, string producerToken, bool useCompression)
        {
            _relayUrl = relayUrl;
            _producerToken = producerToken;
            _useCompression = useCompression;

            ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) => true;

            _httpClient = new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 30, 0);
            

        }
        #endregion Public Constructors

        #region Public Enums

        public enum OmfMessageType
        {
            type = 0,
            container,
            data,
        }

        public enum OmfAction
        {
            create = 0,
            update,
            delete,
        }

        #endregion Public Enums

        public HttpResponseMessage SendOmfRequest(OmfMessageType messageType, string contentString, string version = "1.0", OmfAction action = OmfAction.create)
        {
            var _request = new HttpRequestMessage(HttpMethod.Post, new Uri(_relayUrl));
            _request.Headers.Add("producertoken", _producerToken);
            _request.Headers.Add("messagetype", messageType.ToString());
            _request.Headers.Add("messageformat", "JSON");
            _request.Headers.Add("omfversion", $"{version}");
            _request.Headers.Add("action", "CREATE");

            if (_useCompression)
            {
                _request.Headers.Add("compression", "GZIP");
                _request.Content = new ByteArrayContent(GZipCompress(Encoding.UTF8.GetBytes(contentString)));
            }
            else
                _request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(contentString));

            return _httpClient.SendAsync(_request).Result;
        }

        private static byte[] GZipCompress(byte[] uncompressed)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(uncompressed, 0, uncompressed.Length);
                }
                return memory.ToArray();
            }
        }
    }
}
