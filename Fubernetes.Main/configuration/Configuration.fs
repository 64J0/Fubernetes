namespace Fubernetes

open System.IO
open Fubernetes.Resources

module Configuration =
    type Configuration =
        { OutPath: string
          Resources: List<string> }

    let private createOutPath (outPath: string) =
        let directory = Path.GetDirectoryName(outPath)
        let destinationPathExists () = Directory.Exists(outPath)
        
        if not (destinationPathExists()) then
            Directory.CreateDirectory(directory) |> ignore
            let file = File.Create(outPath)
            file.Close()

    let private parseResources (resources: List<string>) : string =
        resources
        |> Shared.reduceIfNotEmpty (fun (acc: string) (el: string) -> $"{acc}\n---\n{el}")

    let createOutPathAndBuildYaml (config: Configuration) : unit =
        createOutPath (config.OutPath)
        
        let parsedResources = parseResources config.Resources
        File.WriteAllText(config.OutPath, parsedResources)
