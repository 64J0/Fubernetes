open Fubernetes.Service
open Fubernetes.Secret
open Fubernetes.Configuration
open Fubernetes.Resources

let createSimpleSecret () =
    let clusterIpService =
        new ClusterIP(
            { ClusterIPConstructor.Default with
                Metadata =
                    { Name = "svc-clusterip-01"
                      Namespace = "default" }
                Spec =
                    { Selector = ([ "app", "nginx"; "network", "main-svc" ] |> Map.ofList)
                      Ports =
                        [ { Name = "tcp-port"
                            Protocol = ServiceProtocol.TCP
                            Port = 8000
                            TargetPort = 8080 }
                          { Name = "udp-port"
                            Protocol = ServiceProtocol.UDP
                            Port = 8001
                            TargetPort = 8081 } ]
                      Type = ServiceKind.ClusterIP } }
        )

    let nodePortService =
        new NodePort(
            { NodePortConstructor.Default with
                Metadata =
                    { Name = "svc-nodeport-01"
                      Namespace = "default" }
                Spec =
                    { Selector = ([ "app", "nginx"; "network", "main-svc" ] |> Map.ofList)
                      Ports =
                        [ { Name = "tcp-port"
                            Protocol = ServiceProtocol.TCP
                            Port = 8000
                            TargetPort = 8080
                            NodePort = None }
                          { Name = "udp-port"
                            Protocol = ServiceProtocol.UDP
                            Port = 8001
                            TargetPort = 8081
                            NodePort = Some 30071 } ]
                      Type = ServiceKind.NodePort } }
        )

    let headlessService =
        new Headless(
            { HeadlessConstructor.Default with
                Metadata =
                    { Name = "svc-headless-01"
                      Namespace = "default" }
                Spec =
                    { Selector = ([ "app", "nginx"; "network", "main-svc" ] |> Map.ofList)
                      Ports =
                        [ { Name = "tcp-port"
                            Protocol = ServiceProtocol.TCP
                            Port = 8000
                            TargetPort = 8080 } ]
                      ClusterIP = "None"
                      Type = ServiceKind.ClusterIP } }
        )

    let headlessServiceWithoutSelector =
        new Headless(
            { HeadlessConstructor.Default with
                Metadata =
                    { Name = "svc-headless-01"
                      Namespace = "default" }
                Spec =
                    { Selector = Map.empty
                      Ports =
                        [ { Name = "tcp-port"
                            Protocol = ServiceProtocol.TCP
                            Port = 8000
                            TargetPort = 8080 } ]
                      ClusterIP = "None"
                      Type = ServiceKind.ClusterIP } }
        )

    let opaqueSecret =
        new OpaqueSecret(
            { OpaqueSecretConstructor.Default with
                Metadata =
                    { Name = "secret-01"
                      Namespace = "default" }
                Data = [ "key-1", "value-1"; "key-2", "value-2"; "key-3", "value 3" ] |> Map.ofList }
        )

    // let podTest =
    //     new Deployment.Pod(
    //         { Name = "container"
    //           Image = "nginx"
    //           ImagePullPolicy = Deployment.ImagePullPolicy.Always
    //           Command = Some "#!/bin/bash"
    //           Arguments = Some [ "apt"; "update" ]
    //           Resources =
    //             Some
    //                 { RequestMemory = 1024.
    //                   RequestCPU = 250.
    //                   LimitMemory = 2048.
    //                   LimitCPU = 500. }
    //           ContainerPort = Some 8080
    //           VolumeMount =
    //             Some
    //                 [ { Name = "testVolume1"
    //                     MountPath = "/var/"
    //                     ReadOnly = None }
    //                   { Name = "testVolume2"
    //                     MountPath = "/var/"
    //                     ReadOnly = Some true } ]
    //           Env =
    //             Some
    //                 [ Deployment.EnvType.Simple
    //                       { Name = "hostname"
    //                         Value = "localhost" }
    //                   Deployment.EnvType.Secret
    //                       { Name = "password"
    //                         SecretName = "database-secrets"
    //                         SecretKey = "db-pwd" } ]
    //           ImagePullSecrets = None
    //           Volumes =
    //             Some
    //                 [ Deployment.VolumeType.VolumeFromPVC
    //                       { Name = "cache-volume"
    //                         ClaimName = "cache-volume-pvc" }
    //                   Deployment.VolumeType.VolumeFromSecretProviderClass
    //                       { Name = "secrets-store"
    //                         Driver = "secrets-csi"
    //                         ReadOnly = true
    //                         SecretProviderClassName = "my-secrets-csi" } ] }
    //     )

    let resourceList =
        [ opaqueSecret.toYamlBuffer ()
          clusterIpService.toYamlBuffer ()
          nodePortService.toYamlBuffer ()
          headlessService.toYamlBuffer ()
          headlessServiceWithoutSelector.toYamlBuffer () ]

    let outPath = "./prod/application.yml"

    let config: Configuration =
        { OutPath = outPath
          Resources = resourceList }

    createOutPathAndBuildYaml (config)

[<EntryPoint>]
let main argv =
    createSimpleSecret ()
    0 // return an integer exit code
