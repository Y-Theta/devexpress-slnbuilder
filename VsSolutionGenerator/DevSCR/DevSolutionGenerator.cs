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
using System.Windows.Input;
using System.Windows.Controls;
using System.Security;

namespace VsSolutionGenerator.DevSCR
{
    internal class DevSolutionGenerator
    {
        private readonly string _ext = ".csproj";
        private readonly string _version = ".v23.1";
        private StringBuilder _console = new StringBuilder();

        private string[] _ignoreDirs = new string[]
        {
            ".vs",
            "obj",
            "debug",
            "release",
            "obj.ncd",
            "obj.nc",
        };

        public string OldKey { get; set; }
        public string NewKey { get; set; }

        public Sln GenerateSolution(string folder,
            Dictionary<string, List<string>> addonPorjects = null,
            Dictionary<string, List<(string key, Stream value)>> inMemoryFile = null)
        {
            Sln sln = new Sln();
            OverridePublicKey(folder, OldKey, NewKey);
            sln.Header = VsHeaderInfoTable.VS_2022_Enterprise_17_6;
            Dictionary<ProjectItemSection, List<ProjectItemSection>> treeStruct = new Dictionary<ProjectItemSection, List<ProjectItemSection>>();
            GenerateProjectSection(folder, sln, treeStruct, addonPorjects, inMemoryFile);

            sln.Global = new GlobalItemSection();
            var solutionConfigs = SlnSectionDefaultGenerator.GenerateDefaultSolutionConfigPlatform();
            sln.Global.GlobalSections.Add(solutionConfigs);
            //sln.Global.GlobalSections.Add(SlnSectionDefaultGenerator.GenerateDefaultProjectConfigurationPlatformSection(sln.Projects, solutionConfigs.Configs));
            sln.Global.GlobalSections.Add(SlnSectionDefaultGenerator.GenerateDefaultSolutionPropertySection());
            var nestedProjs = new NestedProjectSection();
            nestedProjs.ProjectMap = treeStruct;
            sln.Global.GlobalSections.Add(nestedProjs);
            sln.Global.GlobalSections.Add(SlnSectionDefaultGenerator.GenerateDefaultExtensibilityGlobalSection());

            return sln;
        }

        /// <summary>
        /// 生成 解决方案的工程项
        /// </summary>
        private void GenerateProjectSection(string folder,
            Sln sln,
            Dictionary<ProjectItemSection, List<ProjectItemSection>> treeStruct,
            Dictionary<string, List<string>> addonPorjects = null,
            Dictionary<string, List<(string key, Stream value)>> inMemoryFile = null)
        {
            Dictionary<string, List<string>> projRefs = new Dictionary<string, List<string>>();
            var tree = GenerateProjectTree(folder, treeStruct);
            foreach (var item in tree)
            {
                if (item.DType != ConstTable.PROJ_TYPE_FOLDER)
                {
                    var kv = MatchDllReference(File.ReadAllText(Path.Combine(folder, item.Path)));
                    if (kv.guid != null)
                    {
                        item.SetGuid(new Guid(kv.guid));
                    }
                    item.OutputName = item.Name;
                    if (!string.IsNullOrEmpty(kv.asbName))
                    {
                        item.OutputName = kv.asbName;
                    }
                    projRefs[item.Guid.ToString("B")] = kv.refs;
                }
                sln.AddProject(item);
            }

            if (addonPorjects != null)
            {
                foreach (var proj in addonPorjects)
                {
                    ProjectItemSection folderSection = null;
                    if (proj.Key != string.Empty)
                    {
                        folderSection = new ProjectItemSection();
                        folderSection.DType = ConstTable.PROJ_TYPE_FOLDER;
                        folderSection.Name = $"{proj.Key}";
                        folderSection.Path = folderSection.Name;
                    }

                    List<ProjectItemSection> subProjs = new List<ProjectItemSection>();
                    if (proj.Value != null && proj.Value.Count > 0)
                    {
                        foreach (var item in proj.Value)
                        {
                            var projPath = Path.Combine(folder, item);
                            if (!File.Exists(projPath))
                            {
                                continue;
                            }
                            ProjectItemSection project = new ProjectItemSection();
                            FileInfo info = new FileInfo(projPath);
                            OverridePublicKey(info.DirectoryName);
                            var kv = MatchDllReference(File.ReadAllText(projPath));
                            if (kv.guid != null)
                            {
                                project.SetGuid(new Guid(kv.guid));
                            }
                            project.Name = info.Name.Replace(info.Extension, "");
                            project.DType = ConstTable.PROJ_TYPE_C_SHARP;
                            project.Path = item;
                            project.OutputName = project.Name;
                            if (!string.IsNullOrEmpty(kv.asbName))
                            {
                                project.OutputName = kv.asbName;
                            }
                            projRefs[project.Guid.ToString("B")] = kv.refs;
                            subProjs.Add(project);
                            sln.AddProject(project);
                        }
                    }

                    if (folderSection != null && subProjs.Count > 0)
                    {
                        sln.AddProject(folderSection);
                        treeStruct[folderSection] = subProjs;
                    }
                }
            }

            foreach (var proj in sln.Projects)
            {
                if (proj.DType == ConstTable.PROJ_TYPE_FOLDER)
                    continue;
                var guidb = proj.Guid.ToString("B");
                if (projRefs.ContainsKey(guidb))
                {
                    ResolveBuildOrder(sln, proj, projRefs[guidb]);
                    continue;
                }
                ResolveBuildOrder(sln, proj, null);
            }
        }

        public ISet<string> CheckSolutionNotFoundReferences(string folder, 
            Dictionary<string, List<string>> addonPorjects = null)
        {
            Sln sln = new Sln();
            Dictionary<string, List<string>> projRefs = new Dictionary<string, List<string>>();
            Dictionary<ProjectItemSection, List<ProjectItemSection>> treeStruct = 
                new Dictionary<ProjectItemSection, List<ProjectItemSection>>();
            HashSet<string> notFoundedRefs = new HashSet<string>();
            var tree = GenerateProjectTree(folder, treeStruct);
            foreach (var item in tree)
            {
                if (item.DType != ConstTable.PROJ_TYPE_FOLDER)
                {
                    var kv = MatchDllReference(File.ReadAllText(Path.Combine(folder, item.Path)));
                    if (kv.guid != null)
                    {
                        item.SetGuid(new Guid(kv.guid));
                    }
                    item.OutputName = item.Name;
                    if (!string.IsNullOrEmpty(kv.asbName))
                    {
                        item.OutputName = kv.asbName;
                    }
                    projRefs[item.Guid.ToString("B")] = kv.refs;
                }
                sln.AddProject(item);
            }

            if (addonPorjects != null)
            {
                foreach (var proj in addonPorjects)
                {
                    if (proj.Value != null && proj.Value.Count > 0)
                    {
                        foreach (var item in proj.Value)
                        {
                            var projPath = Path.Combine(folder, item);
                            if (!File.Exists(projPath))
                            {
                                continue;
                            }
                            ProjectItemSection project = new ProjectItemSection();
                            FileInfo info = new FileInfo(projPath);
                            var kv = MatchDllReference(File.ReadAllText(projPath));
                            if (kv.guid != null)
                            {
                                project.SetGuid(new Guid(kv.guid));
                            }
                            project.Name = info.Name.Replace(info.Extension, "");
                            project.DType = ConstTable.PROJ_TYPE_C_SHARP;
                            project.Path = item;
                            project.OutputName = project.Name;
                            if (!string.IsNullOrEmpty(kv.asbName))
                            {
                                project.OutputName = kv.asbName;
                            }
                            projRefs[project.Guid.ToString("B")] = kv.refs;
                            sln.AddProject(project);
                        }
                    }
                }
            }

            foreach (var proj in sln.Projects)
            {
                if (proj.DType == ConstTable.PROJ_TYPE_FOLDER)
                    continue;
                var guidb = proj.Guid.ToString("B");
                if (projRefs.ContainsKey(guidb))
                {
                    var items = ResolveBuildOrder(sln, proj, projRefs[guidb]);
                    if (items != null && items.Count > 0)
                    {
                        items.ForEach(p => notFoundedRefs.Add(p));
                    }
                    continue;
                }
                var refs = ResolveBuildOrder(sln, proj, null);
                if (refs != null && refs.Count > 0)
                {
                    refs.ForEach(p => notFoundedRefs.Add(p));
                }
            }

            return notFoundedRefs;
        }

        private List<ProjectItemSection> GenerateProjectTree(string folder, Dictionary<ProjectItemSection, List<ProjectItemSection>> tree, string relativePath = null)
        {
            List<ProjectItemSection> sections = new List<ProjectItemSection>();
            var entries = Directory.GetFileSystemEntries(folder);
            bool lastLevel = entries.Any(entry => entry.EndsWith(_ext));
            foreach (var element in entries)
            {
                //dir
                if (Directory.Exists(element) && !_ignoreDirs.Contains(element.Split(Path.DirectorySeparatorChar).Last().ToLower()) && !lastLevel) 
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
                else if (File.Exists(element) && element.EndsWith(_ext) && element.ToLower().Contains("netcore"))
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
        private List<string> ResolveBuildOrder(
            Sln sln,
            ProjectItemSection projectItem,
            List<string> dllrefs)
        {
            if (dllrefs == null)
                return null;

            List<string> notFound = new List<string>();
            List<ProjectItemSection> items = new List<ProjectItemSection>();
            foreach (var refer in dllrefs)
            {
                var name = refer;
                var dependence = sln.Projects.FirstOrDefault(p => p.OutputName == name);
                if (dependence != null)
                {
                    items.Add(dependence);
                }
                else
                {
                    notFound.Add(refer);
                }
            }

            if (items.Count > 0)
            {
                var dep = new ProjectDependenceSection();
                dep.ProjectDependencies = items;
                projectItem.ProjectSections.Add(dep);
            }

            return notFound;
        }

        /// <summary>
        /// 从项目 引用里解析 生成顺序
        /// </summary>
        private (List<string> refs, string guid, string asbName) MatchDllReference(string projectContent)
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

            string guid = null;
            var guids = c.XPathSelectElements(@"//PropertyGroup/ProjectGuid");
            if (guids.Any())
            {
                var projguid = guids.First();
                guid = projguid.Value;
            }

            string assemblyName = null;
            var asbNames = c.XPathSelectElements(@"//PropertyGroup/AssemblyName");
            if (asbNames.Any())
            {
                var asb = asbNames.First();
                assemblyName = asb.Value;
            }

            return (projReference, guid, assemblyName);
        }

        internal Dictionary<string, List<string>> GenerateAddOnProjects(IEnumerable<FolderItem> roots, string rootPath)
        {
            Dictionary<string, List<string>> addons = null;
            if (roots.Count() > 0)
            {
                addons = new Dictionary<string, List<string>>();
                foreach (var entry in roots)
                {
                    if (entry.SubEntries.Count > 0)
                    {
                        var name = entry.Name ?? string.Empty;
                        List<string> values = null;
                        if (!addons.ContainsKey(name))
                        {
                            addons[entry.Name] = new List<string>();
                        }
                        values = addons[entry.Name];

                        foreach (var path in entry.SubEntries)
                        {
                            if (string.IsNullOrEmpty(path.Path))
                            {
                                continue;
                            }

                            Uri abs = new Uri(path.Path);
                            Uri root = new Uri($"{rootPath}\\");
                            var rel = root.MakeRelativeUri(abs);
                            var relative = Uri.UnescapeDataString(rel.ToString().Replace('/', System.IO.Path.DirectorySeparatorChar));
                            values.Add(relative);
                        }
                    }
                }
            }
            return addons;
        }

        public void OverridePublicKey(string dir)
        {
            OverridePublicKey(dir, OldKey, NewKey);
        }

        /// <summary>
        /// 更新 internalvisibleto 的 publickkey
        /// </summary>
        public void OverridePublicKey(string dir, string key, string newkey)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(newkey))
                return;

            var files = Directory.GetFiles(dir, "AssemblyInfo.cs", SearchOption.AllDirectories);
            files.AsParallel().ForAll(entry =>
            {
                if (File.Exists(entry) && (entry.EndsWith(@"AssemblyInfo.cs") || entry.EndsWith("AssemblyVersion.cs")))
                {
                    var content = File.ReadAllText(entry);
                    if (content.Contains(key))
                    {
                        content = content.Replace(key, newkey);
                        File.WriteAllText(entry, content);
                    }
                }
            });
        }
    }
}
