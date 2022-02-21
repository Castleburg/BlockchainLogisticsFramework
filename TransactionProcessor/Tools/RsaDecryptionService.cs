using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Tls;
using Sawtooth.Sdk.Processor;
using SharedObjects.Commands;
using TransactionProcessor.Context;
using TransactionProcessor.Tools.Interfaces;

namespace TransactionProcessor.Tools
{
    public class RsaDecryptionService : ICryptographicService
    {
        public bool VerifySignature(Token token, CustomCertificate certificate)
        {
            try
            {
                var rsa = RSA.Create();
                rsa.ImportSubjectPublicKeyInfo(certificate.PublicKey, out _);

                var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm(certificate.HashAlgorithm);

                return rsaDeformatter.VerifySignature(certificate.Signature,
                           token.SignedCertificate) &&
                       certificate.PublicKey.SequenceEqual(token.Command.PublicKey);
            }
            catch
            {
                return false;
            }
        }
    }
}
