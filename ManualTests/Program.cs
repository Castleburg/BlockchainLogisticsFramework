﻿using System;
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

            Console.WriteLine("Starting up!");
            var validatorAddress = "tcp://" + (args.Any() ? args.First() : "192.168.0.106:4004");
            var clientAddress = "http://" + (args.Any() ? args.First() : "192.168.0.106:8008/batches");

            var processor = new Processor(validatorAddress);
            processor.Run();

            var client = new Client(clientAddress, "Test", "1.0");

            var newEntity = new NewEntity()
            {
                CreatorId = "HelloWorld"
            };
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.NewEntity,
                JsonContainer = JsonConvert.SerializeObject(newEntity),
                TimeStamp = DateTime.Now,
                TransactionId = Guid.NewGuid()
            };

            var result = client.PostPayload(JsonConvert.SerializeObject(command));





            Console.WriteLine("Done!");
        }
    }
}
