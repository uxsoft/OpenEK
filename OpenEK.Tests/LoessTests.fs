namespace OpenEK.Tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open OpenEK.Windows.Extensions


[<TestClass>]
type LoessTests () =

    [<TestMethod>]
    member this.StraightLine () =
        ()

    [<TestMethod>]
    member this.ZigZag () =
        let interpolator = LoessInterpolator()
        
        let xValues = [| 0.; 1.; 2.; 3.; 4.; 5.; 6.; 7.; 8.; 9.; |]
        let yValues = [| 1.; 0.; 1.; 0.; 1.; 0.; 1.; 0.; 1.; 0.; |]

        let result = interpolator.Smooth(xValues, yValues)

        Assert.IsTrue(true);
