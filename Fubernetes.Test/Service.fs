module Tests.Service

open Expecto
open Expecto.Flip
open Fubernetes.Service

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

    let builtYaml = clusterIpService.toYamlBuffer ()
    Expect.equal "ClusterIP yaml should be correct" expected builtYaml

let serviceTestsList: Test =
    testList
        "service tests"
        [ testCase "Check the ClusterIP yaml" <| verifyClusterIP
          ptestCase "Check the NodePort yaml" <| fun () -> ()
          ptestCase "Check the Headless yaml" <| fun () -> ()
          ptestCase "Check the ExternalName yaml" <| fun () -> ()
          ptestCase "Check the LoadBalancer yaml" <| fun () -> () ]
    |> testLabel "svc"
