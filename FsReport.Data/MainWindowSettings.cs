namespace FsReport.Data
{
    public class MainWindowSettings
    {
        public double MainWindowWidth { get; set; }

        public double MainWindowHeight { get; set; }

        public int ReportTypeIndex { get; set; }

        public string? ReportFileName { get; set; }

        public bool AlwaysOnTop { get; set; }

        public bool AutoOpenReportFile { get; set; }
    }
}
