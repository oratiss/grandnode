using Grand.Core.Configuration;
using Grand.Core.DependencyInjection;
using Grand.Core.TypeFinders;
using Microsoft.Extensions.DependencyInjection;

namespace Grand.Plugin.Payments.Khanoumi
{
    public class DependencyRegistrar : IDependencyInjection
    {
        public void Register(IServiceCollection serviceCollection, ITypeFinder typeFinder, GrandConfig config)
        {
            serviceCollection.AddScoped<KhanoumiPaymentProcessor>();
            serviceCollection.AddScoped<KhanoumiPaymentSetting>();
        }

        public int Order => 1;
    }
}
