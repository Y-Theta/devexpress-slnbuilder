using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VsSolutionGenerator.DevSCR;

namespace VsSolutionGenerator.Pages
{
    /// <summary>
    /// SolutionPage.xaml 的交互逻辑
    /// </summary>
    public partial class SolutionPage : Page
    {
        public SolutionPage()
        {
            InitializeComponent();
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                dialog.SelectedPath = Folder.Text;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Folder.Text = dialog.SelectedPath;
                }
            }
        }

        private void Project_Check(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(Folder.Text) || !Directory.Exists(Folder.Text))
                return;

            DevSolutionGenerator resolver = new DevSolutionGenerator();
            var items = resolver.CheckSolutionNotFoundReferences(Folder.Text);
            StringBuilder sb = new StringBuilder();
            if (items.Any())
            {
                sb.Append("以下引用没有对应项目可生成：");
                sb.AppendLine();
                sb.Append(string.Join(Environment.NewLine, items));
            }
            else
            {
                sb.Append("当前工程中所有项目引用可闭包！");
            }
            CheckOutput.Text = sb.ToString();
        }
    }
}
