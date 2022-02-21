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

        public Token AddSignature(Command command, byte[] certificate)
        {

            var rsa = RSA.Create();
            rsa.ImportParameters(_privateRsaParameters);

            var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            rsaFormatter.SetHashAlgorithm(_hashAlgorithm);

            var signedBytes = rsaFormatter.CreateSignature(certificate);

            return new Token()
            {
                Command = command,
                SignedCertificate = signedBytes
            };
        }
    }
}
