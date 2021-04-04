using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Payments.Khanoumi.Models.Saman
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
