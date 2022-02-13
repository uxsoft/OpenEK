namespace OpenEk.Tests

open System
open System.Text.Json
open System.Text.Json.Serialization
open Microsoft.VisualStudio.TestTools.UnitTesting
open OpenEk.Core.EK
open OpenEk.Avalonia.Types

[<TestClass>]
type ModelTests () =

    [<TestMethod>]
    member this.IsSerializable () =
        ()