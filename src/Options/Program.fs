module Options

open System
open Fable.Core.JsInterop
open Fable.Core.JS
open Fable.React
open Fable.React.Props
open Elmish
open Elmish.React
open Shared

type Model =
    { Value: string
      DefaultCounter: Counter }

type Msg =
    | UpdateValue of string
    | SaveValue
    | Failure of string

let init (defaultCounter: Counter): Model * Cmd<Msg> =
    let model =
        { Value = String.Empty
          DefaultCounter = defaultCounter }

    let cmd = []
    model, cmd

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with
    | UpdateValue value ->
        let model' = { model with Value = value }
        model', []
    | SaveValue ->
        let counter' =
            match Int32.TryParse model.Value with
            | (true, v) -> { Count = v }
            | _ -> Counter.Default
        let model' =
            { model with
                  Value = String.Empty
                  DefaultCounter = counter' }

        let cmd = Cmd.OfFunc.attempt Counter.save model'.DefaultCounter (string >> Failure)
        model', cmd
    | Failure ex ->
        console.error ex
        model, []

let view (model: Model) (dispatch: Msg -> unit): ReactElement =
    let onEnter: DOMAttr =
        function
        | (ev: Browser.Types.KeyboardEvent) when ev.keyCode = 13.0 -> dispatch SaveValue
        | _ -> ()
        |> OnKeyDown

    let onInput: DOMAttr =
        OnInput(fun e ->
            !!e.target?value
            |> UpdateValue
            |> dispatch)

    let onClick (msg: Msg): DOMAttr = OnClick(fun _ -> dispatch msg)

    div []
        [ str <| sprintf "Default Count: %d" model.DefaultCounter.Count
          br []
          input
              [ onEnter
                onInput
                valueOrDefault model.Value ]
          button [ onClick SaveValue ] [ str "Save" ] ]

Program.mkProgram (Counter.loadWithDefault >> init) update view
|> Program.withReactBatched "options"
|> Program.run
