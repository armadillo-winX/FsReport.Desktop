namespace FsReport.Core

open System.IO

module TemplateFileHandler =
    let GetTemplateFilesNameList () =
        let templateFiles = Directory.GetFiles(PathInfo.templatesDirectory, "*.*", SearchOption.TopDirectoryOnly)
        let templateFilesNameList = new ResizeArray<string>()
        for file in templateFiles do
            templateFilesNameList.Add(Path.GetFileName(file))
        templateFilesNameList

    let AddTemplateFile (sourceFile: string) =
        let fileName = Path.GetFileName(sourceFile)
        let templateFile = $"{PathInfo.templatesDirectory}\\{fileName}"
        File.Copy(sourceFile, templateFile)