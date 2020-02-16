#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target 
nuget Fake.JavaScript.Yarn //"

#if !FAKE
#load ".fake/build.fsx/intellisense.fsx"
#r "netstandard" // Temp fix for https://github.com/dotnet/fsharp/issues/5216
#endif

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.JavaScript

module Webpack =
  let webpackProd() = 
    let cmd = "webpack --config webpack/webpack.config.prod.js"
    Yarn.exec cmd id 
  let webpackDev() = 
    let cmd = "webpack --config webpack/webpack.config.dev.js"
    Yarn.exec cmd id
  let webpackWatch() =
    let cmd = "webpack --config webpack/webpack.config.dev.js --watch"
    Yarn.exec cmd id

Target.initEnvironment ()

Target.create "Clean" (fun _ ->
  !! "src/**/bin"
  ++ "src/**/obj"
  |> Shell.cleanDirs 
)

Target.create "YarnInstall" (fun _ ->
  Yarn.install id
)

Target.create "Setup" ignore 

Target.create "DotnetBuild" (fun _ ->
  !! "src/**/*.*proj"
  |> Seq.iter (DotNet.build id)
)

Target.create "WebpackDev" (fun _ ->
  Webpack.webpackDev()
)

Target.create "WebpackProd" (fun _ -> 
  Webpack.webpackProd()
)

Target.create "WebpackWatch" (fun _ ->
  Webpack.webpackWatch()
)

Target.create "All" ignore

"Clean"
  ==> "YarnInstall"
  ==> "Setup"

"Setup"
  ==> "WebpackProd"

"All"
  <== [ "WebpackProd" ]

Target.runOrDefault "Setup"
