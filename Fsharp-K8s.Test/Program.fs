open Expecto
open Tests.Service

let projectTests =
  testList "k8s" [
    serviceTestsList
  ]

[<EntryPoint>]
let main args =
  runTestsWithCLIArgs [] args projectTests