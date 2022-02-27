using System;
using SharedObjects.Enums;
using TransactionProcessor.Handlers;
using TransactionProcessor.Process;
using TransactionProcessor.Process.BusinessProcesses.RideShare;
using TransactionProcessor.Tools;

namespace TransactionProcessor
{
    public class Processor
    {
        private readonly string _validatorAddress;
        private readonly string _sawtoothApiAddress;

        public Processor(string validatorAddress, string sawtoothApiAddress)
        {
            if (!Uri.TryCreate(validatorAddress, UriKind.Absolute, out var _))
                throw new Exception($"Invalid validator address: {validatorAddress}");
            _validatorAddress = validatorAddress;
            _sawtoothApiAddress = sawtoothApiAddress;
        }

        public void Run()
        {
            var family = LogisticEnums.EntityType.RideShare.ToString();

            var businessProcess = new RideShareBusinessProcess();
            var logisticProcess = new LogisticProcessor(family, _sawtoothApiAddress, businessProcess);
            var cryptoService = new RsaDecryptionService();

            var processor = new Sawtooth.Sdk.Processor.TransactionProcessor(_validatorAddress);
            processor.AddHandler(new LogisticHandler(LogisticEnums.EntityType.RideShare, "1.0", logisticProcess, cryptoService));
            processor.Start();

            Console.CancelKeyPress += delegate { processor.Stop(); };
        }

    }
}
