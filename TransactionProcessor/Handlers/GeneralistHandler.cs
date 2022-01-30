using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeterO.Cbor;
using Sawtooth.Sdk;
using Sawtooth.Sdk.Processor;
using Google.Protobuf;
using Newtonsoft.Json;
using SharedObjects;

namespace TransactionProcessor.Handlers
{
    internal class GeneralistHandler : ITransactionHandler
    {
        public string FamilyName => "Generalist";
        public string Version => "1.0";
        public string[] Namespaces => new[] { FamilyName.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString() };
        private string Prefix => FamilyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);

        public async Task ApplyAsync(TpProcessRequest request, TransactionContext context)
        {
            Console.WriteLine("Processor Starting up!");
            var obj = CBORObject.DecodeFromBytes(request.Payload.ToByteArray());

            var name = obj["name"].AsString();
            var itemId = obj["itemId"].AsString();
            var verb = obj["verb"].AsString();
            var type = obj["type"].AsString();
            var json = obj["json"].AsString();
            
            if(NotJson(json)) 
                throw new InvalidTransactionException("Payload did not have a valid json format");
            var state = await context.GetStateAsync(new[] { GetAddress(name) });

            if (NotInitNorInitialized(verb, state)) 
                throw new InvalidTransactionException("No address related to that name. Use \"Init\" to create a new one.");

            var value = verb switch
            {
                "Init" => Init(type),
                "Add" => AddItem(UnpackByteString(state.First().Value), itemId, json),
                "Remove" => RemoveItem(UnpackByteString(state.First().Value), itemId),
                "Update" => UpdateItem(UnpackByteString(state.First().Value), itemId, json),
                _ => throw new InvalidTransactionException($"Unknown verb {verb}")
            };
            await SaveState(name, value, context);
        }

        private string GetAddress(string name) => Prefix + name.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString();
        private static bool NotInitNorInitialized(string verb, Dictionary<string, ByteString> state) => !verb.Equals("Init") && !state.Any();

        private static bool NotJson(string json)
        {
            try
            {
                JsonConvert.DeserializeObject(json);
                return false;
            }
            catch (FormatException)
            {
                return true;
            }
        }

        private static GeneralistContainer UnpackByteString(ByteString byteStringJson)
        {
            return JsonConvert.DeserializeObject<GeneralistContainer>(byteStringJson.ToStringUtf8());
        }

        private static GeneralistContainer Init(string type)
        {
            return new GeneralistContainer
            {
                Type = type,
                CreatedDate = DateTime.Now, 
                ItemList = new Dictionary<string, JsonContainer>()
            };
        }

        private static GeneralistContainer AddItem(GeneralistContainer generalistContainer, string itemId, string json)
        {
            if (generalistContainer.ItemList.ContainsKey(itemId))
                throw new InvalidTransactionException($"Cannot add the item ({itemId}), as it already exists");

            var now = DateTime.Now;
            var item = new JsonContainer
            {
                AddedDate = now,
                UpdatedDate = now
            };
            item.SetJsonByteString(json);

            generalistContainer.ItemList.Add(itemId, item);
            return generalistContainer;
        }

        private static GeneralistContainer RemoveItem(GeneralistContainer generalistContainer, string itemId)
        {
            if (!generalistContainer.ItemList.ContainsKey(itemId))
                throw new InvalidTransactionException($"Cannot remove the item ({itemId}), as it does not exists");

            generalistContainer.ItemList.Remove(itemId);
            return generalistContainer;
        }

        private static GeneralistContainer UpdateItem(GeneralistContainer generalistContainer, string itemId, string json)
        {
            if (!generalistContainer.ItemList.ContainsKey(itemId))
                throw new InvalidTransactionException($"Cannot update the item ({itemId}), as it does not exists");

            generalistContainer.ItemList[itemId].SetJsonByteString(json);
            generalistContainer.ItemList[itemId].UpdatedDate = DateTime.Now;
            return generalistContainer;
        }

        private async Task SaveState(string name, GeneralistContainer value, TransactionContext context)
        {
            var jsonValue = JsonConvert.SerializeObject(value);
            await context.SetStateAsync(new Dictionary<string, ByteString>
            {
                { GetAddress(name), ByteString.CopyFrom(jsonValue, Encoding.UTF8) }
            });
        }
    }
}