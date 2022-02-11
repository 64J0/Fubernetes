namespace FsharpK8s.Resources

open System
open System.IO

module Deployment =
    // TODO: Add other values
    type ImagePullPolicy =
        | Always

    // TODO: Add other values
    type VolumeMount =
        { Name: string
          MountPath: string
          ReadOnly: Option<bool> }

    type SimpleEnv =
        { Name: string
          Value: string }

    type SecretEnv =
        { Name: string
          SecretName: string
          SecretKey: string }

    // TODO: Add other values
    type EnvType =
        | Simple of SimpleEnv
        | Secret of SecretEnv

    type VolumeFromPVC =
        { Name: string
          ClaimName: string }

    type VolumeFromSecretProviderClass =
        { Name: string
          Driver: string
          ReadOnly: Option<bool>
          SecretProviderClassName: string }

    // TODO: Add other values
    type VolumeType =
        | VolumeFromPVC of VolumeFromPVC
        | VolumeFromSecretProviderClass of VolumeFromSecretProviderClass

    type PodConstructor =
        { Name: string
          Image: string
          ImagePullPolicy: ImagePullPolicy
          Command: Option<string>
          Arguments: Option<List<string>>
          RequestMemory: float
          RequestCPU: float
          LimitMemory: float
          LimitCPU: float
          ContainerPort: Option<int>
          VolumeMount: Option<List<VolumeMount>>
          Env: Option<List<EnvType>>
          ImagePullSecrets: Option<string>
          Volumes: Option<VolumeType> }

    type Pod(constructor: PodConstructor) =
        member private this.addName (templateString: string) =
            let nameId = "$CONTAINER_NAME$"
            templateString.Replace(nameId, constructor.Name)

        member private this.addImage (templateString: string) =
            let imageId = "$IMAGE$"
            templateString.Replace(imageId, constructor.Image)

        member private this.addImagePullPolicy (templateString: string) =
            let imagePullPolicyId = "$IMAGE_PULL_POLICY$"
            let imagePullPolicyValue = constructor.ImagePullPolicy.ToString()
            templateString.Replace(imagePullPolicyId, imagePullPolicyValue)

        member private this.addCommand (templateString: string) =
            let commandId = "$COMMAND$"
            let commandValue =
                match constructor.Command with
                | Some command -> $"command: [ \"{command}\" ]"
                | None -> ""
            templateString.Replace(commandId, commandValue)

        member private this.addArguments (templateString: string) =
            let argumentsId = "$ARGUMENTS$"
            let argumentsValue =
                match constructor.Arguments with
                | Some arguments ->
                    arguments
                    |> Shared.reduceIfNotEmpty 
                        (fun (acc: string) (el: string) -> $"\"{acc}\", \"{el}\"")
                    |> function
                        | value -> $"args: [ {value} ]"
                | None -> ""
            templateString.Replace(argumentsId, argumentsValue)

        member this.toYamlBuffer () =
            let templatePath = Shared.getTemplatesDirPath "/deployment/Pod.template"

            File.ReadAllText(templatePath, Text.Encoding.UTF8)
            |> this.addName
            |> this.addImage
            |> this.addImagePullPolicy
            |> this.addCommand
            |> this.addArguments
            |> Shared.replaceTabsWithSpaces
            |> Shared.removeEmptyLines











    // =======================================================
    // type DeploymentConstructor =
    //     { Name: string // should be lowercase
    //       Namespace: string
    //       Data: List<Shared.TupleString>
    //       BinaryData: List<Shared.TupleString>
    //       Immutable: bool }

    // type Deployment (constructor: ConfigMapConstructor) =
    //     member private this.addName (templateString: string) =
    //         let nameId = "$CONTAINER_NAME$"
    //         templateString.Replace(nameId, constructor.Name)

    //     member private this.addNamespace (templateString: string) =
    //         let namespaceId = "$NAMESPACE$"
    //         templateString.Replace(namespaceId, constructor.Namespace)

    //     // https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/strings
    //     member private this.getTupleString (tuple: Shared.TupleString) =
    //         $"\n\t{fst tuple}: {snd tuple}"

    //     member private this.addData (templateString: string) =
    //         let dataId = "$DATA$"
    //         let dataValue =
    //             constructor.Data
    //             |> List.map this.getTupleString
    //             |> Shared.reduceIfNotEmpty (+)

    //         templateString.Replace(dataId, dataValue)

    //     member private this.addBinaryData (templateString: string) =
    //         let binaryDataId = "$BINARY_DATA$"
    //         let binaryDataValue =
    //             constructor.BinaryData
    //             |> List.map this.getTupleString
    //             |> Shared.reduceIfNotEmpty (+)

    //         templateString.Replace(binaryDataId, binaryDataValue)

    //     member private this.addImmutable (templateString: string) =
    //         let immutableId = "$IMMUTABLE$"
    //         let immutableValue = 
    //             match constructor.Immutable with
    //             | true -> "true"
    //             | false -> "false"

    //         templateString.Replace(immutableId, immutableValue)

    //     member this.toYamlBuffer () =
    //         let templatePath = Shared.getTemplatesDirPath "/configmap/ConfigMap.template"

    //         File.ReadAllText(templatePath, Text.Encoding.UTF8)
    //         |> this.addName
    //         |> this.addNamespace
    //         |> this.addData
    //         |> this.addBinaryData
    //         |> this.addImmutable
    //         |> Shared.replaceTabsWithSpaces
    //         |> Shared.removeEmptyLines