namespace FsharpK8s

open System
open System.IO

module Configuration =
    type Configuration =
        { OutDir: string
          Resources: List<string> }

    let private parseResources (resources: List<string>) =
        resources
        |> List.reduce (fun (acc: string) (el: string) -> $"{acc}\n---\n{el}")

    let buildYaml (config: Configuration) =
        let parsedResources = parseResources config.Resources
        
        File.WriteAllText(config.OutDir, parsedResources)
