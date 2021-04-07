using Grand.Plugin.Payments.Khanoumi.Models.Saman;
using System;
using TokenResponse = KhanoumiPyamentGrpc.TokenResponse;

namespace Grand.Plugin.Payments.Khanoumi.Helpers
{
    public class SamanHelper
    {
        public static string ProduceRedirectUrl(string StoreLocation, TokenResponse tokenResponse)
        {
            string urlProduced = (tokenResponse?.Status != null && tokenResponse?.Status == 200 ?
                    string.Concat($"{tokenResponse.BankUrl}/?token={tokenResponse.Token}")
                    : string.Concat(StoreLocation, "Plugins/PaymentKhanomi/ErrorHandler", "?Error=",
                        SamanHelper.StatusToMessage(tokenResponse.Status, tokenResponse.ErrorCode).Message)
                );
            var uri = new Uri(urlProduced);
            return uri.AbsoluteUri;
        }

        public static StatusToResult StatusToMessage(int? status, int? errorCode)
        {
            StatusToResult statusToResult = new StatusToResult();
            if (status == 1)
            {
                statusToResult.IsOk = true;
                statusToResult.Message = "توکن با موفقیت ارسال شد.";
                return statusToResult;
            }
            switch (status, errorCode)
            {
                case (-1, 5):
                    statusToResult.Message = "پارامترهای ارسالی نامعتبر است.";
                    break;
                case (-1, 8):
                    statusToResult.Message = "آدرس سرور پذیرنده نامعتبر است.";
                    break;
                case (-1, 10):
                    statusToResult.Message = "توکن ارسال شده یافت نشد.";
                    break;
                case (-1, 11):
                    statusToResult.Message = "با این شماره ترمینال فقط تراکنش های توکنی قابل پرداخت هستند.";
                    break;
                case (-1, 12):
                    statusToResult.Message = "شماره ترمینال ارسال شده یافت نشد.";
                    break;
                default:
                    statusToResult.Message = string.Concat("درخواست نا معتبر", "-", status);
                    break;
            }
            return statusToResult;
        }
    }
}
