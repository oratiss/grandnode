using System;
using System.Collections.Generic;
using System.Text;
using Grand.Domain.Configuration;

namespace Grand.Plugin.Payments.Khanoumi
{
    public class KhanoumiPaymentSetting:ISettings
    {
        public string GrpcAddress { get; set; }
        public string GrpcKey { get; set; }
        public string GrpcPassword { get; set; }
        public decimal AdditionalFee { get; set; } = 0;
        public float AdditionalFeePercentage { get; set; } = 0;


    }
}
