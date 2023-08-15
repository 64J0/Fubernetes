namespace Fubernetes.Resources

open System
open System.IO

// https://kubernetes.io/docs/concepts/configuration/configmap/
module ConfigMap =
    type ConfigMapConstructor =
        { Name: string // should be lowercase
          Namespace: string
          Data: List<Shared.TupleString>
          BinaryData: List<Shared.TupleString>
          Immutable: bool }

    type ConfigMap (constructor: ConfigMapConstructor) =
        member private this.addName (templateString: string) =
            let nameId = "$NAME$"
            templateString.Replace(nameId, constructor.Name)

        member private this.addNamespace (templateString: string) =
            let namespaceId = "$NAMESPACE$"
            templateString.Replace(namespaceId, constructor.Namespace)

        // https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/strings
        member private this.getTupleString (tuple: Shared.TupleString) =
            $"\n\t{fst tuple}: {snd tuple}"

        member private this.addData (templateString: string) =
            let dataId = "$DATA$"
            let dataValue =
                constructor.Data
                |> List.map this.getTupleString
                |> Shared.reduceIfNotEmpty (+)

            templateString.Replace(dataId, dataValue)

        member private this.addBinaryData (templateString: string) =
            let binaryDataId = "$BINARY_DATA$"
            let binaryDataValue =
                constructor.BinaryData
                |> List.map this.getTupleString
                |> Shared.reduceIfNotEmpty (+)

            templateString.Replace(binaryDataId, binaryDataValue)

        member private this.addImmutable (templateString: string) =
            let immutableId = "$IMMUTABLE$"
            let immutableValue = 
                match constructor.Immutable with
                | true -> "true"
                | false -> "false"

            templateString.Replace(immutableId, immutableValue)

        member this.toYamlBuffer () =
            let templatePath = Shared.getTemplatesDirPath "/configmap/ConfigMap.template"

            File.ReadAllText(templatePath, Text.Encoding.UTF8)
            |> this.addName
            |> this.addNamespace
            |> this.addData
            |> this.addBinaryData
            |> this.addImmutable
            |> Shared.replaceTabsWithSpaces
            |> Shared.removeEmptyLines
