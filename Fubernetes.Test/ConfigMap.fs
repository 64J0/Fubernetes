module Tests.ConfigMap

open Expecto
open Expecto.Flip
open Fubernetes

let verifyConfigMap () =
    let expected =
        """apiVersion: v1
kind: ConfigMap
metadata:
  name: test-configmap
  namespace: default
data:
  config-1: value-1
  config-2: value-2
binaryData: {}
immutable: false
"""

    let defaultConfigMap = ConfigMapConstructor.Default

    let configMapConstructor =
        { defaultConfigMap with
            Metadata =
                { Name = "test-configmap"
                  Namespace = "default" }
            Data = [ "config-1", "value-1"; "config-2", "value-2" ] |> Map.ofList }

    let configMap = new ConfigMap(configMapConstructor)

    let builtYaml = configMap.toYamlBuffer ()
    Expect.equal "ConfigMap yaml should be valid" expected builtYaml

let configMapTestsList: Test =
    testList "configMap tests" [ testCase "Check a valid ConfigMap yaml" <| verifyConfigMap ]
    |> testLabel "configMap"
