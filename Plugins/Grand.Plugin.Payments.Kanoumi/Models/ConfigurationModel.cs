using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Payments.Khanoumi.Models
{
    public class ConfigurationModel
    {
        public string GrpcAddress { get; set; }
        public string GrpcKey { get; set; }
        public string GrpcPassword { get; set; }
        public decimal AdditionalFee { get; set; }
        public float AdditionalFeePercentage { get; set; }
    }
}
