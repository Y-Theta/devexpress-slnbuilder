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

namespace VsSolutionGenerator.DevSCR
{
    internal class DevSolutionGenerator
    {
        private StringBuilder _console = new StringBuilder();

        public Sln GenerateSolution(string folder)
        {
            Sln sln = new Sln();
            sln.Header = VsHeaderInfoTable.VS_2022_Enterprise_17_6;
            sln.Projects = new List<ProjectItemSection>();

            Dictionary<ProjectItemSection, List<ProjectItemSection>> treeStruct = new Dictionary<ProjectItemSection, List<ProjectItemSection>>();
            var tree = GenerateProjectTree(folder, treeStruct);
            foreach (var item in tree)
            {
                sln.Projects.Add(item);
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

        private readonly string _ext = ".csproj";
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
        private List<ProjectItemSection> ResolveBuildOrder()
        {

        }
    }
}
