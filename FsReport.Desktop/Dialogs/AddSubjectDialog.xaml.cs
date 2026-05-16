namespace FsReport.Desktop.Dialogs
{
    /// <summary>
    /// AddSubjectDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AddSubjectDialog : Window
    {
        public string? SubjectName { get; set; }

        public string? SubjectFolderName { get; set; }

        public AddSubjectDialog()
        {
            InitializeComponent();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (SubjectNameBox.Text.Length > 0 && SubjectFolderNameBox.Text.Length > 0)
            {
                this.SubjectName = SubjectNameBox.Text;
                this.SubjectFolderName = SubjectFolderNameBox.Text;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show(this, "科目名か科目名に割り当てるフォルダ名を入力してください．",
                    VersionInfo.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
