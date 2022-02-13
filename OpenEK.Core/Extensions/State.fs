namespace OpenEk.Core

open System.ComponentModel

type State<'T>(initial: 'T) = 
    let onPropertyChanged = new Event<_,_>()
    let mutable value = initial
    
    member x.Value
      with get() = value
       and set(v) =
         value <- v
         onPropertyChanged.Trigger(x, PropertyChangedEventArgs(nameof x.Value))
    
    interface INotifyPropertyChanged with
      [<CLIEvent>]
      member x.PropertyChanged = onPropertyChanged.Publish

  