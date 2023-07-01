using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections.Global
{
    public class SolutionPropertySection : GlobalSection
    {
        public override string Attribute => "SolutionProperties";

        public override string Value => ConstTable.PRE_SOLUTION;

        public override IEnumerable<ISection> SubSections => Properties;

        public List<GeneralSection> Properties { get; set; } = new List<GeneralSection>();
    }
}
