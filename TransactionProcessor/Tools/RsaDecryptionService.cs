using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Sawtooth.Sdk.Processor;
using SharedObjects.Commands;
using TransactionProcessor.Tools.Interfaces;

namespace TransactionProcessor.Tools
{
    public class RsaDecryptionService : ICryptographicService
    {
        public bool VerifySignature(Token token)
        {
            try
            {
                var rsa = RSA.Create();
                rsa.ImportParameters(token.RsaParameters);

                var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm(token.HashAlgorithm);

                var publicKey = rsa.ExportRSAPublicKey();

                return rsaDeformatter.VerifySignature(token.Signature.HashedCommand,
                           token.Signature.SignedHashedCommand) &&
                       publicKey.SequenceEqual(token.Command.PublicKey);
            }
            catch
            {
                return false;
            }
        }
    }
}
