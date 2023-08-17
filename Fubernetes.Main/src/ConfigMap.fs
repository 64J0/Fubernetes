namespace Fubernetes

open YamlDotNet.Serialization
open YamlDotNet.Serialization.NamingConventions

type Metadata = { Name: string; Namespace: string }

type ConfigMapConstructor =
    { ApiVersion: string
      Kind: string
      Metadata: Metadata
      Data: Map<string, string>
      BinaryData: Map<string, string>
      Immutable: bool }

    static member Default =
        { ApiVersion = "v1"
          Kind = "ConfigMap"
          Metadata =
            { Name = "default-configmap"
              Namespace = "default" }
          Data = Map.empty
          BinaryData = Map.empty
          Immutable = false }

// https://kubernetes.io/docs/concepts/configuration/configmap/
type ConfigMap(constructor: ConfigMapConstructor) =
    member private this.getSerializer() : ISerializer =
        SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build()

    member this.toYamlBuffer() =

        let serializer = this.getSerializer ()

        serializer.Serialize constructor
