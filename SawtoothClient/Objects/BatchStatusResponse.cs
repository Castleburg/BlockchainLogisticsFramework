using System;
using System.Collections.Generic;
using System.Text;

namespace SawtoothClient.Objects
{
    public class BatchStatusResponse
    {
        public BatchStatus[] Data { get; set; }
    }

    public class BatchStatus
    {
        public string Id { get; set; }
        public string[] Invalid_transactions { get; set; }
        public string Status { get; set; }

        public SawtoothEnums.BatchStatus GetStatus()
        {
            var status = Status switch
            {
                "COMMITTED" => SawtoothEnums.BatchStatus.Committed,
                "INVALID" => SawtoothEnums.BatchStatus.Invalid,
                "PENDING" => SawtoothEnums.BatchStatus.Pending,
                _ => SawtoothEnums.BatchStatus.Unknown
            };
            return status;
        }

    }
}
