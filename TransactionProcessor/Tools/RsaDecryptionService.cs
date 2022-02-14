using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using SharedObjects.Commands;
using TransactionProcessor.Tools.Interfaces;

namespace TransactionProcessor.Tools
{
    public class RsaDecryptionService : ICryptographicService
    {
        public bool VerifySignature(Token commandToken)
        {
            var rsa = RSA.Create();
            rsa.ImportParameters(commandToken.RsaParameters);

            var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm(commandToken.HashAlgorithm);

            var publicKey = rsa.ExportRSAPublicKey();

            return rsaDeformatter.VerifySignature(commandToken.Signature.HashedCommand, commandToken.Signature.SignedHashedCommand) &&
                   publicKey.SequenceEqual(commandToken.Command.PublicKey);
        }
    }
}
