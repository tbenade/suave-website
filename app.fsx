#if BOOTSTRAP
System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
if not (System.IO.File.Exists "paket.exe") then let url = "https://github.com/fsprojects/Paket/releases/download/0.27.2/paket.exe" in use wc = new System.Net.WebClient() in let tmp = System.IO.Path.GetTempFileName() in wc.DownloadFile(url, tmp); System.IO.File.Move(tmp,System.IO.Path.GetFileName url);;
#r "paket.exe"
Paket.Dependencies.Install (System.IO.File.ReadAllText "paket.dependencies")
#endif

//---------------------------------------------------------------------

#I "packages/Suave/lib/net40"
#r "packages/Suave/lib/net40/Suave.dll"

open System
open Suave                 // always open suave
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful // for OK-result
open Suave.Http.ServerErrors
open Suave.Http.RequestErrors
open Suave.Web             // for config
open Suave.Types
open Suave.Logging
open Suave.Http.Files             

printfn "initializing script..."

let logger = Loggers.saneDefaultsFor LogLevel.Verbose

let config = 
    let port = System.Environment.GetEnvironmentVariable("PORT")
    { defaultConfig with
        homeFolder = Some __SOURCE_DIRECTORY__
        logger = logger
        maxOps = 1000
        bindings=[ (if port = null then HttpBinding.mk' HTTP  "127.0.0.1" 8080
                    else HttpBinding.mk' HTTP  "0.0.0.0" (int32 port)) ] }

let app : WebPart = 
    choose [
       GET >>= choose [ path "/" >>= file "index.html"; browseHome ];
       NOT_FOUND "Found no handlers." 
    ] >>= log logger logFormat
    

startWebServer config app
printfn "exiting server..."


