namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("SyncToday.Data.Synchronization")>]
[<assembly: AssemblyProductAttribute("SyncToday.Data.Synchronization")>]
[<assembly: AssemblyDescriptionAttribute("Data Synchronization Core")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
