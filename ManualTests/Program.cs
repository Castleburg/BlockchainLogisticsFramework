using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Sawtooth.Sdk;
using Sawtooth.Sdk.Processor;
using SawtoothClient;
using SawtoothClient.Logistic;
using SawtoothClient.Objects;
using SawtoothClient.RideSharing;
using SawtoothClient.Tools;
using SharedObjects;
using SharedObjects.Commands;
using SharedObjects.Enums;
using SharedObjects.Logistic;
using TransactionProcessor;
using TransactionProcessor.Tools;

namespace ManualTests
{
    internal class Program
    {
        private static string FamilyName = "Test";
        private static string Prefix => FamilyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);
        private static string GetAddress(Guid transactionId) => Prefix + transactionId.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString();

        private static string _clientAddress = "http://192.168.0.106:8008";

        private static void Main(string[] args)
        {
            Console.WriteLine("Starting up!");
            var validatorAddress = "tcp://" + (args.Any() ? args.First() : "192.168.0.106:4004");
            var clientAddress = "http://" + (args.Any() ? args.First() : "192.168.0.106:8008");

            //var processor = new Processor(validatorAddress);
            //processor.Run();

            var client = new SawtoothClient.SawtoothClient(clientAddress, "Test", "1.0");

            

            var rsa = RSA.Create();
            rsa.KeySize = 2048;
            var param = rsa.ExportParameters(true);
            var publicKey = rsa.ExportRSAPrivateKey();

            var command = new Command()
            {
                PublicKey = publicKey,
                CommandType = LogisticEnums.Commands.NewEntity,
                CompanyId = "HelloWorld",
                TimeStamp = DateTime.Now,
                TransactionId = Guid.NewGuid()
            };


            var enc = new RsaEncryptionService(param);
            //var token = enc.AddSignature(command);

            //var dec = new RsaDecryptionService();
            //var verified = dec.VerifyToken(token);

            //var lc = new LogisticsClient("HelloWorld", publicKey, client, enc);
            //var response = lc.NewEntity(LogisticEnums.EntityType.RideShare);


            var rep = GetEntityFromState(Guid.Parse("{e17d5486-e93e-46a7-bc4a-8f9cec8cec2d}"));


            


            //var rc = new RideShareClient(lc);

            //var batchStatus = client.GetBatchStatuses("d4f1a2f9dfed0fb2bef43cfa5812d8faec4fe8838eb3ea398505ad0e81b4f8f203e688e090de3fe9647c4124723a1abe9c0b24f5b5a6692a3da632b28c9a5063", 1);
            //var batchContent = batchStatus.Content.ReadAsStringAsync().Result;
            //var batchStatusResponse = JsonConvert.DeserializeObject<BatchStatusResponse>(batchContent);

            Console.WriteLine("Done!");
        }

        public static Entity GetEntityFromState(Guid transactionId)
        {
            var response = GetStateHttp(_clientAddress,
                GetAddress(transactionId));
            if (!response.IsSuccessStatusCode)
                throw new InvalidTransactionException("Unable to fetch transaction state");

            var content = response.Content.ReadAsStringAsync().Result;
            var encodedString = JsonConvert.DeserializeObject<StateResponse>(content);

            var data = Convert.FromBase64String(encodedString.Data);
            var decodedString = Encoding.UTF8.GetString(data);

            var state = JsonConvert.DeserializeObject<Entity>(decodedString);
            return state;
        }

        private static HttpResponseMessage GetStateHttp(string httpAddress, string stateAddress)
        {
            var builder = new StringBuilder();
            builder.Append(httpAddress);
            builder.Append("/state/");
            builder.Append(stateAddress);
            var request = builder.ToString();

            var httpClient = new HttpClient();
            return httpClient.GetAsync(request).Result;
        }

    }
}
