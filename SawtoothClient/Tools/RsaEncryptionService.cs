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
        private readonly RSACng _cryptoService;
        private static int Sha256Bits => 256;
        private int MaximumAllowedBytes => (_cryptoService.KeySize / 8) - (2 * Sha256Bits / 8) - 2; //With OAEP SHA1

        public RsaEncryptionService(RSAParameters privateRsaParameters)
        {
            _cryptoService = new RSACng();
            _cryptoService.ImportParameters(privateRsaParameters);

            if(_cryptoService.KeySize % 8 != 0)
                throw new ArgumentException("Key size must be a multiple of 8");
        }

        public Token AddSignature(Command command)
        {
            command.PublicKey = _cryptoService.ExportRSAPrivateKey(); //Used for signing, so terminology is flipped
            var sig = new SignatureInfo()
            {
                CommandType = command.CommandType,
                TimeStamp = command.TimeStamp,
                TransactionId = command.TransactionId
            };

            var jsonCommand = JsonConvert.SerializeObject(sig);
            var data = Encoding.UTF8.GetBytes(jsonCommand);

            if (data.Length >= MaximumAllowedBytes)
                throw new ArgumentException($"Too much data to sign, limit is {MaximumAllowedBytes} bytes using current key-size");

            var cipherText = _cryptoService.Encrypt(data, RSAEncryptionPadding.OaepSHA256);

            return new Token()
            {
                Command = command,
                SignedInfo = cipherText
            };
        }
    }
}
