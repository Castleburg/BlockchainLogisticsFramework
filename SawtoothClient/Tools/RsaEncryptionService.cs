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
        private readonly RSAParameters _privateRsaParameters;
        private readonly string _hashAlgorithm;

        public RsaEncryptionService(RSAParameters privateRsaParameters, string hashAlgorithm = "SHA256")
        {
            _privateRsaParameters = privateRsaParameters;
            _hashAlgorithm = hashAlgorithm;
        }

        public Token SignCommand(Command command)
        {

            var rsa = RSA.Create();
            rsa.ImportParameters(_privateRsaParameters);

            var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            rsaFormatter.SetHashAlgorithm(_hashAlgorithm);

            command.PublicKey = rsa.ExportRSAPublicKey();

            var bytes = ObjectToByteArray(command);
            var signedBytes = rsaFormatter.CreateSignature(bytes);
            var publicRsaParameters = rsa.ExportParameters(false); //true returns public and private key, false only public

            var signature = new Signature()
            {
                SignedHashedCommand = signedBytes,
                HashedCommand = bytes
            };
            return new Token()
            {
                Command = command,
                RsaParameters = publicRsaParameters,
                Signature = signature,
                HashAlgorithm = _hashAlgorithm
            };
        }
        private static byte[] ObjectToByteArray(object obj)
        {
            var jsonString = JsonConvert.SerializeObject(obj);
            using var sha256Hash = SHA256.Create();
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(jsonString));
            return bytes;
        }
    }
}
