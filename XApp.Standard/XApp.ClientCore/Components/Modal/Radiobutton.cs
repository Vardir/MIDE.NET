namespace Vardirsoft.XApp.Components
{
    public class RadioButton : CheckBox
    {
        public RadioButtonGroup Group { get; set; }

        public RadioButton(string id) : base(id) {}

        protected override void OnCheckedChanged()
        {
            if (IsChecked && Group != null)
            {
                Group.SetButton(this);
            }
        }
    }
}