using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

            if (EK.Manager.Bus.IsConnected)
                ManagerOnOnConnected(this, null);
            else EK.Manager.OnConnected += ManagerOnOnConnected;

            PropertyChanged += OnPropertyChanged;
        }

        void ManagerOnOnConnected(object sender, Unit args)
        {
            var led = EK.Manager.Bus.GetLed();
            if (led.IsSome())
            {
                SelectedEffect = led.Value.Mode.ToString() ?? "";
                SelectedColor = Color.FromRgb(led.Value.Red, led.Value.Green, led.Value.Blue);
            }
        }

        void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            EkCommand? command = e.PropertyName switch
            {
                nameof(SelectedEffect) =>
                    EkCommand.NewSetLedMode(
                        Enum.Parse<LedMode>(SelectedEffect)),
                nameof(SelectedColor) =>
                    EkCommand.NewSetLedColor(
                        System.Drawing.Color.FromArgb(
                            SelectedColor.A,
                            SelectedColor.R,
                            _selectedColor.G,
                            SelectedColor.B)),
                nameof(SelectedBrightness) =>
                    EkCommand.NewSetLedBrightness(SelectedBrightness),
                nameof(SelectedSpeed) =>
                    EkCommand.NewSetLedSpeed(SelectedSpeed),
                _ => null
            };

            if (command != null)
                EK.Manager.Send(command);
        }

        public List<string> Effects { get; set; }

        [GeneratedProperty] string _selectedEffect = "";
        [GeneratedProperty] Color _selectedColor = Colors.White;
        [GeneratedProperty] byte _selectedSpeed = 0;
        [GeneratedProperty] byte _selectedBrightness = 99;
    }
}