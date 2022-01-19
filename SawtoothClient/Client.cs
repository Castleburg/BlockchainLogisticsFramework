using System.Net.Http;
using PeterO.Cbor;
using Sawtooth.Sdk;
using Sawtooth.Sdk.Client;

namespace SawtoothClient
{
    public class Client
    {
        private readonly string _address;
        private readonly string _prefix;
        private readonly Encoder _encoder;

        private readonly HttpClient _httpClient;

        public Client(string address, string familyName, string familyVersion)
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

            _encoder = new Encoder(settings, signer.GetPrivateKey());
        }

        public HttpResponseMessage PostPayload(string name, string verb, string itemId, string type, string json)
        {
            var obj = CBORObject.NewMap()
                .Add("name", name)
                .Add("itemId", itemId)
                .Add("verb", verb)
                .Add("type", type)
                .Add("json", json);

            var payload = _encoder.EncodeSingleTransaction(obj.EncodeToBytes());
            var content = new ByteArrayContent(payload);
            content.Headers.Add("Content-Type", "application/octet-stream");
            return _httpClient.PostAsync(_address, content).Result;
        }

    }
}
