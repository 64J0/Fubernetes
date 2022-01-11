namespace FsharpK8s.Resources

open System
open System.IO

// https://kubernetes.io/docs/concepts/configuration/secret/
module Secret =
    type OpaqueSecretConstructor =
        { Name: string // should be lowercase
          Namespace: string
          Data: List<Shared.TupleString> }

    // TODO
    type ServiceAccountTokenConstructor = unit

    // TODO
    type DockerCfgConstructor = unit

    // TODO
    type DockerConfigJsonConstructor = unit

    // TODO
    type BasicAuthenticationConstructor = unit

    // TODO
    type SshAuthConstructor = unit

    // TODO
    type TlsConstructor = unit

    // TODO
    type BootstrapTokenDataConstructor = unit

    type OpaqueSecret (constructor: OpaqueSecretConstructor) =
        member private this.addName (templateString: string) =
            let nameId = "$NAME$"
            templateString.Replace(nameId, constructor.Name)

        member private this.addNamespace (templateString: string) =
            let namespaceId = "$NAMESPACE$"
            templateString.Replace(namespaceId, constructor.Namespace)

        // https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/strings
        member private this.getTupleString (tuple: Shared.TupleString) =
            $"\n\t{fst tuple}: {snd tuple}"

        member private this.encodeBase64 (tuple: Shared.TupleString) =
            let encodedValue () = 
                (snd tuple)
                |> Text.Encoding.UTF8.GetBytes
                |> Convert.ToBase64String
            (fst tuple, encodedValue ())

        member private this.addData (templateString: string) =
            let dataId = "$DATA$"
            let dataValue =
                constructor.Data
                |> List.map (this.encodeBase64 >> this.getTupleString)
                |> List.reduce (+)

            templateString.Replace(dataId, dataValue)

        member this.toYamlBuffer () =
            let templatePath = Shared.getTemplatesDirPath "/secret/OpaqueSecret.template"

            File.ReadAllText(templatePath, Text.Encoding.UTF8)
            |> this.addName
            |> this.addNamespace
            |> this.addData
            |> Shared.replaceTabsWithSpaces
            |> Shared.removeEmptyLines
