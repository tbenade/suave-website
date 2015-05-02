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
open Suave.Web             // for config
open Suave.Types             

printfn "initializing script..."

let app : WebPart = 
    choose [
       log logger log_format >>= never;
       url_regex "(.*?)\.(dll|mdb|log)$" >>= FORBIDDEN "Access denied.";
       GET >>= choose [ url "/" >>= file "index.html"; browse ];
       NOT_FOUND "Found no handlers." 
    ]
    

startWebServer config app
printfn "exiting server..."


