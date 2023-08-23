namespace Fubernetes

module ConfigMap =

    type ConfigMapConstructor =
        { ApiVersion: string
          Kind: string
          Metadata: Shared.Metadata
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
        inherit Shared.BaseManifest(constructor)
