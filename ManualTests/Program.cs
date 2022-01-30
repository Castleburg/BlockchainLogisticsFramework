using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SawtoothClient;
using TransactionProcessor;

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
