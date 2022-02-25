using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using SawtoothClient;
using SawtoothClient.Logistic;
using SawtoothClient.Objects;
using SawtoothClient.RideSharing;
using SawtoothClient.Tools;
using SharedObjects.Commands;
using SharedObjects.Enums;
using TransactionProcessor;
using TransactionProcessor.Tools;

namespace ManualTests
{
    internal class Program
    {
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

            var lc = new LogisticsClient("HelloWorld", publicKey, client, enc);
            var response = lc.NewEntity(LogisticEnums.EntityType.RideShare);

            //var rc = new RideShareClient(lc);

            //var batchStatus = client.GetBatchStatuses("d4f1a2f9dfed0fb2bef43cfa5812d8faec4fe8838eb3ea398505ad0e81b4f8f203e688e090de3fe9647c4124723a1abe9c0b24f5b5a6692a3da632b28c9a5063", 1);
            //var batchContent = batchStatus.Content.ReadAsStringAsync().Result;
            //var batchStatusResponse = JsonConvert.DeserializeObject<BatchStatusResponse>(batchContent);

            Console.WriteLine("Done!");
        }
    }
}
