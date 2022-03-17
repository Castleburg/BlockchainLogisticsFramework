using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Sawtooth.Sdk;
using Sawtooth.Sdk.Processor;
using SawtoothClient;
using SawtoothClient.Logistic;
using SawtoothClient.Objects;
using SawtoothClient.RideSharing;
using SawtoothClient.Tools;
using SharedObjects;
using SharedObjects.Commands;
using SharedObjects.Enums;
using SharedObjects.Logistic;
using TransactionProcessor;
using TransactionProcessor.Tools;

namespace ManualTests
{
    internal class Program
    {
        private static string FamilyName = "Test";
        private static string Prefix => FamilyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);
        private static string GetAddress(Guid transactionId) => Prefix + transactionId.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString();

        private static string _clientAddress = "http://192.168.0.106:8008";

        private static void Main(string[] args)
        {
            Console.WriteLine("Starting up!");
            
            Demo();

            Console.WriteLine("Done!");
        }

        public static void Demo()
        {
            var validatorAddress = "tcp://localhost:4004";
            var clientAddress = "http://localhost:8008";

            var processor = new Processor(validatorAddress, clientAddress);
            processor.Run();


            var rsa = RSA.Create();
            rsa.KeySize = 2048;
            var param = rsa.ExportParameters(true);
            var publicKey = rsa.ExportRSAPrivateKey();

            var companyId = "RideSharing Inc.";


            var client = new SawtoothClient.SawtoothClient(clientAddress, LogisticEnums.EntityType.RideShare, "1.0");
            var enc = new RsaEncryptionService(param);
            var logicClient = new LogisticsClient(companyId, publicKey, client, enc);
            var rideClient = new RideShareClient(logicClient);


            var driverId = "BobsId";
            var passengarId = "AliceId";

            var startResponse = rideClient.StartRide(driverId, "Glostrup");
            var transactionId = startResponse.FirstOrDefault().TransactionId;

            var addPassengerResponse = rideClient.AddPassenger(driverId, "Glostrup", transactionId);
            DisplayEntity(GetEntityFromState(transactionId));

            var removePassengerResponse = rideClient.RemovePassenger(passengarId, "Frederiksberg", transactionId);
            DisplayEntity(GetEntityFromState(transactionId));

            var endResponse = rideClient.StopRide(driverId, "Nørrebro", transactionId);
            DisplayEntity(GetEntityFromState(transactionId));

            var rsa2 = RSA.Create();
            rsa.KeySize = 2048;
            var param2 = rsa2.ExportParameters(true);
            var publicKey2 = rsa2.ExportRSAPrivateKey();

            var inviteResponse = rideClient.NewInvite(transactionId, publicKey2);
            DisplayEntity(GetEntityFromState(transactionId));

            var client2 = new SawtoothClient.SawtoothClient(clientAddress, LogisticEnums.EntityType.RideShare, "1.0");
            var enc2 = new RsaEncryptionService(param2);
            var logicClient2 = new LogisticsClient("Parking Inc.", publicKey2, client2, enc2);
            var rideClient2 = new RideShareClient(logicClient2);

            var acceptInviteResponse = rideClient2.AcceptInvite("Nørrebro Udendørs Parkering", DateTime.Now.AddDays(1), 120, transactionId);
            DisplayEntity(GetEntityFromState(transactionId));
        }

        public static void DisplayEntity(Entity entity)
        {
            Console.WriteLine();

            Console.WriteLine($"Type: {entity.Type}");
            Console.WriteLine($"TransactionId: {entity.TransactionId}");
            Console.WriteLine($"CreatorId: {entity.CreatorId}");
            Console.WriteLine($"CreatedDate: {entity.CreatedDate}");
            Console.WriteLine($"ModifiedDate: {entity.LastModified}");

            Console.WriteLine();
            Console.WriteLine($"EventAmount: {entity.Events.Count}");
            if (entity.SignOff.Invites.Count > 0)
            {
                Console.WriteLine($"EventAmount: {entity.Events.LastOrDefault().Type}");
                Console.WriteLine($"EventAmount: {entity.Events.LastOrDefault().JsonContainer}");
            }

            Console.WriteLine();
            Console.WriteLine($"IsFinal: {entity.Final}");
            Console.WriteLine($"InviteAmount: {entity.SignOff.Invites.Count}");
            if (entity.SignOff.Invites.Count > 0)
                Console.WriteLine($"InviteStatus: {entity.SignOff.Invites.FirstOrDefault().InviteStatus}");
            
            Console.WriteLine();
            Console.WriteLine($"SignatoryAmount: {entity.SignOff.Signatories.Count}");
            if (entity.SignOff.Signatories.Count > 0)
            {
                Console.WriteLine($"SignatoryId: {entity.SignOff.Signatories.FirstOrDefault().Id}");
                Console.WriteLine($"SignatoryContent: {entity.SignOff.Signatories.FirstOrDefault().JsonContainer}");
            }
            
            Console.WriteLine();
        }


        public static Entity GetEntityFromState(Guid transactionId)
        {
            var response = GetStateHttp(_clientAddress,
                GetAddress(transactionId));
            if (!response.IsSuccessStatusCode)
                throw new InvalidTransactionException("Unable to fetch transaction state");

            var content = response.Content.ReadAsStringAsync().Result;
            var encodedString = JsonConvert.DeserializeObject<StateResponse>(content);

            var data = Convert.FromBase64String(encodedString.Data);
            var decodedString = Encoding.UTF8.GetString(data);

            var state = JsonConvert.DeserializeObject<Entity>(decodedString);
            return state;
        }

        private static HttpResponseMessage GetStateHttp(string httpAddress, string stateAddress)
        {
            var builder = new StringBuilder();
            builder.Append(httpAddress);
            builder.Append("/state/");
            builder.Append(stateAddress);
            var request = builder.ToString();

            var httpClient = new HttpClient();
            return httpClient.GetAsync(request).Result;
        }

    }
}
