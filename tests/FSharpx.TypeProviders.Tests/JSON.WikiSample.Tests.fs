﻿module FSharpx.TypeProviders.Tests.JSON.WikieSampleTests

open NUnit.Framework
open FSharpx
open FsUnit

type WikiSample =
    StructuredJSON<Schema=
        """{  
                 "firstName": "John",
                 "lastName" : "Smith",
                 "age"      : 25,
                 "address"  :
                 {
                     "streetAddress": "21 2nd Street",
                     "city"         : "New York",
                     "state"        : "NY",
                     "postalCode"   : "10021"
                 },
                 "phoneNumber":
                 [
                     {
                       "type"  : "home",
                       "number": "212 555-1234"
                     },
                     {
                       "type"  : "fax",
                       "number": "646 555-4567"
                     }
                 ]
             }""">

[<Test>]
let ``Can parse wiki sample``() = 
    let document = WikiSample().Root
    document.FirstName |> should equal "John"

    let phone = document.GetPhoneNumbers() |> Seq.head
    phone.Number |> should equal "212 555-1234"

[<Test>]
let ``Can load and manipulate wiki data``() = 
    let document = WikiSample(filename="WikiData.json").Root
    document.FirstName |> should equal "John"
    document.LastName |> should equal "Doe"

    document.GetPhoneNumbers() |> Seq.length |> should equal 0

    document.NewPhoneNumber(Type="home",Number="456 123-4567")
    |> document.AddPhoneNumber

    document.GetPhoneNumbers() |> Seq.length |> should equal 1

[<Test>]
let ``Can load empty json file and fails on property access``() = 
    let document = WikiSample(filename="Empty.json").Root
    let failed = ref false
    try
        document.FirstName |> ignore
    with
    | _ -> failed := true
    Assert.IsTrue !failed