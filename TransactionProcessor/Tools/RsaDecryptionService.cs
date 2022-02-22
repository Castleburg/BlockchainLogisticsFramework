using System;
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

        public bool VerifyToken(Token token)
        {
            try
            {
                var cryptoService = new RSACryptoServiceProvider();
                cryptoService.ImportRSAPrivateKey(token.Command.PublicKey, out _);

                var bytes = cryptoService.Decrypt(token.SignedInfo, true);
                var plaintext = Encoding.UTF8.GetString(bytes);
                var sigInfo = JsonConvert.DeserializeObject<SignatureInfo>(plaintext);
                if(sigInfo is null)
                    throw new InvalidTransactionException("No signed command was provided in token");

                return sigInfo.CommandType == token.Command.CommandType &&
                    sigInfo.TimeStamp == token.Command.TimeStamp &&
                    sigInfo.TransactionId.Equals(token.Command.TransactionId);
            }
            catch (JsonSerializationException)
            {
                throw new InvalidTransactionException("Failed to deserialize command");
            }
            catch (Exception e)
            {
                Console.WriteLine("VerifySignature failed due to an exception: " + e);
                return false;
            }
        }
    }
}
