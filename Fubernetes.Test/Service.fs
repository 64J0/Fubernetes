module Tests.Service

open Expecto
open Expecto.Flip
open Fubernetes.Configuration
open Fubernetes.Resources

let verifyClusterIP () =
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

  let builtYaml = clusterIpService.toYamlBuffer()
  Expect.equal "ClusterIP yaml should be correct" expected builtYaml

let serviceTestsList : Test =
  testList "service tests" [
    testCase "Check the ClusterIP yaml" <| verifyClusterIP
    ptestCase "Check the NodePort yaml" <| fun () -> ()
    ptestCase "Check the Headless yaml" <| fun () -> ()
    ptestCase "Check the ExternalName yaml" <| fun () -> ()
    ptestCase "Check the LoadBalancer yaml" <| fun () -> ()
  ]
  |> testLabel "svc"