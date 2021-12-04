namespace FsharpK8s.Resources

open System
open System.IO

// https://kubernetes.io/docs/concepts/configuration/secret/
module Secret =
    type private TupleString = string * string
    type private ListTupleString = List<TupleString>

    type OpaqueSecretConstructor =
        { Name: string // should be lowercase
          Namespace: string
          Data: ListTupleString }

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
        member private this.getTupleString (tuple: TupleString) =
            let tabLength = 2
            let tabSpace = new String(' ', tabLength)
            $"{fst tuple}: {snd tuple}\n\t".Replace("\t", tabSpace)

        member private this.encodeBase64 (tuple: TupleString) =
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
                |> List.reduce (fun (acc: string) (el: string) -> acc + el)

            templateString.Replace(dataId, dataValue)

        member this.toYamlBuffer () =
            let templatePath = "./src/templates/secret/OpaqueSecret.yaml"

            File.ReadAllText(templatePath, Text.Encoding.UTF8)
            |> this.addName
            |> this.addNamespace
            |> this.addData
