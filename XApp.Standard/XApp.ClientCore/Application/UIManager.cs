using System;
using System.Reflection;

using XApp.Components;

namespace XApp.Application
{
    public abstract class UIManager
    {
        public Menu ApplicationMenu { get; }

        public UIManager()
        {
            ApplicationMenu = new Menu("menu");
        }

        public abstract void RegisterUIExtension(object obj);
        public abstract void RegisterUIExtension(string path);
        public abstract void RegisterUIExtension(Type type);
        public abstract void RegisterUIExtension(Assembly assembly);

        public (DialogResult dialogResult, T data) OpenDialog<T>(BaseDialogBox<T> dialogBox)
        {
            OpenDialog_Impl(dialogBox);
            
            return (dialogBox.SelectedResult, dialogBox.GetData());
        }
        
        protected abstract (DialogResult result, T value) OpenDialog_Impl<T>(BaseDialogBox<T> dialogBox);
    }
}