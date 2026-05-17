namespace FsReport.Core

open System.IO
open System.Reflection

module PathInfo =

    let applicationPath = Assembly.GetExecutingAssembly().Location 

    let applicationDirectory =  Path.GetDirectoryName(applicationPath)

    let reportRootDirectoryConfig = $"{applicationDirectory}\\ReportRootDirectoryConfig.xml"

    let subjectFolderNameConfig = $"{applicationDirectory}\\SubjectFolderNameConfig.json"

    let mainWindowConfig = $"{applicationDirectory}\\MainWindowSettings.json"

    let reportFileNameTemplateConfig = $"{applicationDirectory}\\ReportFileNameTemplateConfig.txt"

    let fileAssociationConfig = $"{applicationDirectory}\\FileAssociation.json"

    let templatesDirectory = $"{applicationDirectory}\\Templates"
