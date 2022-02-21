using System.ComponentModel.DataAnnotations;

namespace TransactionProcessor.Context
{
    public class CustomCertificate
    {
        [Key]
        public string CompanyId { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] Signature { get; set; }
        public string HashAlgorithm { get; set; }
    }
}
