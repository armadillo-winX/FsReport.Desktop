namespace FsReport.Core

open FsReport.Data
open System
open System.Diagnostics
open System.IO

module ReportFileHandler =
    let MakeReport (reportCreationInfo: ReportFileCreationInfo) =
        let subjectDirectory = $"{reportCreationInfo.ReportRootDirectory}\\{reportCreationInfo.SubjectFolderName}"
        if Directory.Exists(subjectDirectory) = false then Directory.CreateDirectory(subjectDirectory) |> ignore
        let reportDirectory = 
            if reportCreationInfo.Type = ReportType.Numbering then
                let mutable i = 1
                let mutable numbering = i.ToString("D2")
                while Directory.Exists($"{subjectDirectory}\\{numbering}") do
                    i <- i + 1
                    numbering <- i.ToString("D2")
                $"{subjectDirectory}\\{numbering}"
            elif reportCreationInfo.Type = ReportType.Midterm then
                $"{subjectDirectory}\\Midterm"
            elif reportCreationInfo.Type = ReportType.Final then
                $"{subjectDirectory}\\Final"
            else
                if String.IsNullOrEmpty(reportCreationInfo.ReportFolderNameOptional) = false then
                    $"{subjectDirectory}\\{reportCreationInfo.ReportFolderNameOptional}"
                else
                    $"{subjectDirectory}\\Report"
        Directory.CreateDirectory(reportDirectory) |> ignore
        let templateFilePath = $"{PathInfo.templatesDirectory}\\{reportCreationInfo.TemplateFileName}"
        let extension = Path.GetExtension(templateFilePath)
        let reportFilePath = $"{reportDirectory}\\{reportCreationInfo.ReportFileNameWithoutExtension}{extension}"
        File.Copy(templateFilePath, reportFilePath)
        reportFilePath

    let OpenReport (reportFilePath: string) (applicationPath: string) =
        let processStartInfo = new ProcessStartInfo()
        processStartInfo.FileName <- applicationPath
        processStartInfo.Arguments <- reportFilePath
        Process.Start(processStartInfo)
