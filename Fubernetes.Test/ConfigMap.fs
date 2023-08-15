module Tests.ConfigMap

open Expecto
open Expecto.Flip
open Fubernetes.Resources

let verifyConfigMap () =
    let expected =
        """apiVersion: v1
kind: ConfigMap
metadata:
    name: configmap-01
    namespace: default
data: 
    config-1: value-1
    config-2: value-2
binaryData: 
immutable: false"""

    let configMap =
        new ConfigMap.ConfigMap(
            { Name = "configmap-01"
              Namespace = "default"
              Data = [ ("config-1", "value-1"); ("config-2", "value-2") ]
              BinaryData = []
              Immutable = false }
        )

    let builtYaml = configMap.toYamlBuffer ()
    Expect.equal "ConfigMap yaml should be valid" expected builtYaml

let configMapTestsList: Test =
    testList "configMap tests" [ testCase "Check a valid ConfigMap yaml" <| verifyConfigMap ]
    |> testLabel "configMap"
