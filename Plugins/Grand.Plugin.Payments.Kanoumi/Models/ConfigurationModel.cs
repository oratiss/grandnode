using Grand.Core.Models;
using Grand.Plugin.Payments.Khanoumi.Enumerations;

namespace Grand.Plugin.Payments.Khanoumi.Models
{
    public class ConfigurationModel:BaseModel
    {
        public string GrpcAddress { get; set; }
        public string GrpcKey { get; set; }
        public string GrpcPassword { get; set; }
        public decimal AdditionalFee { get; set; }
        public float AdditionalFeePercentage { get; set; }
        public bool RialToToman { get; set; }
        public bool UseKhanoumiGate { get; set; }
        public Enumeration.KhanoumiGateType KhanoumiGateType { get; set; }
        public bool UseSandBox { get; set; }



    }
}
