using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections.Global
{
    public class ExtensibilityGlobalSection : GlobalSection
    {
        public override string Attribute => "ExtensibilityGlobals";

        public override string Value => ConstTable.POST_SOLUTION;

        public override IEnumerable<ISection> SubSections => Properties;

        public List<GeneralSection> Properties { get; set; } = new List<GeneralSection>();
    }
}
