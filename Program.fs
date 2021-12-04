// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open FsharpK8s.Configuration
open FsharpK8s.Resources

// SYSTEM SPECS:
//
// The user must define the output directory
// default value will be the current directory

[<EntryPoint>]
let main argv =
    let opaqueSecret = 
        new Secret.OpaqueSecret(
            { Name = "Secret01"
              Namespace = "default"
              Data = 
                [ ("key1", "value1") 
                  ("key2", "value2")
                  ("key3", "value3") ] })

    let resourceList = 
        [ opaqueSecret.toYamlBuffer() ]
    
    let outPath = "./prod/test.secret.yml"
    let config: Configuration = 
        { OutPath = outPath
          Resources = resourceList }

    buildYaml (config)
    0 // return an integer exit code