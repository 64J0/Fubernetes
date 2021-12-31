module Tests.Service

open Expecto
open FsharpK8s.Configuration
open FsharpK8s.Resources

let tests =
  test "Check the ClusterIP yaml" {
    let expected =
      """apiVersion: v1
kind: Service
metadata:
    name: svc-clusterip-01
    namespace: default
spec:
    selector: 
        app: nginx
        network: main-svc
    ports: 
        - name: tcp-port
          protocol: tcp
          port: 8000
          targetPort: 8080
        - name: udp-port
          protocol: udp
          port: 8001
          targetPort: 8081
    type: ClusterIP"""

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

    let yaml = clusterIpService.toYamlBuffer()
    Expect.equal expected yaml "ClusterIP yaml should be correct"
    // let subject = "Hello World"
    // Expect.equal subject "Hello World" "The strings should equal"
  }

[<EntryPoint>]
let main args =
  runTestsWithCLIArgs [] args tests