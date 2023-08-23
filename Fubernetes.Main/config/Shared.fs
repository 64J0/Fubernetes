namespace Fubernetes

open YamlDotNet.Serialization
open YamlDotNet.Serialization.NamingConventions

[<RequireQualifiedAccess>]
module Shared =
    type Metadata = { Name: string; Namespace: string }

    type BaseManifest(constructor: 'T) =
        member private this.getSerializer() : ISerializer =
            SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build()

        member this.toYamlBuffer() =

            let serializer = this.getSerializer ()

            serializer.Serialize constructor
