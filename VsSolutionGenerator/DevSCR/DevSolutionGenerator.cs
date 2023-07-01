using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsSolutionGenerator.SlnGenerator.Models.Sections.Project;
using VsSolutionGenerator.SlnGenerator.Models;
using VsSolutionGenerator.SlnGenerator.Models.Sections.Global;
using VsSolutionGenerator.SlnGenerator;

using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Windows;

namespace VsSolutionGenerator.DevSCR
{
    internal class DevSolutionGenerator
    {
        private readonly string _ext = ".csproj";
        private readonly string _version = ".v23.1";
        private StringBuilder _console = new StringBuilder();

        public Sln GenerateSolution(string folder)
        {
            Sln sln = new Sln();
            sln.Header = VsHeaderInfoTable.VS_2022_Enterprise_17_6;

            Dictionary<ProjectItemSection, List<ProjectItemSection>> treeStruct = new Dictionary<ProjectItemSection, List<ProjectItemSection>>();
            var tree = GenerateProjectTree(folder, treeStruct);
            foreach (var item in tree)
            {
                sln.AddProject(item);
            }

            foreach (var proj in sln.Projects)
            {
                if (proj.DType == ConstTable.PROJ_TYPE_FOLDER)
                    continue;
                ResolveBuildOrder(folder, sln, proj);
            }

            sln.Global = new GlobalItemSection();
            var solutionConfigs = SlnSectionDefaultGenerator.GenerateDefaultSolutionConfigPlatform();
            sln.Global.GlobalSections.Add(solutionConfigs);
            sln.Global.GlobalSections.Add(SlnSectionDefaultGenerator.GenerateDefaultProjectConfigurationPlatformSection(sln.Projects, solutionConfigs.Configs));
            sln.Global.GlobalSections.Add(SlnSectionDefaultGenerator.GenerateDefaultSolutionPropertySection());
            var nestedProjs = new NestedProjectSection();
            nestedProjs.ProjectMap = treeStruct;
            sln.Global.GlobalSections.Add(nestedProjs);
            sln.Global.GlobalSections.Add(SlnSectionDefaultGenerator.GenerateDefaultExtensibilityGlobalSection());

            return sln;
        }

        private List<ProjectItemSection> GenerateProjectTree(string folder, Dictionary<ProjectItemSection, List<ProjectItemSection>> tree, string relativePath = null)
        {
            List<ProjectItemSection> sections = new List<ProjectItemSection>();
            var entries = Directory.GetFileSystemEntries(folder);
            bool lastLevel = entries.Any(entry => entry.EndsWith(_ext));
            foreach (var element in entries)
            {
                //dir
                if (Directory.Exists(element) && !lastLevel)
                {
                    ProjectItemSection folderSection = new ProjectItemSection();
                    folderSection.DType = ConstTable.PROJ_TYPE_FOLDER;
                    folderSection.Name = $"Folder.{element.Split(Path.DirectorySeparatorChar).Last()}";
                    folderSection.Path = folderSection.Name;
                    var subPath = element.Replace($"{folder}{Path.DirectorySeparatorChar}", "");
                    if (relativePath != null)
                    {
                        subPath = $"{relativePath}{Path.DirectorySeparatorChar}{subPath}";
                    }
                    var subSections = GenerateProjectTree(element, tree, subPath);
                    if (!subSections.Any())
                    {
                        continue;
                    }
                    if (subSections.Count > 1)
                    {
                        sections.Add(folderSection);
                        tree.Add(folderSection, subSections);
                    }
                    sections.AddRange(subSections);
                }
                else if (File.Exists(element) && element.EndsWith(_ext) && element.Contains("NetCore"))
                {
                    FileInfo info = new FileInfo(element);
                    ProjectItemSection project = new ProjectItemSection();
                    project.Name = info.Name.Replace(info.Extension, "");
                    project.DType = ConstTable.PROJ_TYPE_C_SHARP;
                    project.Path = project.Name;
                    if (relativePath != null)
                    {
                        project.Path = $"{relativePath}{Path.DirectorySeparatorChar}{project.Path}{info.Extension}";
                    }
                    sections.Add(project);
                }
            }

            return sections;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResolveBuildOrder(string pathRoot,
            Sln sln,
            ProjectItemSection projectItem)
        {
            var path = Path.Combine(pathRoot, projectItem.Path);
            var file = File.ReadAllText(path);
            var dllrefs = MatchDllReference(file);
            if (dllrefs.Count == 0)
                return;

            List<ProjectItemSection> items = new List<ProjectItemSection>();
            foreach (var refer in dllrefs)
            {
                var name = refer.Replace(_version, "");
                var dependence = sln.Projects.FirstOrDefault(p => p.Name.Replace(".NetCore", "") == name);
                if (dependence != null)
                {
                    items.Add(dependence);
                }
            }

            if (items.Count > 0)
            {
                var dep = new ProjectDependenceSection();
                dep.ProjectDependencies = items;
                projectItem.ProjectSections.Add(dep);
            }
        }

        private List<string> MatchDllReference(string projectContent)
        {
            var xmlReader = new XmlTextReader(new StringReader(projectContent));
            xmlReader.Namespaces = false;
            var c = XElement.Load(xmlReader);
            var nodes = c.XPathSelectElements(@"//ItemGroup/Reference[@Include]");
            List<string> projReference = new List<string>();
            foreach (var ele in nodes)
            {
                var include = ele.XPathEvaluate(@"@Include") as IEnumerable<object>;
                var attr = include.FirstOrDefault(x => x is XAttribute) as XAttribute;
                if (attr != null)
                {
                    projReference.Add(attr.Value);
                }
            }

            return projReference;
        }
    }
}
