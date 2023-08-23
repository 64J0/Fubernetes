namespace Fubernetes

open System
open System.IO

// https://kubernetes.io/docs/concepts/services-networking/service/
module Service =

    [<RequireQualifiedAccess>]
    type ServiceProtocol =
        | TCP
        | UDP
        | SCTP
        | HTTP

    [<RequireQualifiedAccess>]
    type ServiceKind =
        | ClusterIP
        | NodePort
        | ExternalName
        | LoadBalancer

    type ClusterIPPortConfig =
        { Name: string
          Protocol: ServiceProtocol
          Port: int
          TargetPort: int }

    type NodePortPortConfig =
        { Name: string
          Protocol: ServiceProtocol
          Port: int
          TargetPort: int
          NodePort: int Option }

    type ServiceSpec<'T> =
        { Selector: Map<string, string>
          Ports: List<'T>
          Type: ServiceKind }

    type ClusterIPConstructor =
        { ApiVersion: string
          Kind: string
          Metadata: Shared.Metadata
          Spec: ServiceSpec<ClusterIPPortConfig> }

        static member Default =
            { ApiVersion = "v1"
              Kind = "Service"
              Metadata =
                { Name = "default-clusterip"
                  Namespace = "default" }
              Spec =
                { Selector = [ "app.kubernetes.io/name", "MyApp" ] |> Map.ofList
                  Ports = List.empty
                  Type = ServiceKind.ClusterIP } }

    type ClusterIP(constructor: ClusterIPConstructor) =
        inherit Shared.BaseManifest(constructor)

    // =================================================================
    type NodePortConstructor =
        { ApiVersion: string
          Kind: string
          Metadata: Shared.Metadata
          Spec: ServiceSpec<NodePortPortConfig> }

        static member Default =
            { ApiVersion = "v1"
              Kind = "Service"
              Metadata =
                { Name = "default-nodeport"
                  Namespace = "default" }
              Spec =
                { Selector = [ "app.kubernetes.io/name", "MyApp" ] |> Map.ofList
                  Ports = List.empty
                  Type = ServiceKind.NodePort } }

    type NodePort(constructor: NodePortConstructor) =
        inherit Shared.BaseManifest(constructor)

    // =================================================================
    // https://blog.knoldus.com/what-is-headless-service-setup-a-service-in-kubernetes/

    type HeadlessServiceSpec =
        { Selector: Map<string, string>
          Ports: List<ClusterIPPortConfig>
          Type: ServiceKind
          ClusterIP: string }

    type HeadlessConstructor =
        { ApiVersion: string
          Kind: string
          Metadata: Shared.Metadata
          Spec: HeadlessServiceSpec }

        static member Default =
            { ApiVersion = "v1"
              Kind = "Service"
              Metadata =
                { Name = "default-headlessservice"
                  Namespace = "default" }
              Spec =
                { Selector = [ "app.kubernetes.io/name", "MyApp" ] |> Map.ofList
                  Ports = List.empty
                  Type = ServiceKind.ClusterIP
                  ClusterIP = "None" } }

    type Headless(constructor: HeadlessConstructor) =
        inherit Shared.BaseManifest(constructor)

    // =================================================================
    type ExternalNameSpec =
        { Type: ServiceKind
          ExternalName: string }

    type ExternalNameConstructor =
        { ApiVersion: string
          Kind: string
          Metadata: Shared.Metadata
          Spec: ExternalNameSpec }

        static member Default =
            { ApiVersion = "v1"
              Kind = "Service"
              Metadata =
                { Name = "default-externalname"
                  Namespace = "default" }
              Spec =
                { Type = ServiceKind.ExternalName
                  ExternalName = "my.database.example.com" } }

    type ExternalName(constructor: ExternalNameConstructor) =
        inherit Shared.BaseManifest(constructor)

    // =================================================================
    // https://kubernetes.io/docs/tasks/access-application-cluster/create-external-load-balancer/#preserving-the-client-source-ip
    [<RequireQualifiedAccess>]
    type ExternalTrafficPolicy =
        | Cluster
        | Local

    type LoadBalancerSpec =
        { Selector: Map<string, string>
          Ports: List<ClusterIPPortConfig>
          ExternalTrafficPolicy: ExternalTrafficPolicy
          Type: ServiceKind }

    type LoadBalancerConstructor =
        { ApiVersion: string
          Kind: string
          Metadata: Shared.Metadata
          Spec: LoadBalancerSpec }

        static member Default =
            { ApiVersion = "v1"
              Kind = "Service"
              Metadata =
                { Name = "default-headlessservice"
                  Namespace = "default" }
              Spec =
                { Selector = [ "app", "example" ] |> Map.ofList
                  Ports = List.empty
                  ExternalTrafficPolicy = ExternalTrafficPolicy.Local
                  Type = ServiceKind.LoadBalancer } }

    type LoadBalancerService(constructor: LoadBalancerConstructor) =
        inherit Shared.BaseManifest(constructor)
