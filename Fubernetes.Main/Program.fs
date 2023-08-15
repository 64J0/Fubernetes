open Fubernetes.Resources

[<EntryPoint>]
let main argv =
    let defaultConfigMap = ConfigMapTest.ConfigMapConstructor.Default

    let configMap =
        { defaultConfigMap with
            Data = [ "config-1: value-1"; "config-2: value-2" ] }

    let configMapYaml = new ConfigMapTest.ConfigMap(configMap)
    configMapYaml.toYamlBuffer ()

    0 // return an integer exit code
