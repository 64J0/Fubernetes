# F# Kubernetes YAML generator

> Still on development

This tool is being created to solve the problem of dealing with YAML configuration files for Kubernetes using a programatic solution. We choose to use F# as the main language mainly due to its strong type system that could help us find bugs/problems on our configuration files easily and faster.

## How it works?

Basically we have created some pre-defined types to make it easier and less error-prone creating the YAML Kubernetes files. In the `Program.fs` file, in the main root of this project it's possible to see an example on how to define opaque secrets.

```fsharp
// Define the secret config necessary data, I.E.: the name, namespace, the labels (Option type) and the data
// The data should be specified in plain text, later we do the encoding to base64 automatically
let opaqueSecret = 
    new Secret.OpaqueSecret(
        { Name = "Secret01"
          Namespace = "default"
          Data = 
            [ ("key1", "value1") 
              ("key2", "value2")
              ("key3", "value3") ] })

// After defining the objects you want to create in the Kubernetes cluster just place them in a list
// Remember to call their toYamlBuffer function that transforms their manifest into a string
let resourceList = 
    [ opaqueSecret.toYamlBuffer() ]

// Define the output path using relative path
let outPath = "./prod/test.secret.yml"

// Create a record union with both OutPath and the Resources you want to generate
let config: Configuration = 
    { OutPath = outPath
      Resources = resourceList }

// Finally, call this function to create the directory if it does not exist yet and create the YAML file
buildYaml (config)
```

The result after running this command will be a file called `test.secret.yml` with this content inside:

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: Secret01
  namespace: default
type: Opaque
data:
  key1: dmFsdWUx
	key2: dmFsdWUy
	key3: dmFsdWUz
```

And then, you could just apply this configuration by running:

```bash
$ kubectl apply -f prod/test.secret.yml
```