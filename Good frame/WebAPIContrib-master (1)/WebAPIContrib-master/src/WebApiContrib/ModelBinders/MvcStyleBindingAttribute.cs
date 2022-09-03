using System;
using System.Web.Http.Controllers;

namespace WebApiContrib.ModelBinders
{
    public class MvcStyleBindingAttribute :
        Attribute,
        System.Web.Http.Controllers.IControllerConfiguration
    {
        public void Initialize(
        System.Web.Http.Controllers.HttpControllerSettings controllerSettings,
        System.Web.Http.Controllers.HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Services.Replace(
                typeof(System.Web.Http.Controllers.IActionValueBinder), 
                new MvcActionValueBinder());
        }
    }
}
