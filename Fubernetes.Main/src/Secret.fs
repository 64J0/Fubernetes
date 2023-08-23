namespace Fubernetes

open Fubernetes

[<RequireQualifiedAccess>]
type SecretKind =
    | Opaque

type OpaqueSecretConstructor =
    { ApiVersion: string
      Kind: string
      Metadata: Shared.Metadata
      Type: SecretKind
      Data: Map<string, string> }

    static member Default =
        { ApiVersion = "v1"
          Kind = "Secret"
          Metadata =
            { Name = "default-opaquesecret"
              Namespace = "default" }
          Type = SecretKind.Opaque
          Data = Map.empty }

// https://kubernetes.io/docs/concepts/configuration/secret/
type OpaqueSecret(constructor: OpaqueSecretConstructor) =
    inherit Shared.BaseManifest (constructor)

// TODO
type ServiceAccountTokenConstructor = unit

// TODO
type DockerCfgConstructor = unit

// TODO
type DockerConfigJsonConstructor = unit

// TODO
type BasicAuthenticationConstructor = unit

// TODO
type SSHAuthConstructor = unit

// TODO
type TLSConstructor = unit

// TODO
type BootstrapTokenDataConstructor = unit
