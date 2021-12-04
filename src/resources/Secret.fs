namespace FsharpK8s

open System
open System.IO

// https://kubernetes.io/docs/concepts/configuration/secret/
module Secret =
    type private TupleString = string * string
    type private ListTupleString = List<TupleString>

    type OpaqueSecretConstructor =
        { Name: string
          Namespace: string
          Labels: Option<ListTupleString>
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
            let nameId = "$NAME"
            templateString.Replace(nameId, constructor.Name)

        member private this.addNamespace (templateString: string) =
            let namespaceId = "$NAMESPACE"
            templateString.Replace(namespaceId, constructor.Namespace)

        // https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/strings
        member private this.getTupleString (tuple: TupleString) =
            $"{fst tuple}:{snd tuple}\n\t"

        member private this.addLabels (templateString: string) =
            let labelsId = "$LABELS"
            let labelsValue =
                match (constructor.Labels) with
                | Some labels -> 
                    labels
                    |> List.map this.getTupleString
                    |> List.reduce (fun (acc: string) (el: string) -> acc + el)
                | None -> ""

            templateString.Replace(labelsId, labelsValue)

        member private this.encodeBase64 (tuple: TupleString) =
            let encodedValue = 
                (snd tuple)
                |> Text.Encoding.UTF8.GetBytes
                |> Convert.ToBase64String
            (fst tuple, encodedValue)

        member private this.addData (templateString: string) =
            let dataId = "$DATA"
            let dataValue =
                constructor.Data
                |> List.map (this.encodeBase64 >> this.getTupleString)
                |> List.reduce (fun (acc: string) (el: string) -> acc + el)

            templateString.Replace(dataId, dataValue)

        member this.toYamlBuffer () =
            let templatePath = "../templates/secret/OpaqueSecret.yaml"

            File.ReadAllText(templatePath, Text.Encoding.UTF8)
            |> this.addName
            |> this.addNamespace
            |> this.addLabels
            |> this.addData
