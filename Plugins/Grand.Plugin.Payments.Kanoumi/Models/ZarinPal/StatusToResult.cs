namespace Grand.Plugin.Payments.Khanoumi.Models.Zarinpal
{
    public class StatusToResult
    {
        public StatusToResult()
        {
            Message = string.Empty;
            IsOk = false;
        }
        public string Message { get; set; }
        public bool IsOk { get; set; }
    }
}
