namespace FsReport.Desktop.Dialogs
{
    /// <summary>
    /// AboutDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();

            NameBlock.Text = VersionInfo.Name;
            VersionBlock.Text = $"Version.{VersionInfo.Version}";
            CopyrightBlock.Text = VersionInfo.Copyright;
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
