using MIDE.API.Components;
using MIDE.API.Extensibility;
using MIDE.Application.Attrubites;

namespace KernelTemplate
{
    [Dependency(Version = "x.x.x", Type = DependencyType.ApplicationKernel)]
    [ExtensionProperties(Version = "1.0.0")]
    public class TemplateExtension : AppExtension
    {
        public TemplateExtension(string id) : base(id)
        {

        }

        protected override void RegisterMenuItems(IMenuConstructionContext context)
        {
            //context.AddItem("path", new MenuButton("button", 0));
        }
        protected override void RegisterModules()
        {
            //RegisterModule(new TModule(x));
        }
    }
}