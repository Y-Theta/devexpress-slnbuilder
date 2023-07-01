using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections.Global
{
    public class ProjectConfigurationPlatformSection : GlobalSection
    {
        public override string Attribute => "ProjectConfigurationPlatforms";

        public override string Value => ConstTable.POST_SOLUTION;

        public override IEnumerable<ISection> SubSections => Configs;

        public List<GeneralSection> Configs = new List<GeneralSection>();
    }
}
