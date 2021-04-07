using System;
using System.Collections.Generic;
using System.Text;
using Grand.Plugin.Payments.Khanoumi.Models.Zarinpal;

namespace Grand.Plugin.Payments.Khanoumi.Helpers
{
    public class ZarinpalHelper
    {
        public static string ProduceRedirectUrl(string StoreLocation, int? status, bool UseSandbox, string Authority, string ZarinGate)
        {
            string urlProduced = (status.HasValue && status.Value == 100 
                    ? string.Concat($"https://{(UseSandbox ? "sandbox" : "www")}.zarinpal.com/pg/StartPay/", Authority, ZarinGate)
                    : string.Concat(StoreLocation, "Plugins/PaymentZarinpal/ErrorHandler", "?Error=", StatusToMessage(status).Message));
            
            var uri = new Uri(urlProduced);
            return uri.AbsoluteUri;
        }

        public static StatusToResult StatusToMessage(int? _status)
        {
            StatusToResult statusToResult = new StatusToResult();
            switch (_status)
            {
                case -1:
                    statusToResult.Message = "اطلاعات ارسال شده ناقص است.";
                    break;
                case -2:
                    statusToResult.Message = "آی پی و مرچنت کد پذیرنده صحیح نمی باشد";
                    break;
                case -3:
                    statusToResult.Message = "امکان پرداخت مبلغ درخواست شده میسر نمی باشد";
                    break;
                case -4:
                    statusToResult.Message = "سطح تایید پذیرنده پایین تر از نقره ای می باشد";
                    break;
                case -11:
                    statusToResult.Message = "درخواست مورد نظر یافت نشد.";
                    break;
                case -12:
                    statusToResult.Message = "امکان ویرایش درخواست میسر نمی باشد";
                    break;
                case -21:
                    statusToResult.Message = "انصراف از خرید";
                    break;
                case -22:
                    statusToResult.Message = "تراکنش ناموفق می باشد.";
                    break;
                case -33:
                    statusToResult.Message = "مبلغ تراکنش با مبلغ پرداخت شده مطابقت ندارد.";
                    break;
                case -34:
                    statusToResult.Message = "سقف تقسیم تراکنش از لحاظ تعداد یا رقم عبور نموده است";
                    break;
                case -40:
                    statusToResult.Message = "اجازه دسترسی به متد مبروطه وجود ندارد";
                    break;
                case -41:
                    statusToResult.Message = "اجازه ارسال شده مربوط به Additional Data غیر معتبر می باشد";
                    break;
                case -42:
                    statusToResult.Message = "مدت زمان معتبر طول شناسه پرداخت باید بین 30 دقیقه تا 45 روز می باشد";
                    break;
                case -54:
                    statusToResult.Message = "درخواست مورد نظر آرشیو شده است";
                    break;
                case 100:
                    statusToResult.IsOk = true;
                    statusToResult.Message = "تراکنش با موفقیت انجام شد";
                    break;
                case 101:
                    statusToResult.IsOk = true;
                    statusToResult.Message = "تراکنش با موفقیت انجام شد و Payment Verification قبلا انجام شده است";
                    break;
                default:
                    statusToResult.Message = string.Concat("درخواست نا معتبر", "-", _status);
                    break;
            }
            return statusToResult;
        }
    }
}
