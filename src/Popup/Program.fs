module Popup

open Fable.Core
open Fable.React
open Fable.React.Props
open Fable.Import
open Browser.Dom
open Elmish
open Elmish.React
open Shared

type Model = Counter

type Msg =
    | Increment
    | Decrement
    | Failure of string

let init (counter: Counter): Counter * Cmd<Msg> =
    let model = counter
    let cmd = []
    model, cmd

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with
    | Increment ->
        let model' = { model with Count = model.Count + 1 }
        model', []
    | Decrement ->
        let model' = { model with Count = model.Count - 1 }
        model', []
    | Failure ex ->
        console.error ex
        model, []

let view (model: Model) (dispatch: Msg -> unit): ReactElement =
    model.Count
    |> string
    |> JS.BrowserAction.setBadgeText
    div []
        [ str <| sprintf "Count : %d" model.Count
          br []
          button [ OnClick(fun _ -> dispatch Increment) ] [ str "increment" ]
          button [ OnClick(fun _ -> dispatch Decrement) ] [ str "decrement" ] ]

Program.mkProgram (Counter.loadWithDefault >> init) update view
|> Program.withReactBatched "popup"
|> Program.run
