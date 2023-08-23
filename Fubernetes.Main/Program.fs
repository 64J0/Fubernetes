open Fubernetes.ConfigMap

[<EntryPoint>]
let main (argv: string array) =
    let configMapConstructor =
        { ConfigMapConstructor.Default with
            Metadata =
                { Name = "test-configmap"
                  Namespace = "default" }
            Data = [ "config-1", "value-1"; "config-2", "value-2" ] |> Map.ofList }

    let configMapYaml = new ConfigMap(configMapConstructor)
    let serializedConfigMap = configMapYaml.toYamlBuffer ()

    printfn "Serialized ConfigMap: %A" serializedConfigMap

    0 // return an integer exit code
