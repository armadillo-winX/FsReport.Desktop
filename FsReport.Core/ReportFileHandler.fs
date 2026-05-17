namespace FsReport.Core

open FsReport.Data
open System
open System.Diagnostics
open System.IO

module ReportFileHandler =
    let MakeReport (reportCreationInfo: ReportFileCreationInfo) =
        let subjectDirectory = $"{reportCreationInfo.ReportRootDirectory}\\{reportCreationInfo.SubjectFolderName}"
        let mutable reportFileNameFormatted = reportCreationInfo.ReportFileNameWithoutExtension;
        reportFileNameFormatted <- reportFileNameFormatted.Replace("%SubjectName%", reportCreationInfo.SubjectName).Replace("%SubjectFolder%", reportCreationInfo.SubjectFolderName)
        if Directory.Exists(subjectDirectory) = false then Directory.CreateDirectory(subjectDirectory) |> ignore
        let reportDirectory =
            match reportCreationInfo with
            | r when r.Type = ReportType.Numbering -> 
                let mutable i = 1
                let mutable numbering = i.ToString("D2")
                while Directory.Exists($"{subjectDirectory}\\{numbering}") do
                    i <- i + 1
                    numbering <- i.ToString("D2")
                reportFileNameFormatted <- reportFileNameFormatted.Replace("%ReportFolder%", numbering).Replace("%ReportType%", "Numbering")
                $"{subjectDirectory}\\{numbering}"
            | r when r.Type = ReportType.Midterm ->
                reportFileNameFormatted <- reportFileNameFormatted.Replace("%ReportFolder%", "Midterm").Replace("%ReportType%", "Midterm")
                $"{subjectDirectory}\\Midterm"
            | r when r.Type = ReportType.Final ->
                reportFileNameFormatted <- reportFileNameFormatted.Replace("%ReportFolder%", "Final").Replace("%ReportType%", "Final")
                $"{subjectDirectory}\\Final"
            | r when String.IsNullOrEmpty(r.ReportFolderNameOptional) = false ->
                reportFileNameFormatted <- reportFileNameFormatted.Replace("%ReportFolder%", reportCreationInfo.ReportFolderNameOptional).Replace("%ReportType%", "Other")
                $"{subjectDirectory}\\{reportCreationInfo.ReportFolderNameOptional}"
            |_ -> 
                reportFileNameFormatted <- reportFileNameFormatted.Replace("%ReportFolder%", "Report").Replace("%ReportType%", "Other")
                $"{subjectDirectory}\\Report"
        Directory.CreateDirectory(reportDirectory) |> ignore
        let templateFilePath = $"{PathInfo.templatesDirectory}\\{reportCreationInfo.TemplateFileName}"
        let extension = Path.GetExtension(templateFilePath)
        let reportFilePath = $"{reportDirectory}\\{reportFileNameFormatted}{extension}"
        File.Copy(templateFilePath, reportFilePath)
        reportFilePath

    let OpenReport (reportFilePath: string) (applicationPath: string) =
        let processStartInfo = new ProcessStartInfo()
        processStartInfo.FileName <- applicationPath
        processStartInfo.Arguments <- $"\"{reportFilePath}\""
        Process.Start(processStartInfo)
