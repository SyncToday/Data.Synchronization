module SyncToday.Data.Synchronizer.Tests

open SyncToday.Data.Synchronization
open NUnit.Framework

let dummy a = a
let nothing _ = ()
let notcalled _ = failwith "create should not be called"
let comparenotcalled (_,_,_,_) = failwith "compare should not be called"

[<Test>]
let ``0-empty call sample`` () =
  let result = Synchronizer.synchronize [] [] [] (fun p->p) (fun p->p) (fun p->p) (fun p->p) (fun p->()) (fun p->()) (fun q ->())
  Assert.IsNotNull(result)
  Assert.IsEmpty( result )

[<Test>]
let ``1-empty sync does nothing`` () =
  let result = Synchronizer.synchronize [] [] [] dummy dummy notcalled notcalled notcalled notcalled comparenotcalled
  Assert.IsNotNull(result)
  Assert.IsEmpty( result )

[<Test>]
let ``2-no data in system means we keep the mapping as it was before`` () =
  let result = Synchronizer.synchronize [|("2","X")|] [] [] dummy dummy notcalled notcalled notcalled notcalled comparenotcalled
  Assert.IsNotNull( result )
  Assert.IsNotEmpty( result )
  Assert.AreEqual( ("2","X"), result |> Seq.head )

[<Test>]
let ``3-same data in system means we keep the mapping as it was before`` () =
  let mutable compareCalled = false
  let result = Synchronizer.synchronize [|("2","X")|] [|"2"|] [|"X"|] dummy dummy notcalled notcalled notcalled notcalled (fun (_,_,_,_) -> compareCalled <-true )
  Assert.IsNotNull( result )
  Assert.IsNotEmpty( result )
  Assert.AreEqual( ("2","X"), result |> Seq.head )
  Assert.IsTrue(compareCalled)

[<Test>]
let ``4-pseudoreal test`` () =
  let mapped = [| ("2","X"); ("3","A"); ("5","J"); ("7","B") |]
  let system1 = [| "2";"3";"7";"9" |]
  let system2 = [| "A";"B";"J";"P";"X" |]
  let compareAndUpdate (system1:'system1,system2:'system2,update1:'system1->unit,update2:'system2->unit) : unit =
    ()
      
  let result = Synchronizer.synchronize mapped system1 system2 dummy dummy dummy dummy nothing nothing compareAndUpdate
  Assert.IsNotNull( result )
  Assert.IsNotEmpty( result )
  Assert.AreEqual( [("P","P");("9","9")] |> Seq.append mapped, result )

[<Test>]
let ``5-same data with different types in system means we keep the mapping as it was before`` () =
  let mutable compareCalled = false
  let result = Synchronizer.synchronize [|(2,"X")|] [|2|] [|"X"|] dummy dummy notcalled notcalled notcalled notcalled (fun (_,_,_,_) -> compareCalled <-true )
  Assert.IsNotNull( result )
  Assert.IsNotEmpty( result )
  Assert.AreEqual( (2,"X"), result |> Seq.head )
  Assert.IsTrue(compareCalled)

[<Test>]
let ``6-pseudoreal test with different types`` () =
  let dummySI (a:string) = int(a.[0])
  let dummyIS (a:int) = a.ToString()
  let mapped = [| (2,"X"); (3,"A"); (5,"J"); (7,"B") |]
  let system1 = [| 2;3;7;9 |]
  let system2 = [| "A";"B";"J";"P";"X" |]
  let compareAndUpdate (system1:'system1,system2:'system2,update1:'system1->unit,update2:'system2->unit) : unit =
    ()
      
  let result = Synchronizer.synchronize mapped system1 system2 dummy dummy dummySI dummyIS nothing nothing compareAndUpdate
  Assert.IsNotNull( result )
  Assert.IsNotEmpty( result )
  Assert.AreEqual( [(80,"P");(9,"9")] |> Seq.append mapped, result )

[<Test>]
let ``7-empty mapping, data in systems means create and map`` () =
  let mutable dummySICalled = false
  let mutable dummyISCalled = false
  let dummySI (a:string) = 
    if dummySICalled then failwith "dummySICalled"
    dummySICalled <- true
    int(a.[0])
  let dummyIS (a:int) = 
    if dummyISCalled then failwith "dummyISCalled"
    dummyISCalled <- true
    char(a).ToString()
  let mapped = [||]
  let system1 = [| int('A') |]
  let system2 = [| "B" |]
  let compareAndUpdate (system1:'system1,system2:'system2,update1:'system1->unit,update2:'system2->unit) : unit =
    ()
      
  let result = Synchronizer.synchronize mapped system1 system2 dummy dummy dummySI dummyIS nothing nothing compareAndUpdate
  Assert.IsNotNull( result )
  Assert.IsNotEmpty( result )
  Assert.AreEqual( mapped |> Seq.append [(int('B'),"B");(int('A'),"A")], result )

[<Test>]
let ``8-twice same means nothing`` () =
  let dummySI (a:string) = int(a.[0])
  let dummyIS (a:int) = char(a).ToString()
  let mapped = [||]
  let system1 = [| int('A') |]
  let system2 = [| "B" |]
  let compareAndUpdate (system1:'system1,system2:'system2,update1:'system1->unit,update2:'system2->unit) : unit =
    ()
      
  let result = Synchronizer.synchronize mapped system1 system2 dummy dummy dummySI dummyIS nothing nothing compareAndUpdate
  let result = Synchronizer.synchronize result [| int('A');int('B') |] [| "A";"B" |] dummy dummy dummySI dummyIS nothing nothing compareAndUpdate
  Assert.IsNotNull( result )
  Assert.IsNotEmpty( result )
  Assert.AreEqual( [(int('B'),"B");(int('A'),"A")] |> Seq.append mapped, result )
