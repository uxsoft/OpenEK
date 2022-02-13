module OpenEk.Avalonia.Program

open System
open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Presenters
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Markup.Xaml.Styling
open Avalonia.Platform
open Avalonia.Themes.Fluent
open FUI.Avalonia.DSL
open FUI.HotReload.HotReload

type Hot() as this =
    inherit ContentPresenter()
    
    let model = Types.init()
    do this.Content <- App.view model
    
    interface IHotReloadable with
        member this.Accept(old) =
            // hydrated accepts current
            let m = transferModel (old.GetModel()) Types.init // Transfer existing model data to new model type
            let v = App.view m // Build UI using new view
            old.SetView v // Use old container to host new UI
            
        member this.GetModel() =
            box model
                    
        member this.SetView(view) =
            this.Content <- view

let createMainWindow () =    
    let hot = Hot()
    let disposables : IDisposable list = [] //hotReload hot AvaloniaScheduler.Instance
    
    Window {
        height 800.
        width 800.
        title "Open EK Connect"
        transparencyLevelHint WindowTransparencyLevel.Transparent
        systemDecorations SystemDecorations.Full
        extendClientAreaToDecorationsHint true
        extendClientAreaTitleBarHeightHint -1.
        extendClientAreaChromeHints ExtendClientAreaChromeHints.PreferSystemChrome
        background null //(SolidColorBrush(Colors.Transparent))
        onWindowClosed (fun _ -> for d in disposables do d.Dispose())

        hot
    }
        
type App() =
    inherit Application()
    override this.Initialize() =
        this.Styles.Add(FluentTheme(baseUri = null, Mode = FluentThemeMode.Dark))
        this.Styles.Add(StyleInclude(baseUri = null, Source = Uri("avares://OpenEk.Avalonia/Styles/SideBar.xaml")))

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->            
            desktopLifetime.MainWindow <- createMainWindow()
        | _ -> ()

[<EntryPoint>]
let main(args: string[]) =
    AppBuilder
        .Configure<App>()
        .UsePlatformDetect()
        .UseSkia()
        .With(SkiaOptions(MaxGpuResourceSizeBytes = 8096000L) :> obj)
        .With(Win32PlatformOptions(AllowEglInitialization = true))
        .StartWithClassicDesktopLifetime(args)