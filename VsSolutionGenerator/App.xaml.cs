
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
            var sln = resolver.GenerateSolution(@"F:\Devexpress\23.1\Sources\Win");
            var stream = File.Open(@"F:\Devexpress\23.1\Sources\Win\DevExpress.NetCore.Desktop.sln", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using (var slnw = new SlnWriter(stream))
            {
                slnw.Write(sln);
                slnw.Flush();
                stream.Position = 0;
             
            } 
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
