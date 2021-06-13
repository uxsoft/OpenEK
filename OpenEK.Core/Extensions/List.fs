module List

open System

let roll (count: int) (item: 'T) (col: 'T list) =
    let length = col |> List.length

    let tail =
        col |> List.skip (Math.Min(0, length + 1 - count))

    tail @ [ item ]
