open Expecto
open Tests.Service

[<EntryPoint>]
let main args =
  runTestsWithCLIArgs [] args serviceTestsList