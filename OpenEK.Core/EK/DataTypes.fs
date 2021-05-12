namespace OpenEK.Core.Native

open System.Drawing
open System.Runtime.InteropServices

type FanData =
    { Model: uint16
      RatedSpeed: uint16
      Speed: uint16
      RatedPower: uint16
      Power: uint16
      Load: uint16
      Pwm: uint16 }

[<StructLayout(LayoutKind.Sequential, Size = 1)>]
type LedColor =
    struct end

type LedMode =
    | Off = 0uy
    | Fixed = 1uy
    | Breathing = 2uy
    | Fading = 3uy
    | Marquee = 4uy
    | CoveringMarquee = 5uy
    | Pulse = 6uy
    | SpectrumWave = 7uy
    | Alternating = 8uy
    | Candle = 9uy

type LedData =
    { Model: byte
      Mode: LedMode
      Color: byte
      Speed: byte
      Brightness: byte
      Red: byte
      Green: byte
      Blue: byte
      RgbReload: byte
      LastMode: LedMode
      LastColor: byte
      LastSpd: byte
      LastBrightness: byte
      LastRed: byte
      LastGreen: byte
      LastBlue: byte
      Reserve0: byte }

type SensorsData =
    { TempModel1: uint16
      Temp1: uint16
      TempModel2: uint16
      Temp2: uint16
      TempModel3: uint16
      Temp3: uint16
      FlowModel: uint16
      FlowValue: uint16
      LevelModel: uint16
      LevelValue: uint16 }

type EkCommand =
    | SetFansPwm of uint16
    | SetPumpPwm of uint16 
    | SetLedColor of Color
    | SetLedMode of LedMode
    | SetLedSpeed of byte