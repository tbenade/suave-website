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
open Suave.Log
open Suave.Http.Files             

printfn "initializing script..."

let config = 
    let port = System.Environment.GetEnvironmentVariable("PORT")
    { defaultConfig with 
        logger = Logging.Loggers.saneDefaultsFor Logging.LogLevel.Verbose
        bindings=[ (if port = null then HttpBinding.mk' HTTP  "127.0.0.1" 8080
                    else HttpBinding.mk' HTTP  "0.0.0.0" (int32 port)) ] }

let app : WebPart = 
    choose [
       pathRegex "(.*?)\.(dll|mdb|log)$" >>= FORBIDDEN "Access denied.";
       GET >>= choose [ path "/" >>= file "index.html"; browseHome ];
       NOT_FOUND "Found no handlers." 
    ] >>= log logger log_format
    

startWebServer config app
printfn "exiting server..."


