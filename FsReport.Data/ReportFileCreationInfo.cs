namespace FsReport.Data
{
    public enum ReportType
    {
        Numbering,
        Midterm,
        Final,
        Other
    }

    public class ReportFileCreationInfo
    {
        public required string ReportRootDirectory { get; set; }

        public required string SubjectName { get; set; }

        public required string SubjectFolderName { get; set; }

        public required ReportType Type { get; set; }

        public string? ReportFolderNameOptional { get; set; }

        public required string ReportFileNameWithoutExtension { get; set; }

        public required string TemplateFileName { get; set; }
    }
}
