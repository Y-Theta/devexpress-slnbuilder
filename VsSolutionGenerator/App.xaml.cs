
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using VsSolutionGenerator.DevSCR;
using VsSolutionGenerator.SlnGenerator;
using VsSolutionGenerator.SlnGenerator.Models;
using VsSolutionGenerator.SlnGenerator.Models.Sections.Global;
using VsSolutionGenerator.SlnGenerator.Models.Sections.Project;

namespace VsSolutionGenerator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            DevSolutionGenerator resolver = new DevSolutionGenerator();
            resolver.OldKey = "0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2";
            resolver.NewKey = "002400000480000094000000060200000024000052534131000400000100010079f66415bde7deb6b08f5f4f28d495cb2fef22d8ead0caa23355ba7e7cac85fb92f8eff4a8e4069ca592db888e470e844be616a04e94fb136f5e0baaf0df46d4a52c2d59662fd5ee7f275f9d80aba619a984fcca2651acefc63f3093904072d3ab1815c043b0449dd5a785ca58bc27820c3ac5b1a3a25ecf4b113c900be97bba";
            var sln = resolver.GenerateSolution(@"F:\Devexpress\23.1\Sources\Win", new Dictionary<string, List<string>>
            {
                ["Folder.AspNet"] = new List<string>
                {
                    @"..\DevExpress.AspNetCore.Common\DevExpress.AspNetCore.Common.csproj",
                    @"..\DevExpress.Web\DevExpress.AspNetCore.Core.csproj",
                    @"..\DevExpress.Web.Resources\DevExpress.AspNetCore.Resources.csproj",
                }
            });
            var slnFile = @"F:\Devexpress\23.1\Sources\Win\DevExpress.NetCore.Desktop.sln";
            if (File.Exists(slnFile))
            {
                File.Delete(slnFile);
            }
            var stream = File.Open(slnFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using (var slnw = new SlnWriter(stream))
            {
                slnw.Write(sln);
                slnw.Flush();
                stream.Position = 0;
            }

            //resolver.OverridePublicKey(@"F:\Devexpress\23.1\Sources\Win",
            //    "0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2",
            //    "002400000480000094000000060200000024000052534131000400000100010079f66415bde7deb6b08f5f4f28d495cb2fef22d8ead0caa23355ba7e7cac85fb92f8eff4a8e4069ca592db888e470e844be616a04e94fb136f5e0baaf0df46d4a52c2d59662fd5ee7f275f9d80aba619a984fcca2651acefc63f3093904072d3ab1815c043b0449dd5a785ca58bc27820c3ac5b1a3a25ecf4b113c900be97bba");
            //var xmlReader = new XmlTextReader(new StringReader(File.ReadAllText(@"D:\Program\MyProj\C#\Cli\Project1\ClassLibrary2\bin\Debug\net47\sdsd.xml")));
            //xmlReader.Namespaces = false;
            //var c = XElement.Load(xmlReader);
            //var nodes = c.XPathSelectElements(@"//ItemGroup/Reference[@Include]");
            //List<string> projReference = new List<string>();
            //foreach (var ele in nodes)
            //{
            //    var include = ele.XPathEvaluate(@"@Include") as IEnumerable<object>;
            //    var attr = include.FirstOrDefault(x => x is XAttribute) as XAttribute;
            //    if (attr != null)
            //    {
            //        projReference.Add(attr.Value);
            //    }
            //}

            base.OnStartup(e);
        }

       
    }
}
