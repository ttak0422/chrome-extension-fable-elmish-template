namespace JS

open Fable.Core

type IBrowserAction =
    abstract setBadgeText: string -> unit

module BrowserAction =

    [<Import("*", "./browserAction.js")>]
    let private native: IBrowserAction = jsNative

    let setBadgeText = native.setBadgeText
