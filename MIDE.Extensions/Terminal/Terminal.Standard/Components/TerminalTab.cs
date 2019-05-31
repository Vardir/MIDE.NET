using MIDE.API.Components;
using MyCMD.Kernel.Commands;
using MIDE.API.Extensibility;
using System.Collections.ObjectModel;

namespace Terminal.Components
{
    public class TerminalTab : Tab, IModuleExecutionListener
    {
        private TerminalExtension baseExtension;
        private TerminalModule terminal;

        public ObservableCollection<string> Lines { get; }

        public TerminalTab(string id, TerminalExtension extension, bool allowDuplicates = false) : base(id, allowDuplicates)
        {
            baseExtension = extension;
            Lines = new ObservableCollection<string>();
            terminal = extension.GetModule<TerminalModule>("terminal");
        }

        public void Execute(string query) => terminal.Execute(query, this);
        public void ReceiveResult(object context)
        {
            if (context is ExecutionResult exr)
            {
                if (exr.isEmpty)
                    Lines.Add("> ");
                else if (exr.isSuccessful)
                    Lines.Add($"> {exr.result}");
                else
                    Lines.Add($"!> {exr.errorMessage}");
            }
            else
                Lines.Add($"> {context}");
        }
        public void ReceiveStatus(int completion, object context)
        {
            if (completion >= 0)
                Lines.Add($"> {localization["t:progress"]} {completion}.\n{context}");
            else
                Lines.Add($"> {context}");
        }

        protected override Tab Create(string id, Toolbar toolbar, bool allowDuplicates)
        {
            TerminalTab clone = new TerminalTab(id, baseExtension, allowDuplicates);
            foreach (var item in toolbar.Items)
            {
                clone.TabToolbar.AddChild(item.Clone());
            }
            return clone;
        }
    }
}