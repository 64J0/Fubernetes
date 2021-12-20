namespace FsharpK8s.Resources

open System
open System.IO

module ServiceShared =
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

    type NodePortConfig =
        { Name: string
          Protocol: KubernetesProtocol
          Port: int
          TargetPort: int 
          NodePort: int Option }

// https://kubernetes.io/docs/concepts/services-networking/service/
module Service =
    open ServiceShared

    type ClusterIPServiceConstructor =
        { Name: string
          Namespace: string
          Selector: List<Shared.TupleString>
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
                |> List.map (fun (tuple: Shared.TupleString) -> $"\n\t\t{fst tuple}: {snd tuple}")
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

    // =================================================================
    type NodePortConstructor =
        { Name: string
          Namespace: string
          Selector: List<Shared.TupleString>
          Ports: List<NodePortConfig> }

    type NodePortService (constructor: NodePortConstructor) =
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
                |> List.map (fun (tuple: Shared.TupleString) -> $"\n\t\t{fst tuple}: {snd tuple}")
                |> List.reduce (+)

            templateString.Replace(selectorId, selectorValues)

        member private this.addPort (templateString: string) =
            let portId = "$PORTS$"
            let portValues =
                constructor.Ports
                |> List.map (fun (port: NodePortConfig) ->
                    let portConfigWithoutNodePort =
                        $"\n\t\t- name: {port.Name}" +
                        $"\n\t\t  protocol: {port.Protocol.ToString().ToLower()}" +
                        $"\n\t\t  port: {port.Port}" +
                        $"\n\t\t  targetPort: {port.TargetPort}"
                    
                    match port.NodePort with
                    | None -> portConfigWithoutNodePort
                    | Some nodePort ->
                        portConfigWithoutNodePort +
                        $"\n\t\t  nodePort: {nodePort}")
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

    // =================================================================
    // https://blog.knoldus.com/what-is-headless-service-setup-a-service-in-kubernetes/
    type HeadlessConstructor =
        { Name: string
          Namespace: string
          Selector: List<Shared.TupleString> Option
          Ports: List<PortConfig> }

    type HeadlessService (constructor: HeadlessConstructor) =
        member private this.addFieldName 
            (fieldName: string) 
            (templateString: string) 
            =
            $"{fieldName}\n{templateString}"

        member private this.addName (templateString: string) =
            let nameId = "$NAME$"
            templateString.Replace(nameId, constructor.Name)

        member private this.addNamespace (templateString: string) =
            let namespaceId = "$NAMESPACE$"
            templateString.Replace(namespaceId, constructor.Namespace)

        member private this.addSelector (templateString: string) =
            let selectorId = "$SELECTOR$"
            let selectorValues =
                match constructor.Selector with
                | None -> ""
                | Some listOfTuple ->
                    listOfTuple
                    |> List.map (fun (tuple: Shared.TupleString) -> 
                        $"\n\t\t{fst tuple}: {snd tuple}")
                    |> List.reduce (+)
                    |> this.addFieldName "selector:"

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
            let templatePath = "./src/templates/service/Headless.template"

            File.ReadAllText(templatePath, Text.Encoding.UTF8)
            |> this.addName
            |> this.addNamespace
            |> this.addSelector
            |> this.addPort
            |> Shared.replaceTabsWithSpaces
            |> Shared.removeEmptyLines