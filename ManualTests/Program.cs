using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SawtoothClient;
using TransactionProcessor;

namespace ManualTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up!");
            var validatorAddress = args.Any() ? args.First() : "tcp://127.0.0.1:4004";

            var processorThread = Task.Run(() =>
            {
                var processor = new Processor(validatorAddress);
                processor.Run();
            });

            var client = new Client(validatorAddress, "Transport1", "1.0");

            client.PostPayload("TestName", "set", "");

            Console.WriteLine("Waiting for processorThread to finish");
            processorThread.Wait();
            Console.WriteLine("Done!");
        }
    }
}
