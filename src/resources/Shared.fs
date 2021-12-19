namespace FsharpK8s.Resources

open System

module Shared =
    type TupleString = string * string

    let replaceTabsWithSpaces (templateString: string) =
        let tabLength = 4
        let tabSpace = new String(' ', tabLength)
        templateString.Replace("\t", tabSpace)

    let removeEmptyLines (templateString: string) =
        templateString.Split("\n")
        |> Array.filter (System.String.IsNullOrEmpty >> not)
        |> Array.reduce (fun (acc: string) (line: string) -> $"{acc}\n{line}")