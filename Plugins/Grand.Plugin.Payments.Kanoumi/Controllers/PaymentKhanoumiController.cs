using Grand.Framework.Mvc.Filters;
using Grand.Plugin.Payments.Khanoumi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Grand.Services.Configuration;

namespace Grand.Plugin.Payments.Khanoumi.Controllers
{
    public class PaymentKhanoumiController : Controller
    {
        private readonly KhanoumiPaymentSetting _khanoumiPaymentSetting;
        private readonly ISettingService _settingService;

        public PaymentKhanoumiController(KhanoumiPaymentSetting khanoumiPaymentSetting, ISettingService settingService)
        {
            _khanoumiPaymentSetting = khanoumiPaymentSetting;
            _settingService = settingService;
        }

        [HttpGet]
        [AuthorizeAdmin]
        [Area("Admin")]
        public IActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.GrpcKey = _khanoumiPaymentSetting.GrpcKey;
            model.GrpcPassword = _khanoumiPaymentSetting.GrpcPassword;
            model.GrpcAddress = _khanoumiPaymentSetting.GrpcAddress;
            model.AdditionalFee = _khanoumiPaymentSetting.AdditionalFee;
            model.AdditionalFeePercentage = _khanoumiPaymentSetting.AdditionalFeePercentage;

            return View("~/Plugins/Payments.Khanoumi/Views/PaymentKhanoumi/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area("Admin")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            _khanoumiPaymentSetting.GrpcAddress = model.GrpcAddress;
            _khanoumiPaymentSetting.GrpcKey = model.GrpcKey;
            _khanoumiPaymentSetting.GrpcPassword = model.GrpcPassword;
            _khanoumiPaymentSetting.AdditionalFee = model.AdditionalFee;
            _khanoumiPaymentSetting.AdditionalFeePercentage = model.AdditionalFeePercentage;

            await _settingService.ClearCache();
            await _settingService.SaveSetting(_khanoumiPaymentSetting);
            return View("~/Plugins/Payments.Khanoumi/Views/PaymentKhanoumi/Configure.cshtml", model);

        }
    }
}