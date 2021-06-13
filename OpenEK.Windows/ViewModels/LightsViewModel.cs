using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using OpenEK.Core;
using OpenEK.Core.EK;

namespace OpenEK.Windows.ViewModels
{
    public class LightsViewModel
    {
        public LightsViewModel()
        {
            Effects = Enum.GetNames(typeof(LedMode)).ToList();

            var ledColor = EKManager.deviceState.LedColor;
            
            SelectedEffect.Value = EKManager.deviceState.LedMode.ToString();
            SelectedColor.Value = Color.FromRgb(ledColor.R, ledColor.G, ledColor.B);
            SelectedBrightness.Value = ledColor.A;
            SelectedSpeed.Value = EKManager.deviceState.LedSpeed;
            
            (SelectedBrightness as INotifyPropertyChanged).PropertyChanged += SelectedColor_PropertyChanged;
            (SelectedColor as INotifyPropertyChanged).PropertyChanged += SelectedColor_PropertyChanged;
            (SelectedEffect as INotifyPropertyChanged).PropertyChanged += SelectedEffect_PropertyChanged;
            (SelectedSpeed as INotifyPropertyChanged).PropertyChanged += SelectedSpeed_PropertyChanged;
        }

        public List<string> Effects { get; set; }
        public State<string> SelectedEffect { get; set; } = new("");
        public State<Color> SelectedColor { get; set; } = new(Colors.White);
        public State<byte> SelectedSpeed { get; set; } = new(0);
        public State<byte> SelectedBrightness { get; set; } = new(99);

        void SelectedSpeed_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var command = Commands.EkCommand.NewSetLedSpeed(SelectedSpeed.Value);
            EKManager.queueCommand(command);
        }

        void SelectedEffect_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var command = Commands.EkCommand.NewSetLedMode(Enum.Parse<LedMode>(SelectedEffect.Value));
            EKManager.queueCommand(command);
        }

        void SelectedColor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var command = Commands.EkCommand.NewSetLedColor(
                System.Drawing.Color.FromArgb(
                    SelectedBrightness.Value,
                    SelectedColor.Value.R,
                    SelectedColor.Value.G,
                    SelectedColor.Value.B));
            EKManager.queueCommand(command);
        }
    }
}