using System.Diagnostics;

namespace Vardirsoft.XApp.Components
{
    public abstract class TextComponent : LayoutComponent
    {
        protected string text;

        public virtual string Text
        {
            [DebuggerStepThrough]
            get => text;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref text, value, false);
        }

        public TextComponent(string id) : base(id)
        {
            
        }
    }
}