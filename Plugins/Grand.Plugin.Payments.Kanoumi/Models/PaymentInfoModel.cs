using System;
using System.Collections.Generic;
using System.Text;
using Grand.Core.ModelBinding;
using Grand.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Grand.Plugin.Payments.Khanoumi.Models
{
    public class PaymentInfoModel:BaseModel
    {

        public string Description { get; set; }


    }
}
