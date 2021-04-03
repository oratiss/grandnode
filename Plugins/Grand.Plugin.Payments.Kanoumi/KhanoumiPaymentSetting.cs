using Grand.Domain.Configuration;
using Grand.Plugin.Payments.Khanoumi.Enumerations;

namespace Grand.Plugin.Payments.Khanoumi
{
    public class KhanoumiPaymentSetting:ISettings
    {
        public string GrpcAddress { get; set; }
        public string GrpcKey { get; set; }
        public string GrpcPassword { get; set; }
        public decimal AdditionalFee { get; set; } = 0;
        public float AdditionalFeePercentage { get; set; } = 0;
        public bool RialToToman { get; set; }
        public bool GateWayChosenByKhanoumiService { get; set; }
        public Enumeration.KhanoumiGateType KhanoumiGateType { get; set; }



    }
}
