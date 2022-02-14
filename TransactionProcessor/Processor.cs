using System;
using TransactionProcessor.Handlers;
using TransactionProcessor.Process;
using TransactionProcessor.Process.BusinessProcesses;
using TransactionProcessor.Process.BusinessProcesses.RideShare;
using TransactionProcessor.Tools;

namespace TransactionProcessor
{
    public class Processor
    {
        private readonly string _validatorAddress;

        public Processor(string validatorAddress)
        {
            if (!Uri.TryCreate(validatorAddress, UriKind.Absolute, out var _))
                throw new Exception($"Invalid validator address: {validatorAddress}");
            _validatorAddress = validatorAddress;
        }

        public void Run()
        {
            var businessProcess = new RideShareBusinessProcess();
            var logisticProcess = new LogisticProcessor("Test", businessProcess);
            var cryptoService = new RsaDecryptionService();

            var processor = new Sawtooth.Sdk.Processor.TransactionProcessor(_validatorAddress);
            processor.AddHandler(new LogisticHandler("Test", "1.0", logisticProcess, cryptoService));
            processor.Start();

            Console.CancelKeyPress += delegate { processor.Stop(); };
        }

    }
}
