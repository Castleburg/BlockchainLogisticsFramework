using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeterO.Cbor;
using Sawtooth.Sdk;
using Sawtooth.Sdk.Processor;
using Sawtooth.Sdk.Client;
using Google.Protobuf;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace TransactionProcessor.Handlers
{
    internal class GeneralistHandler : ITransactionHandler
    {
        public string FamilyName => "Generalist";
        public string Version => "1.0";
        public string[] Namespaces => new[] { FamilyName.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString() };
        private string Prefix => FamilyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);

        public GeneralistHandler()
        {
        }

        public async Task ApplyAsync(TpProcessRequest request, TransactionContext context)
        {
            var obj = CBORObject.DecodeFromBytes(request.Payload.ToByteArray());

            var name = obj["name"].AsString();
            var json = obj["json"].AsString();

            try
            {
                JsonConvert.DeserializeObject(json);

            }
            catch (FormatException ex)
            {
                throw new InvalidTransactionException("Payload did not have a valid json format");
            }
            await context.SetStateAsync(new Dictionary<string, ByteString>
            {
                { GetAddress(name), ByteString.CopyFrom(json, Encoding.Unicode) }
            });
        }

        private string GetAddress(string name) => Prefix + name.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString();
    }
}