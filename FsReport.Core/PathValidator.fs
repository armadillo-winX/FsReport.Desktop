namespace FsReport.Core

module PathValidator =
    let IsValidObjectName (objectName: string) =
        let invalidLetters = [ '/'; '?'; '<'; '>'; '\\'; ':'; '*'; '|'; '"' ]
        let result = String.exists (fun c -> List.contains c invalidLetters) objectName
        if result then false else true