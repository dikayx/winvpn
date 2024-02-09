using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinVPN.MVVM.ViewModel
{
    internal class SettingsViewModel
    {
        // Bindable Properties
        // These may be added in the future
        public bool Option1IsChecked { get; set; }
        public bool Option2IsChecked { get; set; }
        public bool Option3IsChecked { get; set; }

        public string Option1Text { get; set; } = "Option 1";
        public string Option2Text { get; set; } = "Option 2";
        public string Option3Text { get; set; } = "Option 3";

        public string AboutText { get; set; } =
            "The idea for this app comes\n" +
            "from 'Payload', the implementation\n" +
            "and improvements are made by me.";

        public SettingsViewModel()
        {
            
        }
    }
}
