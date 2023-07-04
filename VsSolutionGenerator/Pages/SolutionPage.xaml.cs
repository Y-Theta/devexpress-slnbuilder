using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VsSolutionGenerator.SlnGenerator;
using Path = System.IO.Path;
using TextBox = System.Windows.Controls.TextBox;

namespace VsSolutionGenerator.Pages
{
    /// <summary>
    /// SolutionPage.xaml 的交互逻辑
    /// </summary>
    public partial class SolutionPage : Page
    {

        private ObservableCollection<FolderItem> _folderStruct = new ObservableCollection<FolderItem>();

        public SolutionPage()
        {
            InitializeComponent();
            Loaded += SolutionPage_Loaded;
        }

        private void SolutionPage_Loaded(object sender, RoutedEventArgs e)
        {
            _folderStruct = new ObservableCollection<FolderItem>();
            PART_ADDITION_TREE.ItemsSource = _folderStruct;
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
            var addons = resolver.GenerateAddOnProjects(_folderStruct, Folder.Text);
            var items = resolver.CheckSolutionNotFoundReferences(Folder.Text, addons);
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

        private void ProjItemDelete(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb)
            {
                if (tb.DataContext is FolderItem tree)
                {
                    if (tree.Parent != null)
                    {
                        tree.Parent.SubEntries.Remove(tree);
                    }
                    else
                    {
                        _folderStruct.Remove(tree);
                    }
                }
            }
        }

        private void ExtraProjItemAdd(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb)
            {
                if(tb.DataContext is FolderItem tree)
                {
                    var item = new FolderItem { Name = "", Parent = tree };
                    tree.SubEntries.Add(item);
                    item.IsFolder = false;
                }
            }
        }

        private void AddProjFolder(object sender, MouseButtonEventArgs e)
        {
            _folderStruct.Add(new FolderItem { IsFolder = true });
        }

        private void SelectItemProj(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (tb.DataContext is FolderItem tree)
                {
                    using (OpenFileDialog dialog = new OpenFileDialog())
                    {
                        dialog.InitialDirectory = Folder.Text;
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            tree.Path = dialog.FileName;
                            tree.Name = tree.Path.Split(System.IO.Path.DirectorySeparatorChar).Last();
                        }
                    }
                }
            }
        }

        private void BuildSln(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(Folder.Text))
                return;
            if (string.IsNullOrEmpty(SlnName.Text))
                return;

            try
            {
                DevSolutionGenerator gen = new DevSolutionGenerator();
                gen.OldKey = OldPubKey.Text;
                gen.NewKey = NewPubKey.Text;
                var addons = gen.GenerateAddOnProjects(_folderStruct, Folder.Text);
                var sln = gen.GenerateSolution(Folder.Text, addons);

                var finalFile = Path.Combine(Folder.Text, $"{SlnName.Text.Replace(".sln", "")}.sln");
                if (File.Exists(finalFile))
                {
                    File.Delete(finalFile);
                }
                using (var slnWriter = new SlnWriter(File.Open(finalFile, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                {
                    slnWriter.Write(sln);
                    slnWriter.Flush();
                }
            }
            catch(Exception ex)
            {

            }
       
        }
    }
}
