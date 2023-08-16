open Fubernetes.Resources

[<EntryPoint>]
let main argv =
    let defaultConfigMap = ConfigMapTest.ConfigMapConstructor.Default

    let configMap =
        { defaultConfigMap with
            Metadata =
                { Name = "test-configmap"
                  Namespace = "default" }
            Data = [ "config-1", "value-1"; "config-2", "value-2" ] |> Map.ofList }

    let configMapYaml = new ConfigMapTest.ConfigMap(configMap)
    configMapYaml.toYamlBuffer ()

    0 // return an integer exit code
