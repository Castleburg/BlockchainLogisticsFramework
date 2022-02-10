using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using SawtoothClient;
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
            var decrypt = new RsaDecryptionService();
            var encrypt = new RsaEncryptionService();



            //var rsa = RSA.Create();
            //var t = rsa.ExportRSAPublicKey();
            //var stringy = System.Text.Encoding.UTF8.GetString(t);

            var rsa1 = new RSACryptoServiceProvider();
            var rsaParameters = rsa1.ExportParameters(true);

            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.NewEntity,
                TransactionId = Guid.NewGuid(),
                JsonContainer = "HelloWorld",
                TimeStamp = DateTime.Now
            };
            var token = encrypt.SignCommand(command, rsaParameters, "SHA256");
            if(decrypt.VerifySignature(token))
                Console.WriteLine("Hurray!");



            Console.WriteLine("Starting up!");
            var validatorAddress = "tcp://" + (args.Any() ? args.First() : "192.168.0.106:4004");
            var clientAddress = "http://" + (args.Any() ? args.First() : "192.168.0.106:8008/batches");

            var processor = new Processor(validatorAddress);
            processor.Run();

            var client = new Client(clientAddress, "Generalist", "1.0");

            var container = new TestContainer()
            {
                Name = "Item1",
                Numbers = new List<int>{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10},
                Time = DateTime.Today
            };

            var result = client.PostPayload("BorgTest3", "Init", "Item1", "", JsonConvert.SerializeObject(container));
            Console.WriteLine("Done!");
        }
    }
}
