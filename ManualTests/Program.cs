using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using SawtoothClient;
using SawtoothClient.Logistic;
using SawtoothClient.Objects;
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

            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.NewEntity,
                TimeStamp = DateTime.Now,
                TransactionId = Guid.NewGuid()
            };

            var rsa = RSA.Create();
            var param = rsa.ExportParameters(true);

            var lc = new LogisticsClient(rsa.ExportRSAPublicKey(), client);
            //lc.NewEntity(LogisticEnums.EntityType.RideShare, "HelloMe");
            


            //var res = client.GetBatches("0", "0", 100, "");
            //var content = res.Content.ToString();

            var res = client.GetBatchStatuses("f760195b586f3124a00676a5f86be93a94f46f2edf591a99529942a7202eb6174de93f4464f3f40458d8fd39ffbb8f2f79c83175f6b63e595f71c704b36dc9c6",5);
            var content = res.Content.ReadAsStringAsync().Result;


            var test =
                "{\n  \"link\": \"http://192.168.0.106:8008/batch_statuses?id=f760195b586f3124a00676a5f86be93a94f46f2edf591a99529942a7202eb6174de93f4464f3f40458d8fd39ffbb8f2f79c83175f6b63e595f71c704b36dc9c6\"\n}";
            var ljdhfg = JsonConvert.DeserializeObject<BatchResponse>(test);

            Console.WriteLine("Done!");
        }
    }
}
