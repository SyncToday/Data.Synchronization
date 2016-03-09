namespace SyncToday.Data.Synchronization

/// Documentation for my library
///
/// ## Example
///
///     let h = Library.hello 1
///     printfn "%d" h
///
module Synchronizer = 
  
  /// Returns 42
  ///
  /// ## Parameters
  ///  - `num` - whatever
  let synchronize 
    ( mapped: ('system1Id*'system2Id) seq ) (system1Ids:'system1Id seq) (system2Ids:'system2Id seq) 
    (get1:'system1Id->'system1) (get2:'system2Id->'system2)
    (createIn1:'system2->'system1Id) (createIn2:'system1->'system2Id) 
    (updateIn1:'system1->unit) (updateIn2:'system2->unit) 
    (compareAndUpdate:'system1*'system2*('system1->unit)*('system2->unit)->unit) =

    let getDifference ( mapped: ('system1Id*'system2Id) seq ) ( chooser : ('system1Id*'system2Id) -> 'systemId ) ( all:'systemId seq ) =
      set (all |> Seq.toArray) - set (mapped |> Seq.map( chooser ) |> Seq.toArray) |> Set.toSeq
    let createIn1Ids = 
      getDifference mapped  ( fun (_,p) -> p ) system2Ids
    let createIn2Ids = 
      getDifference mapped  ( fun (p,_) -> p ) system1Ids
    let mappedExisting = 
      mapped 
      |> Seq.filter( 
        fun (id1,id2) -> 
          system1Ids |> Seq.exists ( fun p->p=id1 ) &&
          system2Ids |> Seq.exists ( fun p->p=id2 ) 
      )
    let createInOther (ids: 'systemId seq) (get:'systemId->'a) (create:'a->'systemId' ) =
      ids |> Seq.map( fun id -> (id,get(id)) ) |> Seq.map( fun (id,entity) -> (id,create(entity)) )
    let append1 = 
      createInOther createIn1Ids get2 createIn1 |> Seq.map ( fun(a,b) -> (b,a) ) |> Seq.toList
    let append2 = 
      createInOther createIn2Ids get1 createIn2 |> Seq.toList

    mappedExisting |> Seq.iter ( fun (id1,id2) -> compareAndUpdate(get1(id1),get2(id2),updateIn1,updateIn2) ) 

    ( mapped |> Seq.toList) @ append1 @ append2 |> List.toSeq

    
