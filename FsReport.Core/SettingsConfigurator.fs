namespace FsReport.Core

open System
open System.Collections.Generic
open System.IO
open System.Text.Encodings.Web
open System.Text.Json
open System.Text.Unicode
open System.Xml

module SettingsConfigurator =
    let SaveReportRootDirConf (reportRootDir: string) =
        let reportRootDirConfXml = new XmlDocument()
        reportRootDirConfXml.CreateXmlDeclaration("1.0", "UTF-8", null) |> reportRootDirConfXml.AppendChild |> ignore
        let rootNode = reportRootDirConfXml.CreateElement("ReportRootDirectoryConfig")
        reportRootDirConfXml.AppendChild(rootNode) |> ignore
        let pathNode = reportRootDirConfXml.CreateElement("Path")
        reportRootDir |> reportRootDirConfXml.CreateTextNode |> pathNode.AppendChild |> ignore
        rootNode.AppendChild(pathNode) |> ignore
        reportRootDirConfXml.Save(PathInfo.reportRootDirectoryConfig)

    let GetReportRootDirConf () =
        if File.Exists(PathInfo.reportRootDirectoryConfig) then
            let reportRootDirConfXml = new XmlDocument()
            reportRootDirConfXml.Load(PathInfo.reportRootDirectoryConfig)
            reportRootDirConfXml.SelectSingleNode("//Path").InnerText
        else
            String.Empty

    let GetSubjectFolderNameDictionary () =
        let mutable reportDirNameDictionary = new Dictionary<string, string>()
        if File.Exists(PathInfo.subjectFolderNameConfig) then
            let config = File.ReadAllText(PathInfo.subjectFolderNameConfig)
            let serializeOption = new JsonSerializerOptions()
            serializeOption.Encoder <- JavaScriptEncoder.Create(UnicodeRanges.All)
            serializeOption.WriteIndented <- true
            reportDirNameDictionary <- JsonSerializer.Deserialize<Dictionary<string, string>>(config, serializeOption)
        reportDirNameDictionary

    let SaveReportFolderNameConfig (reportDirNameDictionary: Dictionary<string, string>) =
        let reportDirNameConfig = PathInfo.subjectFolderNameConfig
        let serializeOption = new JsonSerializerOptions()
        serializeOption.Encoder <- JavaScriptEncoder.Create(UnicodeRanges.All)
        serializeOption.WriteIndented <- true
        let jsonData = JsonSerializer.Serialize(reportDirNameDictionary, serializeOption)
        File.WriteAllText(reportDirNameConfig, jsonData)

    let GetFileAssociationDictionary () =
        let mutable fileAssociationDictionary = new Dictionary<string, string>()
        if File.Exists(PathInfo.fileAssociationConfig) then
            let config = File.ReadAllText(PathInfo.fileAssociationConfig)
            let serializeOption = new JsonSerializerOptions()
            serializeOption.Encoder <- JavaScriptEncoder.Create(UnicodeRanges.All)
            serializeOption.WriteIndented <- true
            fileAssociationDictionary <- JsonSerializer.Deserialize<Dictionary<string, string>>(config, serializeOption)
        fileAssociationDictionary

    let SaveFileAssociationConfig (fileAssociationDictionary: Dictionary<string, string>) =
        let serializeOption = new JsonSerializerOptions()
        serializeOption.Encoder <- JavaScriptEncoder.Create(UnicodeRanges.All)
        serializeOption.WriteIndented <- true
        let jsonData = JsonSerializer.Serialize(fileAssociationDictionary, serializeOption)
        File.WriteAllText(PathInfo.fileAssociationConfig, jsonData)

