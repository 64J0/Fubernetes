namespace FsharpK8s.Resources

open System
open System.IO

// https://kubernetes.io/docs/concepts/services-networking/service/
module Service =
    type private TupleString = string * string

    type KubernetesProtocol =
        | TCP
        | UDP
        | SCTP
        | HTTP

    type PortConfig =
        { Name: string
          Protocol: KubernetesProtocol
          Port: int
          TargetPort: int }

    type ClusterIPServiceConstructor =
        { Name: string
          Namespace: string
          Selector: List<TupleString>
          Ports: List<PortConfig> }

    type ClusterIPService (constructor: ClusterIPServiceConstructor) =
        member private this.addName (templateString: string) =
            let nameId = "$NAME$"
            templateString.Replace(nameId, constructor.Name)

        member private this.addNamespace (templateString: string) =
            let namespaceId = "$NAMESPACE$"
            templateString.Replace(namespaceId, constructor.Namespace)

        member private this.addSelector (templateString: string) =
            let selectorId = "$SELECTOR$"
            let selectorValues =
                constructor.Selector
                |> List.map (fun (tuple: TupleString) -> $"\n\t\t{fst tuple}: {snd tuple}")
                |> List.reduce (+)

            templateString.Replace(selectorId, selectorValues)

        member private this.addPort (templateString: string) =
            let portId = "$PORTS$"
            let portValues =
                constructor.Ports
                |> List.map (fun (port: PortConfig) ->
                    $"\n\t\t- name: {port.Name}" +
                    $"\n\t\t  protocol: {port.Protocol.ToString().ToLower()}" +
                    $"\n\t\t  port: {port.Port}" +
                    $"\n\t\t  targetPort: {port.TargetPort}")
                |> List.reduce (+)

            templateString.Replace(portId, portValues)

        member this.toYamlBuffer () =
            let templatePath = "./src/templates/service/ClusterIP.template"

            File.ReadAllText(templatePath, Text.Encoding.UTF8)
            |> this.addName
            |> this.addNamespace
            |> this.addSelector
            |> this.addPort
            |> Shared.replaceTabsWithSpaces
            |> Shared.removeEmptyLines