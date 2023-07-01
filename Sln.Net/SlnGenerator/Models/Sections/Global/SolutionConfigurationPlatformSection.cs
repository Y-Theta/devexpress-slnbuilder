using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections.Global
{

    public class SolutionConfigurationPlatformSection : GlobalSection
    {
        public override string Attribute => "SolutionConfigurationPlatforms";

        public override string Value => ConstTable.PRE_SOLUTION;

        public override IEnumerable<ISection> SubSections => Configs;

        public List<GeneralSection> Configs { get; set; } = new List<GeneralSection>();
    }
}
