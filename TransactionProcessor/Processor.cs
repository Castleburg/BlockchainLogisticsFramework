using System;
using System.Collections.Generic;
using System.Text;
using Sawtooth.Sdk;
using TransactionProcessor.Handlers;

namespace TransactionProcessor
{
    public class Processor
    {
        private readonly string _validatorAddress;
        private readonly string _prefix;

        public Processor(string validatorAddress, string familyName)
        {
            if (!Uri.TryCreate(validatorAddress, UriKind.Absolute, out var _))
                throw new Exception($"Invalid validator address: {validatorAddress}");
            _validatorAddress = validatorAddress;
            _prefix = familyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);
        }

        public void Run()
        {
            var processor = new Sawtooth.Sdk.Processor.TransactionProcessor(_validatorAddress);
            processor.AddHandler(new TransportHandler(_prefix));
            processor.Start();

            Console.CancelKeyPress += delegate { processor.Stop(); };
        }

    }
}
