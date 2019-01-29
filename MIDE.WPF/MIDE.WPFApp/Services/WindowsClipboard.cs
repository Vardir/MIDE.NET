using System.Windows;
using MIDE.API.Services;

namespace MIDE.WPFApp.Services
{
    public class WindowsClipboard : IClipboardProvider
    {
        private static WindowsClipboard instance;
        public static WindowsClipboard Instance => instance ?? (instance = new WindowsClipboard());

        private WindowsClipboard (){}

        public object Pop(API.Services.DataFormat dataFormat) => Clipboard.GetDataObject().GetData(dataFormat.ToString());
        
        public void Push(object obj, bool copy = true) => Clipboard.SetDataObject(obj, copy);
        public void Push(object obj, API.Services.DataFormat dataFormat) => Clipboard.SetData(dataFormat.ToString(), obj);
    }
}