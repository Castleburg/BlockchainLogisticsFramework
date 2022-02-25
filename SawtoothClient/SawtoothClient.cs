using System.Net.Http;
using System.Text;
using PeterO.Cbor;
using Sawtooth.Sdk;
using Sawtooth.Sdk.Client;

namespace SawtoothClient
{
    public class SawtoothDataEnvelope
    {
        readonly string data;
        readonly string head;
        readonly string link;
        readonly Paging paging;

    }

    public class Error
    {
        readonly int code;
        readonly string title;
        readonly string message;
    }

    public class Paging
    {
        readonly int start_index;
        readonly int total_count;
        readonly string previous;
        readonly string next;
    }

    public class SawtoothClient
    {
        private readonly string _address;
        private readonly string _prefix;
        private readonly Sawtooth.Sdk.Client.Encoder _encoder;

        //Resource endpoints - getters
        private readonly string _batches_resource_endpoint = "/batches";
        private readonly string _blocks_resource_endpoint = "/blocks";
        private readonly string _batch_statues_resource_endpoint = "/batch_statuses";

        private readonly HttpClient _httpClient;

        public SawtoothClient(string address, string familyName, string familyVersion)
        {
            _address = address;
            _httpClient = new HttpClient();
            _prefix = familyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);

            var signer = new Signer();
            var settings = new EncoderSettings()
            {
                BatcherPublicKey = signer.GetPublicKey().ToHexString(),
                SignerPublickey = signer.GetPublicKey().ToHexString(),
                FamilyName = familyName,
                FamilyVersion = familyVersion
            };
            settings.Inputs.Add(_prefix);
            settings.Outputs.Add(_prefix);

            _encoder = new Sawtooth.Sdk.Client.Encoder(settings, signer.GetPrivateKey());
        }

        public HttpResponseMessage PostBatch(string json)
        {
            var builder = new StringBuilder();
            builder.Append(_address);
            builder.Append(_batches_resource_endpoint);
            var requestAddress = builder.ToString();
            var payload = _encoder.EncodeSingleTransaction(json.ToByteArray());
            var content = new ByteArrayContent(payload);
            content.Headers.Add("Content-Type", "application/octet-stream");
            return _httpClient.PostAsync(requestAddress, content).Result;
        }
                
        public HttpResponseMessage GetBatches(string head, string start, int limit, string reverse)
        {
            var builder = new StringBuilder();
            builder.Append(_address);
            builder.Append(_batches_resource_endpoint);
            //builder.Append("?head=");
            //builder.Append(head);
            //builder.Append("&start=");
            //builder.Append(start);
            //builder.Append("&limit=");
            //builder.Append(limit);
            //builder.Append("&reverse=");
            //builder.Append(reverse);
            var request = builder.ToString();
            return _httpClient.GetAsync(request).Result;
        }

        public HttpResponseMessage GetBatch(string batchid)
        {
            var builder = new StringBuilder();
            builder.Append(_address);
            builder.Append(_batches_resource_endpoint);
            builder.Append(batchid);
            var request = builder.ToString();
            return _httpClient.GetAsync(request).Result;
        }


        /// <summary>
        /// Functionality to get batch statuses
        /// </summary>
        /// <param name="id">  A comma-separated list of batch ids</param>
        /// <param name="wait"> A time in seconds to wait for commit</param>
        public HttpResponseMessage GetBatchStatuses(string id, int wait)
        {
            var builder = new StringBuilder();
            builder.Append(_address);
            builder.Append(_batch_statues_resource_endpoint);
            builder.Append("?id=");
            builder.Append(id);
            builder.Append("&wait=");
            builder.Append(wait);
            var request = builder.ToString();
            return _httpClient.GetAsync(request).Result;
        }

        public HttpResponseMessage GetBlocks(bool useMin, string min, string max, int count)
        {
            var builder = new StringBuilder();
            builder.Append(_address);
            builder.Append(_blocks_resource_endpoint);
            builder.Append("?count=");
            builder.Append(count);
            if (useMin)
            {
                builder.Append("&min=");
                builder.Append(min);
            }
            else
            {
                builder.Append("&max=");
                builder.Append(max);
            }
            var request = builder.ToString();
            return _httpClient.GetAsync(request).Result;
        }

        public HttpResponseMessage GetBlock(string block_id)
        {
            var builder = new StringBuilder();
            builder.Append(_address);
            builder.Append(_blocks_resource_endpoint);
            builder.Append(block_id);
            var request = builder.ToString();
            return _httpClient.GetAsync(request).Result;
        }



    }

}

