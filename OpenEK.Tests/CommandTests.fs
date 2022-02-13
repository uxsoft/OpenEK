namespace OpenEk.Tests

open System
open System.Text.Json
open System.Text.Json.Serialization
open Microsoft.VisualStudio.TestTools.UnitTesting
open OpenEk.Core.EK
open OpenEk.Avalonia.Types

[<TestClass>]
type CommandTests () =

    [<TestMethod>]
    member this.SetFanSpeed () =
        Device.connect () |> ignore
        let fans = Device.getFans false
        for fan in fans do
            Device.setFan fan.Value (byte fan.Key) (uint16 10) |> ignore
            
        