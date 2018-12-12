using System;
using System.Collections.Generic;

namespace MIDE.Standard.API.Components
{
    public class RadioButtonGroup : IApplicationComponent
    {
        private readonly List<RadioButton> radioButtons;

        public string Id { get; }

        public RadioButtonGroup(string id)
        {
            Id = id;
            radioButtons = new List<RadioButton>();
        }

        public void SetButton(RadioButton radiobutton)
        {
            if (radiobutton.Group != this)
                throw new ArgumentException("The given radio-button does not belong to the group");
            for (int i = 0; i < radioButtons.Count; i++)
            {
                if (radioButtons[i].Id != radiobutton.Id)
                    radiobutton.IsChecked = false;
            }
        }

        public void AddButton(RadioButton radioButton)
        {
            radioButton.Group = null;
            radioButton.IsChecked = false;
            radioButton.Group = this;
            radioButtons.Add(radioButton);
        }
    }
}