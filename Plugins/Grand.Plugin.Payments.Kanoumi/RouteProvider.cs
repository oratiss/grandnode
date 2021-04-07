using Grand.Core.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Payments.Khanoumi
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            
            routeBuilder.MapControllerRoute("Plugin.Payments.Khanoumi.SamanResultHandler", "Plugins/PaymentKhanoumi/SamanResultHandler",
                new { controller = "PaymentKhanoumi", action = "SamanResultHandler" });

            routeBuilder.MapControllerRoute("NPlugin.Payments.Khanoumi.SamanErrorHandler", "Plugins/PaymentKhanoumi/SamanErrorHandler",
                new { controller = "PaymentKhanoumi", action = "SamanErrorHandler" });     
            
            routeBuilder.MapControllerRoute("Plugin.Payments.Khanoumi.ResultHandler", "Plugins/PaymentKhanoumi/ZarinpalResultHandler",
                new { controller = "PaymentKhanoumi", action = "ZarinpalResultHandler" });

            routeBuilder.MapControllerRoute("NPlugin.Payments.Khanoumi.ZarinpalErrorHandler", "Plugins/PaymentKhanoumi/ZarinpalErrorHandler",
                new { controller = "PaymentKhanoumi", action = "ZarinpalErrorHandler" });
        }

        public int Priority => -1;
    }
}
