# F# Kubernetes YAML generator

> Still on development

This tool is being developed to solve the problem of dealing with YAML configuration files for Kubernetes, using a programatic solution. We chose to use F# as the main language due to its strong type system, which can help us find bugs/problems on our configuration files more easily and faster.

This project structure could be explained as:

* `Fubernetes.Main/`: this folder have the main files of the project. Basically there is where all the necessary code lives.
* `Fubernetes.UseLocal/`: this folder should be used to test the project locally. You could run the following command through the CLI to inspect the response: `dotnet run --project Fubernetes.UseLocal/`.
* `Fubernetes.Test/`: this folder store the automated tests for the project.

### Requisites:

* .NET SDK 5.0.401

## How does it work?

We have created pre-defined types to make it easier and less error-prone to create YAML Kubernetes files. See the [Program.fs](Fubernetes.UseLocal/Program.fs) file for examples, such as **defining opaque secrets**:

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
$ dotnet run --project Fubernetes.UseLocal/
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
  <summary>ConfigMap</summary>
  
  - [x] ConfigMap
</details>

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
$ dotnet run --project Fubernetes.Test/Test.fsproj
```

## Build the image and run the Docker container

```bash
# build the image
$ docker build -t Fubernetes-main:latest -f Dockerfile .

# run the image in a container
$ docker run --name Fubernetes Fubernetes-main:latest
# debug container structure
# docker run -it --entrypoint "bash" Fubernetes-main:latest

# running with docker-compose
# TODO
```

## Automated tests coverage

For collecting the automated tests coverage we use dotnet tool: [dotnet-coverage](https://learn.microsoft.com/en-us/dotnet/core/additional-tools/dotnet-coverage).

```bash
$ dotnet tool restore

$ dotnet dotnet-coverage collect -f "xml" "dotnet run --project Fubernetes.Test/"

# and to visualize the report
$ dotnet reportgenerator -reports:output.xml -targetdir:coveragereport
```

There are many examples here: [link](https://github.com/microsoft/codecoverage/blob/main/samples/Calculator/README.md).