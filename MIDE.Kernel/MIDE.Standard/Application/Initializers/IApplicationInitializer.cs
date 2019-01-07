namespace MIDE.Application.Initializers
{
    public interface IApplicationInitializer
    {
        void Execute(AppKernel appKernel);
    }
}