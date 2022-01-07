# F# Kubernetes YAML generator

> Still on development

This tool is being developed to solve the problem of dealing with YAML configuration files for Kubernetes, using a programatic solution. We chose to use F# as the main language due to its strong type system, which can help us find bugs/problems on our configuration files more easily and faster.

### Requisites:

* .NET SDK 5.0.401

## How does it work?

We have created pre-defined types to make it easier and less error-prone to create YAML Kubernetes files. In the `Program.fs` file, in the main root of this project, it is possible to see an example of how to define opaque secrets.

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
let outPath = "./prod/application.yml"

// Create a record with both OutPath and the Resources you want to generate
let config: Configuration = 
    { OutPath = outPath
      Resources = resourceList }

// Finally, call this function to create the directory if it does not exist yet and create the YAML file
createOutPathAndBuildYaml (config)
```

#### Running the program

```bash
$ dotnet run --project Fsharp-K8s.Main/Main.fsproj
```


The result after running this command will be a file called `application.yml`, with the following content inside:

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: secret-01
  namespace: default
type: Opaque
data:
  key1: dmFsdWUx
  key2: dmFsdWUy
  key3: dmFsdWUz
```

##### Then, apply this configuration by running

```bash
$ kubectl apply -f prod/test.secret.yml
```

## What we have mapped so far:

<details>
  <summary>Secrets</summary>
  
  - [x] OpaqueSecrets
  - [ ] ServiceAccountToken
  - [ ] DockerCfg
  - [ ] DockerConfigJson
  - [ ] BasicAuthentication
  - [ ] SshAuth
  - [ ] Tls
  - [ ] BootstrapTokenData
</details>

<details>
  <summary>Services</summary>
  
  - [x] ClusterIP
  - [x] NodePort
  - [x] Headless
  - [x] ExternalName
  - [x] LoadBalancer
</details>

## Automated tests:

To get more confident with this project we are using [Expecto](https://github.com/haf/expecto) to write unit tests. This tool is also written in F# and can be used for other kinds of tests as well: stress, regression or property based tests. For more information please consult the project documentation itself.

To run the tests locally you just need to run these commands on the terminal:

```bash
# restore paket
$ dotnet tool restore

# install the dependencies
$ dotnet paket restore

# run the tests
$ dotnet run --project Fsharp-K8s.Test/Test.fsproj
```
