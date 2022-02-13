namespace FsharpK8s.Resources

open System
open System.IO

module Deployment =
    // TODO: Add other values
    type ImagePullPolicy =
        | Always

    type PodResources =
        { RequestMemory: float
          RequestCPU: float
          LimitMemory: float
          LimitCPU: float }

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
          ReadOnly: bool
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
          Resources: Option<PodResources>
          ContainerPort: Option<int>
          VolumeMount: Option<List<VolumeMount>>
          Env: Option<List<EnvType>>
          ImagePullSecrets: Option<string>
          Volumes: Option<List<VolumeType>> }

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

        member private this.addResources (templateString: string) =
            let resourcesId = "$RESOURCES$"
            let resourcesValue =
                match constructor.Resources with
                | None -> ""
                | Some resources -> 
                    "resources:" +
                    $"\n\trequests:" +
                    $"\n\t\tmemory: \"{resources.RequestMemory}Mi\"" +
                    $"\n\t\tcpu: \"{resources.RequestCPU}m\"" +
                    $"\n\tlimits:" +
                    $"\n\t\tmemory: \"{resources.LimitMemory}Mi\"" +
                    $"\n\t\tcpu: \"{resources.LimitCPU}m\""
            templateString.Replace(resourcesId, resourcesValue)

        member private this.addContainerPort (templateString: string) =
            let containerPortId = "$CONTAINER_PORT$"
            let containerPortValue =
                match constructor.ContainerPort with
                | Some port ->
                    "ports:" +
                    $"\n\t- containerPort: {port}" 
                | None -> ""
            templateString.Replace(containerPortId, containerPortValue)

        member private this.addVolumeMounted (templateString: string) =
            let volumeMountedId = "$VOLUME_MOUNTED$"
            let volumeMountedValue =
                match constructor.VolumeMount with
                | Some volumeMount ->
                    volumeMount
                    |> List.map
                        (fun volumeSpec ->
                            match volumeSpec.ReadOnly with
                            | Some readOnly ->
                                $"\n\t- name: {volumeSpec.Name}" +
                                $"\n\t  mountPath: {volumeSpec.MountPath}" +
                                $"\n\t  readOnly: {readOnly.ToString().ToLower()}"
                            | None -> 
                                $"\n\t- name: {volumeSpec.Name}" +
                                $"\n\t  mountPath: {volumeSpec.MountPath}")
                    |> List.append [ "volumeMounts:" ]
                    |> Shared.reduceIfNotEmpty (+)
                | None -> ""
            templateString.Replace(volumeMountedId, volumeMountedValue)

        member private this.addImagePullSecrets (templateString: string) =
            let imagePullSecretsId = "$IMAGE_PULL_SECRETS$"
            let imagePullSecretsValue =
                match constructor.ImagePullSecrets with
                | Some secret -> 
                    "imagePullSecrets:" +
                    $"\n\t- name: {secret}"
                | None -> ""
            templateString.Replace(imagePullSecretsId, imagePullSecretsValue)

        member private this.addEnvironmentVariables (templateString: string) =
            let envId = "$ENVIRONMENT_VAR$"
            let envValue =
                match constructor.Env with
                | None -> ""
                | Some envVars -> 
                    envVars
                    |> List.map
                        (fun var ->
                            match var with
                            | Simple e ->
                                $"\n\t- name: {e.Name}" +
                                $"\n\t  value: {e.Value}"
                            | Secret e -> 
                                $"\n\t- name: {e.Name}" +
                                "\n\t  valueFrom:" +
                                $"\n\t\tsecretKeyRef:" +
                                $"\n\t\t\tname: {e.SecretName}" +
                                $"\n\t\t\tkey: {e.SecretKey}")
                    |> List.append [ "env:" ]
                    |> Shared.reduceIfNotEmpty (+)
            templateString.Replace(envId, envValue)

        member private this.addVolumes (templateString: string) =
            let volumeId = "$VOLUMES$"
            let volumeValue =
                match constructor.Volumes with
                | None -> ""
                | Some volumeList ->
                    volumeList
                    |> List.map
                        (fun vol ->
                            match vol with
                            | VolumeFromPVC v ->
                                $"\n\t- name: {v.Name}" +
                                "\n\t  persistentVolumeClaim:" +
                                $"\n\t\tclaimName: {v.ClaimName}"
                            | VolumeFromSecretProviderClass v -> 
                                $"\n\t- name: {v.Name}" +
                                "\n\t  csi:" +
                                $"\n\t\tdriver: {v.Driver}" +
                                $"\n\t\treadOnly: {v.ReadOnly}" +
                                $"\n\t\tvolumeAttributes:" +
                                $"\n\t\t\tsecretProviderClass: {v.SecretProviderClassName}")
                    |> List.append [ "volumes:" ]
                    |> Shared.reduceIfNotEmpty (+)
            templateString.Replace(volumeId, volumeValue)

        member this.toYamlBuffer () =
            let templatePath = Shared.getTemplatesDirPath "/deployment/Pod.template"

            File.ReadAllText(templatePath, Text.Encoding.UTF8)
            |> this.addName
            |> this.addImage
            |> this.addImagePullPolicy
            |> this.addCommand
            |> this.addArguments
            |> this.addResources
            |> this.addContainerPort
            |> this.addVolumeMounted
            |> this.addImagePullSecrets
            |> this.addEnvironmentVariables
            |> this.addVolumes
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