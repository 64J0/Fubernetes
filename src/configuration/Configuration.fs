namespace FsharpK8s

open System
open System.IO

module Configuration =
    type Configuration =
        { OutDir: string
          OutFilename: string
          Resources: List<string> }

    let private parseResources (resources: List<string>) =
        resources
        |> List.reduce (fun (acc: string) (el: string) -> $"{acc}\n---\n{el}")

    let buildYaml (config: Configuration) =
        let parsedResources = parseResources config.Resources

        let destinationPathExists () = Directory.Exists(config.OutDir)

        let createDir () =
            if not (destinationPathExists()) then
                printfn "here 1111"
                let file = File.Create(config.OutDir)
                file.Close()
                
        createDir()
        File.WriteAllText(config.OutDir, parsedResources)
