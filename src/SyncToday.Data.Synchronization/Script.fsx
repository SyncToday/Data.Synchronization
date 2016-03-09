#load "Library.fs"
open SyncToday.Data.Synchronization

let result = Synchronizer.synchronize [] ([]:int list) ([]:int list) (fun p->p) (fun p->p) (fun p->p) (fun p->p) (fun _->()) (fun _->()) (fun q ->())
printfn "%A" result
