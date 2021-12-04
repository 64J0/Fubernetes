namespace FsharpK8s

open System
open System.IO

module Configuration =
    type Configuration =
        { OutPath: string
          Resources: List<string> }

    let private parseResources (resources: List<string>) : string =
        resources
        |> List.reduce (fun (acc: string) (el: string) -> $"{acc}\n---\n{el}")

    let buildYaml (config: Configuration) : unit =
        let directory = Path.GetDirectoryName(config.OutPath)
        let destinationPathExists () = Directory.Exists(config.OutPath)
        
        let createPath () =
            if not (destinationPathExists()) then
                Directory.CreateDirectory(directory) |> ignore
                let file = File.Create(config.OutPath)
                file.Close()
                
        createPath()
        
        let parsedResources = parseResources config.Resources
        File.WriteAllText(config.OutPath, parsedResources)
