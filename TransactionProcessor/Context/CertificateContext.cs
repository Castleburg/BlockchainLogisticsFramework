using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace TransactionProcessor.Context
{
    public class CertificateContext : DbContext
    {
        public CertificateContext(string connectionString) : base(connectionString)
        {
        }

        public CustomCertificate GetCertificate(string companyId)
        {
            return Certificates.First(x => x.CompanyId.Equals(companyId));
        }

        private DbSet<CustomCertificate> Certificates { get; set; }
    }
}
