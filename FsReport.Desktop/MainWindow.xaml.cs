using Microsoft.Win32;
using System.Windows.Controls;

namespace FsReport.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, string>? _subjectFolderNameDictionary;
        private Dictionary<string, string>? SubjectFolderNameDictionary
        {
            get
            {
                return _subjectFolderNameDictionary;
            }

            set
            {
                _subjectFolderNameDictionary = value;
                SubjectComboBox.SelectedIndex = -1;
                SubjectComboBox.Items.Clear();
                SetSubjectComboBox();
            } 
        }

        private Dictionary<string, string>? FileAssociationDictionary { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.Title = $"{VersionInfo.Name}";
            OptionalReportFolderNameLabel.IsEnabled = false;
            OptionalReportFolderNameBox.IsEnabled = false;

            try
            {
                ReportRootDirBox.Text = SettingsConfigurator.GetReportRootDirConf();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"レポートルートディレクトリ設定の構成に失敗．\n{ex.Message}",
                    "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                var templateFilesNameList = TemplateFileHandler.GetTemplateFilesNameList();
                foreach (string templateFile in templateFilesNameList)
                {
                    TemplateFilesListBox.Items.Add(templateFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"テンプレートファイルの取得に失敗．\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                this.SubjectFolderNameDictionary = SettingsConfigurator.GetSubjectFolderNameDictionary();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                this.FileAssociationDictionary = SettingsConfigurator.GetFileAssociationDictionary();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateReport()
        {
            string reportRootDirectory = ReportRootDirBox.Text;
            string reportFileName = ReportFileNameBox.Text;
            string optionalReportFolderName = OptionalReportFolderNameBox.Text;

            if (string.IsNullOrEmpty(reportRootDirectory))
            {
                MessageBox.Show(this, "レポートルートディレクトリが空です．レポートルートディレクトリを指定してください．",
                    VersionInfo.Name,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (SubjectComboBox.SelectedIndex > -1 && ReportTypeComboBox.SelectedIndex > -1)
            {
                string? subject = ((ComboBoxItem)SubjectComboBox.SelectedItem).Content as string;
                string subjectSelectionUid = ((ComboBoxItem)SubjectComboBox.SelectedItem).Uid;
                string typeIndex = ((ComboBoxItem)ReportTypeComboBox.SelectedItem).Uid;
                ReportType reportType = ParseToReportType(typeIndex);

                if (subject == null)
                {
                    MessageBox.Show(this, "正常な科目名を取得できませんでした．",
                    "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrEmpty(reportFileName))
                {
                    MessageBox.Show(this, "レポートのファイル名を指定してください．",
                    VersionInfo.Name,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (reportType == ReportType.Other && string.IsNullOrEmpty(optionalReportFolderName))
                {
                    MessageBox.Show(this, 
                    "レポートの種類が「その他」の場合は，" +
                    "レポートフォルダ名を手動で入力する必要があります．",
                    VersionInfo.Name,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (this.SubjectFolderNameDictionary == null)
                {
                    MessageBox.Show(this, "科目に割り当てられたフォルダ名を参照できません．",
                        "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string? subjectFolderName = null;

                if (subjectSelectionUid == "AddSubject")
                {
                    AddSubjectDialog addSubjectDialog = new()
                    {
                        Owner = this
                    };

                    if (addSubjectDialog.ShowDialog() == true
                        && !string.IsNullOrEmpty(addSubjectDialog.SubjectName)
                        && !string.IsNullOrEmpty(addSubjectDialog.SubjectFolderName))
                    {
                        subject = addSubjectDialog.SubjectName;
                        subjectFolderName = addSubjectDialog.SubjectFolderName;
                        bool addResult = this.SubjectFolderNameDictionary.TryAdd(subject, subjectFolderName);
                        if (!addResult)
                        {
                            MessageBox.Show(this,
                                $"科目 '{subject}' は既に割り当てられています．", "エラー",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "新規追加する科目を設定する必要があります．", VersionInfo.Name,
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else
                {
                    bool result = this.SubjectFolderNameDictionary.TryGetValue(subject, out subjectFolderName);

                    if (!result)
                    {
                        MessageBox.Show(this, "科目に割り当てられるフォルダ名が設定されていません．" +
                            "各々の科目にはレポートを格納するフォルダの名前を設定する必要があります．",
                            VersionInfo.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                }

                if (subjectFolderName == null)
                {
                    MessageBox.Show(this, "科目に割り当てられたフォルダ名が正しい値ではありません．",
                        "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (TemplateFilesListBox.SelectedIndex > -1)
                {
                    string? templateFileName = TemplateFilesListBox.SelectedItem.ToString();

                    if (templateFileName == null)
                    {
                        MessageBox.Show(this, "テンプレートファイルの名前が正しい値ではありません．",
                        "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    ReportFileCreationInfo reportFileCreationInfo = new()
                    {
                        ReportRootDirectory = reportRootDirectory,
                        SubjectName  = subject,
                        SubjectFolderName = subjectFolderName,
                        Type = reportType,
                        ReportFolderNameOptional = reportType == ReportType.Other ? optionalReportFolderName : null,
                        ReportFileNameWithoutExtension = reportFileName,
                        TemplateFileName  = templateFileName
                    };

                    string reportFilePath = ReportFileHandler.MakeReport(reportFileCreationInfo);

                    MessageBox.Show(this, $"レポートを作成しました．\n{reportFilePath}", VersionInfo.Name,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    if (AutomaticallyOpenReportFileCheckBox.IsChecked == true)
                        OpenReportFile(reportFilePath);
                }
                else
                {
                    MessageBox.Show(this, "テンプレートファイルを選択してください．",
                    VersionInfo.Name,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;

                }
            }
            else
            {
                MessageBox.Show(this, "科目またはレポートの種類を指定してください．",
                    VersionInfo.Name,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

        private void OpenReportFile(string reportFilePath)
        {
            if (this.FileAssociationDictionary == null)
            {
                MessageBox.Show(this,
                    "ファイルの関連付け設定がありません．レポートファイルをオープンできませんでした．",
                    "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string extension = Path.GetExtension(reportFilePath);
            bool result = this.FileAssociationDictionary.TryGetValue(extension, out var associatedApplicationPath);

            if (associatedApplicationPath == null)
            {
                MessageBox.Show(this, 
                    $"拡張子 '{extension}' 関連付けられたアプリケーションパスが正しい値ではありません．",
                    "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!result)
            {
                MessageBox.Show(this, 
                    $"拡張子 '{extension}' ファイルをオープンするアプリケーションが設定されていません．" +
                    "レポートファイルを自動でオープンするにはファイルを開くアプリケーションを設定しなければいけません．",
                    VersionInfo.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            ReportFileHandler.OpenReport(reportFilePath, associatedApplicationPath);
        }

        private ReportType ParseToReportType(string typeIndex)
        {
            if (typeIndex == "Numbering")
            {
                return ReportType.Numbering;
            } 
            else if (typeIndex == "Midterm")
            {
                return ReportType.Midterm;
            } 
            else if (typeIndex == "Final")
            {
                return ReportType.Final;
            }
            else
            {
                return ReportType.Other;
            }
        }

        private void SetSubjectComboBox()
        {
            if (this.SubjectFolderNameDictionary != null)
            {
                int i = 0;
                foreach (var subjectFolderNameConfig in this.SubjectFolderNameDictionary)
                {
                    string subject = subjectFolderNameConfig.Key;
                    string subjectDirectory = subjectFolderNameConfig.Value;
                    var item = new ComboBoxItem
                    {
                        Uid = $"subject{i}",
                        Content = subject
                    };
                    SubjectComboBox.Items.Add(item);
                    i++;
                }
            }

            var separator = new Separator();
            SubjectComboBox.Items.Add(separator);

            var addItem = new ComboBoxItem()
            {
                Uid = "AddSubject",
                Content = "新規追加"
            };
            SubjectComboBox.Items.Add(addItem);
        }

        private void AboutMenuItemClick(object sender, RoutedEventArgs e)
        {
            string information =
                $"{VersionInfo.Name} ver.{VersionInfo.Version}\n\n{VersionInfo.Copyright}\n" +
                $"system: {VersionInfo.System}\nruntime: {VersionInfo.Runtime}";
            MessageBox.Show(this, information, "バージョン情報",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AlwaysOnTopMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Topmost = AlwaysOnTopMenuItem.IsChecked;
        }

        private void InputParameterButtonClick(object sender, RoutedEventArgs e)
        {
            if (!InputParameterButtonContextMenu.IsOpen)
            {
                InputParameterButtonContextMenu.PlacementTarget = InputParameterButton;
                InputParameterButtonContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                InputParameterButtonContextMenu.IsOpen = true;
            }
        }

        private void InputParameterMenuClick(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> parameterDictionary = new()
            {
                { "SubjectName", "%SubjectName%" },
                { "SubjectFolder", "%SubjectFolder%" },
                { "ReportFolder", "%ReportFolder%" },
                { "ReportType", "%ReportType%" }
            };

            string index = ((MenuItem)sender).Uid.ToString();
            string parameter = parameterDictionary[index];
            ReportFileNameBox.Text += parameter;
            _ = ReportFileNameBox.Focus();
        }

        private void ReportTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportTypeComboBox.SelectedIndex > -1)
            {
                string index = ((ComboBoxItem)ReportTypeComboBox.SelectedItem).Uid;
                OptionalReportFolderNameLabel.IsEnabled = index == "Other";
                OptionalReportFolderNameBox.IsEnabled = index == "Other";
            }
            else
            {
                OptionalReportFolderNameLabel.IsEnabled = false;
                OptionalReportFolderNameBox.IsEnabled = false;
            }
        }

        private void BrowseRootDirButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new();
            if (openFolderDialog.ShowDialog() == true)
            {
                ReportRootDirBox.Text = openFolderDialog.FolderName;
            }
        }

        private void ExitMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"レポートの作成あるいはオープンに失敗しました．\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTemplateFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "サポートされるすべてのファイル|*.tex;*.docx;*.doc;*.xlsx;*.xls;*.pptx;*.ppt;*.pynb|TeX ソースファイル|*.tex|Microsoft Office ドキュメント|*.docx;*.doc;*.xlsx;*.xls;*.pptx;*.ppt|Jupyter ソースファイル|*.ipynb"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string sourceFilePath = openFileDialog.FileName;
                try
                {
                    TemplateFileHandler.AddTemplateFile(sourceFilePath);

                    TemplateFilesListBox.Items.Clear();
                    var templateFilesNameList = TemplateFileHandler.GetTemplateFilesNameList();
                    foreach (string templateFile in templateFilesNameList)
                    {
                        TemplateFilesListBox.Items.Add(templateFile);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this, $"テンプレートファイルの追加に失敗しました．\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                SettingsConfigurator.SaveReportFolderNameConfig(this.SubjectFolderNameDictionary);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"設定の保存に失敗．\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}