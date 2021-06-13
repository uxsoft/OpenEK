using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using Microsoft.FSharp.Core;
using OpenEK.Core;
using OpenEK.Core.Native;
using OpenEK.Windows.Extensions;

namespace OpenEK.Windows.ViewModels
{
    public class LightsViewModel
    {
        public LightsViewModel()
        {
            Effects = Enum.GetNames(typeof(LedMode)).ToList();

            if (Core.App.Device.Api.IsConnected)
                ManagerOnOnConnected(this, null);
            else Core.App.Device.OnConnected += ManagerOnOnConnected;

            (SelectedBrightness as INotifyPropertyChanged).PropertyChanged += SelectedBrightness_PropertyChanged;
            (SelectedColor as INotifyPropertyChanged).PropertyChanged += SelectedColor_PropertyChanged;
            (SelectedEffect as INotifyPropertyChanged).PropertyChanged += SelectedEffect_PropertyChanged;
            (SelectedSpeed as INotifyPropertyChanged).PropertyChanged += SelectedSpeed_PropertyChanged;
        }
        public List<string> Effects { get; set; }
        public State<string> SelectedEffect { get; set; } = new("");
        public State<Color> SelectedColor { get; set; } = new(Colors.White);
        public State<byte> SelectedSpeed { get; set; } = new(0);
        public State<byte> SelectedBrightness { get; set; } = new(99);

        private void SelectedSpeed_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var command = EkCommand.NewSetLedSpeed(SelectedSpeed.Value);
            Core.App.Device.Send(command);
        }

        private void SelectedEffect_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var command = EkCommand.NewSetLedMode(Enum.Parse<LedMode>(SelectedEffect.Value));
            Core.App.Device.Send(command);
        }

        private void SelectedColor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var command = EkCommand.NewSetLedColor(
                        System.Drawing.Color.FromArgb(
                            SelectedColor.Value.A,
                            SelectedColor.Value.R,
                            SelectedColor.Value.G,
                            SelectedColor.Value.B));
            Core.App.Device.Send(command);
        }

        private void SelectedBrightness_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var command = EkCommand.NewSetLedBrightness(SelectedBrightness.Value);
            Core.App.Device.Send(command);
        }

        void ManagerOnOnConnected(object sender, Unit args)
        {
            var led = Core.App.Device.Api.GetLed();
            if (led.IsSome())
            {
                SelectedEffect.Value = led.Value.Mode.ToString() ?? "";
                SelectedColor.Value = Color.FromRgb(led.Value.Red, led.Value.Green, led.Value.Blue);
            }
        }
    }
}