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
            if reportCreationInfo.Type = ReportType.Numbering then
                let mutable i = 1
                let mutable numbering = i.ToString("D2")
                while Directory.Exists($"{subjectDirectory}\\{numbering}") do
                    i <- i + 1
                    numbering <- i.ToString("D2")
                reportFileNameFormatted <- reportFileNameFormatted.Replace("%ReportFolder%", numbering).Replace("%ReportType%", "Numbering")
                $"{subjectDirectory}\\{numbering}"
            elif reportCreationInfo.Type = ReportType.Midterm then
                reportFileNameFormatted <- reportFileNameFormatted.Replace("%ReportFolder%", "Midterm").Replace("%ReportType%", "Midterm")
                $"{subjectDirectory}\\Midterm"
            elif reportCreationInfo.Type = ReportType.Final then
                reportFileNameFormatted <- reportFileNameFormatted.Replace("%ReportFolder%", "Final").Replace("%ReportType%", "Final")
                $"{subjectDirectory}\\Final"
            else
                reportFileNameFormatted <- reportFileNameFormatted.Replace("%ReportType%", "Other")
                if String.IsNullOrEmpty(reportCreationInfo.ReportFolderNameOptional) = false then
                    reportFileNameFormatted<- reportFileNameFormatted.Replace("%ReportFolder%", reportCreationInfo.ReportFolderNameOptional)
                    $"{subjectDirectory}\\{reportCreationInfo.ReportFolderNameOptional}"
                else
                    reportFileNameFormatted<- reportFileNameFormatted.Replace("%ReportFolder%", "Report")
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
        processStartInfo.Arguments <- reportFilePath
        Process.Start(processStartInfo)
