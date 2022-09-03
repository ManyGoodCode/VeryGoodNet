using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using System.Web.Http.ValueProviders.Providers;

namespace WebApiContrib.ModelBinders
{
    public class MvcActionValueBinder : System.Web.Http.ModelBinding.DefaultActionValueBinder
    {
        private const string Key = "{5DC187FB-BFA0-462A-AB93-9E8036871EC8}";

        public override System.Web.Http.Controllers.HttpActionBinding GetBinding(System.Web.Http.Controllers.HttpActionDescriptor actionDescriptor)
        {
            MvcActionBinding actionBinding = new MvcActionBinding();
            System.Web.Http.Controllers.HttpParameterDescriptor[] parameters = actionDescriptor.GetParameters().ToArray();
            // 数组批量转化。传递函数方法当中Converter委托
            HttpParameterBinding[] binders = Array.ConvertAll(
                array: parameters,
                converter: DetermineBinding);

            actionBinding.ParameterBindings = binders;
            return actionBinding;
        }

        private HttpParameterBinding DetermineBinding(System.Web.Http.Controllers.HttpParameterDescriptor parameter)
        {
            System.Web.Http.HttpConfiguration config = parameter.Configuration;
            System.Web.Http.ModelBinding.ModelBinderAttribute attr = new ModelBinderAttribute();

            System.Web.Http.ModelBinding.ModelBinderProvider provider = attr.GetModelBinderProvider(config);
            System.Web.Http.ModelBinding.IModelBinder binder = provider.GetBinder(config, parameter.ParameterType);

            List<ValueProviderFactory> vpfs = new List<ValueProviderFactory>(
            attr.GetValueProviderFactories(config))
            {
                new BodyValueProviderFactory()
            };

            return new ModelBinderParameterBinding(parameter, binder, vpfs);
        }

        private class MvcActionBinding : System.Web.Http.Controllers.HttpActionBinding
        {
            public override Task ExecuteBindingAsync(
                HttpActionContext actionContext, 
                CancellationToken cancellationToken)
            {
                HttpRequestMessage request = actionContext.ControllerContext.Request;
                HttpContent content = request.Content;
                if (content != null)
                {
                    FormDataCollection fd = content.ReadAsAsync<FormDataCollection>().Result;
                    if (fd != null)
                    {
                        IValueProvider vp = new NameValuePairsValueProvider(fd, CultureInfo.InvariantCulture);
                        request.Properties.Add(Key, vp);
                    }
                }

                return base.ExecuteBindingAsync(actionContext, cancellationToken);
            }
        }

        private class BodyValueProviderFactory : System.Web.Http.ValueProviders.ValueProviderFactory
        {
            public override IValueProvider GetValueProvider(HttpActionContext actionContext)
            {
                object vp;
                actionContext.Request.Properties.TryGetValue(Key, out vp);
                return (IValueProvider)vp;
            }
        }
    }
}
