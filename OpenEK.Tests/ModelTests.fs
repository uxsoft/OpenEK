namespace OpenEK.Tests

open System
open System.Text.Json
open System.Text.Json.Serialization
open Microsoft.VisualStudio.TestTools.UnitTesting
open OpenEK.Core.EK
open OpenEk.Avalonia.Types

[<TestClass>]
type ModelTests () =

    [<TestMethod>]
    member this.IsSerializable () =
        let compute =
            { CpuName = "cpu name"
              CpuTemperature = 10f
              GpuName = "gpu name"
              GpuTemperature = 20f }
        
        let fan : FanData =
            { Model = 1us
              RatedSpeed = 2us
              Speed = 3us
              RatedPower = 4us
              Power = 5us
              Load = 6us
              Pwm = 7us }

        let led : LedData =
            { Model = 1uy
              Mode = LedMode.SpectrumWave
              Color = 2uy
              Speed = 3uy
              Brightness = 4uy
              Red = 5uy
              Green = 6uy
              Blue = 7uy
              RgbReload = 8uy
              LastMode = LedMode.CoveringMarquee
              LastColor = 9uy
              LastSpd = 10uy
              LastBrightness = 11uy
              LastRed = 12uy
              LastGreen = 13uy
              LastBlue = 14uy
              Reserve0 = 15uy }

        let device =
            Commands.getState()
        
        let fans =
            [ 1, fan ] |> Map.ofList
        
        let expected =
            { CurrentPage = Page.Illumination
              Compute = compute
              Device = device }
        
        let options = JsonSerializerOptions()
        options.Converters.Add(JsonFSharpConverter())
        
        let json = JsonSerializer.Serialize(expected, options)
        let actual = JsonSerializer.Deserialize<Model>(json, options)
        
        Assert.AreEqual(expected, actual)