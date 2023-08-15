open Fubernetes.Configuration
open Fubernetes.Resources

let createSimpleSecret () =
    let clusterIpService =
        new Service.ClusterIPService(
            { Name = "svc-clusterip-01"
              Namespace = "default"
              Selector = 
                [ ("app", "nginx")
                  ("network", "main-svc") ]
              Ports = 
                [ { Name = "tcp-port"
                    Protocol = ServiceShared.KubernetesProtocol.TCP
                    Port = 8000
                    TargetPort = 8080 }
                  { Name = "udp-port"
                    Protocol = ServiceShared.KubernetesProtocol.UDP
                    Port = 8001
                    TargetPort = 8081 } ] })

    let nodePortService =
        new Service.NodePortService(
            { Name = "svc-nodeport-01"
              Namespace = "default"
              Selector = 
                [ ("app", "nginx")
                  ("network", "main-svc") ]
              Ports = 
                [ { Name = "tcp-port"
                    Protocol = ServiceShared.KubernetesProtocol.TCP
                    Port = 8000
                    TargetPort = 8080
                    NodePort = None }
                  { Name = "udp-port"
                    Protocol = ServiceShared.KubernetesProtocol.UDP
                    Port = 8001
                    TargetPort = 8081
                    NodePort = Some 30071 } ] })

    let headlessService =
        new Service.HeadlessService(
            { Name = "svc-headless-01"
              Namespace = "default"
              Selector = 
                Some [ ("app", "nginx")
                       ("network", "main-svc") ]
              Ports = 
                [ { Name = "tcp-port"
                    Protocol = ServiceShared.KubernetesProtocol.TCP
                    Port = 8000
                    TargetPort = 8080 } ] })

    let headlessServiceWithoutSelector =
        new Service.HeadlessService(
            { Name = "svc-headless-01"
              Namespace = "default"
              Selector = None
              Ports = 
                [ { Name = "tcp-port"
                    Protocol = ServiceShared.KubernetesProtocol.TCP
                    Port = 8000
                    TargetPort = 8080 } ] })

    let opaqueSecret = 
        new Secret.OpaqueSecret(
            { Name = "secret-01"
              Namespace = "default"
              Data = 
                [ ("key1", "value1") 
                  ("key2", "value2")
                  ("key3", "value3") ] })

    let podTest =
      new Deployment.Pod(
        { Name = "container"
          Image = "nginx"
          ImagePullPolicy = Deployment.ImagePullPolicy.Always
          Command = Some "#!/bin/bash"
          Arguments = Some [ "apt"; "update" ]
          Resources = 
            Some 
              { RequestMemory = 1024.
                RequestCPU = 250.
                LimitMemory = 2048.
                LimitCPU = 500. }
          ContainerPort = Some 8080
          VolumeMount = 
            Some [
              { Name = "testVolume1"
                MountPath = "/var/"
                ReadOnly = None }
              { Name = "testVolume2"
                MountPath = "/var/"
                ReadOnly = Some true }]
          Env = 
            Some [
              Deployment.EnvType.Simple
                { Name = "hostname"
                  Value = "localhost" }
              Deployment.EnvType.Secret
                { Name = "password"
                  SecretName = "database-secrets"
                  SecretKey = "db-pwd" }
            ]
          ImagePullSecrets = None
          Volumes =
            Some [
              Deployment.VolumeType.VolumeFromPVC
                { Name = "cache-volume"
                  ClaimName = "cache-volume-pvc" }
              Deployment.VolumeType.VolumeFromSecretProviderClass
                { Name = "secrets-store"
                  Driver = "secrets-csi"
                  ReadOnly = true
                  SecretProviderClassName = "my-secrets-csi" }
            ] })

    let resourceList = 
        [ opaqueSecret.toYamlBuffer()
          clusterIpService.toYamlBuffer()
          nodePortService.toYamlBuffer()
          headlessService.toYamlBuffer()
          headlessServiceWithoutSelector.toYamlBuffer()
          podTest.toYamlBuffer() ]

    let outPath = "./prod/application.yml"
    let config: Configuration = 
        { OutPath = outPath
          Resources = resourceList }

    createOutPathAndBuildYaml (config)

[<EntryPoint>]
let main argv =
    createSimpleSecret ()
    0 // return an integer exit code