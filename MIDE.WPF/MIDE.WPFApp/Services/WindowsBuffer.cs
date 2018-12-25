using System.Windows;
using MIDE.API.Services;

namespace MIDE.WPFApp.Services
{
    public class WindowsBuffer : IBufferProvider
    {
        private static WindowsBuffer instance;
        public static WindowsBuffer Instance => instance ?? (instance = new WindowsBuffer());

        private WindowsBuffer (){}

        public object Pop() => null;

        public void Push(object value) { }
    }
}