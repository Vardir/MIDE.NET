using NodeGraphs.Components;
using System.Windows.Controls;

namespace NodeGraphs.WPF
{
    /// <summary>
    /// Interaction logic for TestControl.xaml
    /// </summary>
    public partial class TestControl : UserControl
    {
        public TestControl()
        {
            InitializeComponent();
            canvas.Model.AddChild(new LabelNode("lbl"));
        }
    }
}
