using System;
using MIDE.API.Components;

namespace MIDE.Application.Initializers
{
    public abstract class BaseMenuInitializer : IApplicationInitializer
    {
        protected readonly AppKernel appKernel;

        public BaseMenuInitializer(AppKernel appKernel)
        {
            this.appKernel = appKernel;
        }

        public void Execute(AppKernel appKernel)
        {
            if (appKernel == null)
                throw new ArgumentNullException(nameof(appKernel));
            PopulateMenu(appKernel.UIManager.ApplicationMenu);
        }

        protected abstract void PopulateMenu(IMenuConstructionContext applicationMenu);
    }
}