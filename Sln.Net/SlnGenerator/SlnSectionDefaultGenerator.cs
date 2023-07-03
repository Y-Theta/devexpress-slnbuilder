using System;
using System.Collections.Generic;

using VsSolutionGenerator.SlnGenerator.Models.Sections;
using VsSolutionGenerator.SlnGenerator.Models.Sections.Global;
using VsSolutionGenerator.SlnGenerator.Models.Sections.Project;

namespace VsSolutionGenerator.SlnGenerator
{
    public class SlnSectionDefaultGenerator
    {

        public static SolutionConfigurationPlatformSection GenerateDefaultSolutionConfigPlatform()
        {
            return new SolutionConfigurationPlatformSection
            {
                Configs = new List<GeneralSection>
                {
                    new GeneralSection("Debug|Any CPU", "Debug|Any CPU"),
                    new GeneralSection("Release|Any CPU", "Release|Any CPU"),
                    new GeneralSection("Debug|x64", "Debug|x64"),
                    new GeneralSection("Release|x64", "Release|x64"),
                }
            };
        }

        public static ProjectConfigurationPlatformSection GenerateDefaultProjectConfigurationPlatformSection(
            IEnumerable<ProjectItemSection> projects, 
            IEnumerable<GeneralSection> configs)
        {
            ProjectConfigurationPlatformSection projConfigs = new ProjectConfigurationPlatformSection();

            foreach (var item in projects)
            {
                foreach (var cfg in configs)
                {
                    projConfigs.Configs.Add(new GeneralSection($"{item.Guid.ToString("B")}.{cfg.Key}.ActiveCfg", $"{cfg.Value}"));
                    projConfigs.Configs.Add(new GeneralSection($"{item.Guid.ToString("B")}.{cfg.Key}.Build.0", $"{cfg.Value}"));
                }
            }

            return projConfigs;
        }

        public static SolutionPropertySection GenerateDefaultSolutionPropertySection()
        {
            SolutionPropertySection projConfigs = new SolutionPropertySection();
            projConfigs.Properties.Add(new GeneralSection("HideSolutionNode", "FALSE"));
            return projConfigs;
        }

        public static ExtensibilityGlobalSection GenerateDefaultExtensibilityGlobalSection()
        {
            ExtensibilityGlobalSection projConfigs = new ExtensibilityGlobalSection();
            projConfigs.Properties.Add(new GeneralSection("SolutionGuid", Guid.NewGuid().ToString("B")));
            return projConfigs;
        }
    }
}
