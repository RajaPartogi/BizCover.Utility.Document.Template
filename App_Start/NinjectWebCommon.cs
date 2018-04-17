using System;
using System.Web;
using BizCover.Common.Infrastructure.Logging;
using BizCover.Utility.Document.Template;
using BizCover.Utility.Document.Template.Services.Classes;
using BizCover.Utility.Document.Template.Services.Interfaces;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]
namespace BizCover.Utility.Document.Template
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);
            return kernel;
        }
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IGenerateDocumentService>().To<GenerateDocumentService>().InTransientScope();
            kernel.Bind<IFileService>().To<FileService>().InTransientScope();
            kernel.Bind<ILogger>().To<NLogLogger>().WithConstructorArgument(typeof(NLog.ILogger), context => NLog.LogManager.GetLogger("BizCover.Utility.Document.Template"));
        }
    }
}