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

        public Processor(string validatorAddress)
        {
            if (!Uri.TryCreate(validatorAddress, UriKind.Absolute, out var _))
                throw new Exception($"Invalid validator address: {validatorAddress}");
            _validatorAddress = validatorAddress;
        }

        public void Run()
        {
            var processor = new Sawtooth.Sdk.Processor.TransactionProcessor(_validatorAddress);
            processor.AddHandler(new TransportHandler());
            processor.Start();

            Console.CancelKeyPress += delegate { processor.Stop(); };
        }

    }
}
