using System;
using System.Collections.Generic;

namespace Vardirsoft.XApp.Components
{
    public class RadioButtonGroup : ApplicationComponent
    {
        private readonly List<RadioButton> _radioButtons;

        public RadioButtonGroup(string id) : base(id)
        {
            _radioButtons = new List<RadioButton>();
        }

        public void SetButton(RadioButton radiobutton)
        {
            if (radiobutton.Group != this)
                throw new ArgumentException("The given radio-button does not belong to the group");
            
            for (var i = 0; i < _radioButtons.Count; i++)
            {
                if (_radioButtons[i].Id != radiobutton.Id)
                {
                    radiobutton.IsChecked = false;
                }
            }
        }

        public void AddButton(RadioButton radioButton)
        {
            radioButton.Group = null;
            radioButton.IsChecked = false;
            radioButton.Group = this;
            _radioButtons.Add(radioButton);
        }
    }
}