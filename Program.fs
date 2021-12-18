open FsharpK8s.Configuration
open FsharpK8s.Resources

let createSimpleSecret () =
    let clusterIpService =
        new Service.ClusterIPService(
            { Name = "svc-01"
              Namespace = "default"
              Selector = 
                [ ("app", "nginx")
                  ("network", "main-svc") ]
              Ports = 
                [ { Name = "tcp-port"
                    Protocol = Service.KubernetesProtocol.TCP
                    Port = 8000
                    TargetPort = 8080 }
                  { Name = "udp-port"
                    Protocol = Service.KubernetesProtocol.UDP
                    Port = 8001
                    TargetPort = 8081 } ] })

    let opaqueSecret = 
        new Secret.OpaqueSecret(
            { Name = "secret-01"
              Namespace = "default"
              Data = 
                [ ("key1", "value1") 
                  ("key2", "value2")
                  ("key3", "value3") ] })

    let resourceList = 
        [ opaqueSecret.toYamlBuffer()
          clusterIpService.toYamlBuffer() ]

    let outPath = "./prod/application.yml"
    let config: Configuration = 
        { OutPath = outPath
          Resources = resourceList }

    createOutPathAndBuildYaml (config)

[<EntryPoint>]
let main argv =
    createSimpleSecret ()
    0 // return an integer exit code