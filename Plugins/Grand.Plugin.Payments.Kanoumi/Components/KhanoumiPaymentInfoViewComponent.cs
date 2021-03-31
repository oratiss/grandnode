using Grand.Plugin.Payments.Khanoumi.Models;
using Grand.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Plugin.Payments.Khanoumi.Components
{
    [ViewComponent(Name = "KhanoumiPaymentInfo")]
    public class KhanoumiPaymentInfoViewComponent:ViewComponent
    {
        private readonly KhanoumiPaymentSetting _khanoumiPaymentSetting;
        private readonly ILocalizationService _localizationService;

        public KhanoumiPaymentInfoViewComponent(KhanoumiPaymentSetting khanoumiPaymentSetting, ILocalizationService localizationService)
        {
            _khanoumiPaymentSetting = khanoumiPaymentSetting;
            _localizationService = localizationService;
        }

        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel() {
                Description = _localizationService.GetResource("Payments.Plugin.Khanoumi.KhanoumiPaymentInfoRedirect")
            };

            return View("~/Plugins/Payments.Khanoumi/Views/PaymentKhanoumi/PaymentInfo.cshtml", model);
        }
    }
}
