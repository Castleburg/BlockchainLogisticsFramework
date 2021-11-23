using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SawtoothClient;
using TransactionProcessor;

namespace ManualTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up!");
            var validatorAddress = "tcp://" + (args.Any() ? args.First() : "127.0.0.1:4004");
            var clientAddress = "http://" + (args.Any() ? args.First() : "127.0.0.1:8008/batches");

            var processor = new Processor(validatorAddress);
            processor.Run();

            var client = new Client(clientAddress, "Transport1", "1.0");

            var result = client.PostPayload("TestName", "set", "helloWorld");
            Console.WriteLine("Done!");
        }
    }
}
