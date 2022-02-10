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
        public bool VerifySignature(CommandToken commandToken)
        {
            var rsa = RSA.Create();
            rsa.ImportParameters(commandToken.RsaParameters);

            var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm(commandToken.HashAlgorithm);

            var publicKey = rsa.ExportRSAPublicKey();

            return rsaDeformatter.VerifySignature(commandToken.Signature.HashedCommand, commandToken.Signature.SignedHashedCommand) &&
                   publicKey.SequenceEqual(commandToken.Command.PublicKey);
        }


        //public bool VerifySignature(CommandToken commandToken)
        //{
        //    var command = commandToken.Command;
        //    var token = Decrypt(commandToken.Signature, commandToken.RsaParameters);

        //    var rsa = RSA.Create();
        //    rsa.ImportParameters(commandToken.RsaParameters);

        //    return (command.PublicKey == token.PublicKey) &&
        //           (command.TimeStamp == token.TimeStamp);
        //}

        //private static Token Decrypt(byte[] encrypted, RSAParameters publicRsaParameters)
        //{
        //    var rsa = new RSACryptoServiceProvider();
        //    rsa.ImportParameters(publicRsaParameters);
        //    var decryptedRsa = rsa.Decrypt(encrypted, false);
        //    var result = Encoding.Default.GetString(decryptedRsa);
        //    var token = JsonConvert.DeserializeObject<Token>(result);
        //    return token;
        //}
    }
}
