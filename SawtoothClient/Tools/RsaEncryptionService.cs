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
        public CommandToken SignCommand(Command command, RSAParameters privateRsaParameters, string hashAlgorithm)
        {

            var rsa = RSA.Create();
            rsa.ImportParameters(privateRsaParameters);

            var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            rsaFormatter.SetHashAlgorithm(hashAlgorithm); //SHA256

            command.PublicKey = rsa.ExportRSAPublicKey();

            var bytes = ObjectToByteArray(command);
            var signedBytes = rsaFormatter.CreateSignature(bytes);
            var publicRsaParameters = rsa.ExportParameters(false); //true returns public and private key, false only public

            //var rsa = new RSACryptoServiceProvider(2048);
            //rsa.ImportParameters(privateRsaParameters);

            //if (rsa.PublicOnly)
            //    throw new ArgumentException("No private key found in RSAParameters");

            

            //var publicRsaParameters = rsa.ExportParameters(true); //true returns public and private key, false only public
            //var signature = rsa.Encrypt(bytes, false);
            var signature = new Signature()
            {
                SignedHashedCommand = signedBytes,
                HashedCommand = bytes
            };
            return new CommandToken()
            {
                Command = command,
                RsaParameters = publicRsaParameters,
                Signature = signature,
                HashAlgorithm = hashAlgorithm
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
