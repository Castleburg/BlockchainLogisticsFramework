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

            var enc = new RsaEncryptionService(param);

            var lc = new LogisticsClient(rsa.ExportRSAPublicKey(), client, enc);
            var response = lc.NewEntity(LogisticEnums.EntityType.RideShare, "HelloMe");
            
            Console.WriteLine("Done!");
        }
    }
}
