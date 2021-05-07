using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Fonderie;
using Microsoft.FSharp.Core;
using OpenEK.Core.Native;
using OpenEK.Windows.Extensions;

namespace OpenEK.Windows.ViewModels
{
    public partial class LightsViewModel
    {
        public LightsViewModel()
        {
            Effects = Enum.GetNames(typeof(LedMode)).ToList();

            var led = EK.Manager.Bus.GetLed();
            if (led.IsSome())
            {
                _selectedEffect = led.Value.ToString() ?? "";
                _selectedColor = Color.FromRgb(led.Value.Red, led.Value.Green, led.Value.Blue);
            }
        }

        private Color _selectedColor = Colors.White;
        public List<string> Effects { get; set; } = new();

        private string _selectedEffect = "";
        public string SelectedEffect
        {
            get => _selectedEffect;
            set
            {
                _selectedEffect = value;
                // TODO use F# library to set effect
            }
        }
    }
}
