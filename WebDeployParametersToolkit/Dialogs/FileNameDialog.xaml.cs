using System.Windows;
using System.Windows.Input;

namespace WebDeployParametersToolkit
{
    /// <summary>
    /// Interaction logic for FileNameDialog.xaml
    /// </summary>
    public partial class FileNameDialog : Window
    {
        public FileNameDialog()
        {
            InitializeComponent();

            EnvironmentName.Focus();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public string Input
        {
            get
            {
                return $"SetParameters{EnvironmentName.Text.TrimEnd()}.xml";
            }
        }

        private void EnvironmentName_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            var cleanText = string.Join(string.Empty, EnvironmentName.Text.Split(System.IO.Path.GetInvalidFileNameChars()));
            if (cleanText != EnvironmentName.Text)
            {
                EnvironmentName.Text = cleanText;
                EnvironmentName.SelectionStart = cleanText.Length;
                EnvironmentName.SelectionLength = 0;
            }
            FileName.Content = $"SetParameters{EnvironmentName.Text}.xml";
        }
    }
}