open Expecto
open Tests.ConfigMap
open Tests.Service

let projectTests =
  testList "k8s" [
    configMapTestsList
    serviceTestsList
  ]

[<EntryPoint>]
let main args =
  runTestsWithCLIArgs [] args projectTests