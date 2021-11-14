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

namespace TransactionProcessor.Handlers
{
    internal class TransportHandler : ITransactionHandler
    {
        public string FamilyName => "Transport1";
        public string Version => "1.0";
        public string[] Namespaces => new[] { FamilyName.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString() };
        readonly string _prefix;

        public TransportHandler(string prefix)
        {
            _prefix = prefix;
        }

        public async Task ApplyAsync(TpProcessRequest request, TransactionContext context)
        {
            var obj = CBORObject.DecodeFromBytes(request.Payload.ToByteArray());

            var name = obj["Name"].AsString();
            var verb = obj["Verb"].AsString().ToLowerInvariant();

            switch (verb)
            {
                case "set":
                    var value = obj["Value"].AsString();
                    await SetLocation(name, value, context);
                    break;
                default:
                    throw new InvalidTransactionException($"Unknown verb {verb}");
            }
        }

        private string GetAddress(string name) => _prefix + name.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString();

        private async Task SetLocation(string name, string value, TransactionContext context)
        {
            //var state = await context.GetStateAsync(Arrayify(GetAddress(name)));
            Console.WriteLine($"TransportHandler SetLocation name:{name}, value:{value}");
            
        }



    }
}
