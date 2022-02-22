using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Esf;
using SharedObjects.Commands;
using SharedObjects.Logistic;

namespace SawtoothClient.Tools
{
    public class RsaEncryptionService
    {
        private readonly RSACryptoServiceProvider _cryptoService;

        public RsaEncryptionService(RSAParameters privateRsaParameters)
        {
            _cryptoService = new RSACryptoServiceProvider();
            _cryptoService.ImportParameters(privateRsaParameters);
        }

        public Token AddSignature(Command command)
        {
            command.PublicKey = _cryptoService.ExportRSAPrivateKey();
            var sig = new SignatureInfo()
            {
                CommandType = command.CommandType,
                TimeStamp = command.TimeStamp,
                TransactionId = command.TransactionId
            };


            var jsonCommand = JsonConvert.SerializeObject(sig);
            var data = Encoding.UTF8.GetBytes(jsonCommand);

            if(data.Length > 245)
                throw new ArgumentException("Too much data");

            var cipherText = _cryptoService.Encrypt(data, true);

            return new Token()
            {
                Command = command,
                SignedInfo = cipherText
            };
        }
    }
}
