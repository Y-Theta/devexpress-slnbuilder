using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections.Global
{
    public class GlobalItemSection : ISection
    {
        public string TagName => "Global";

        public string Attribute => null;

        public string Value => null;

        public ISection Parent => null;

        public IEnumerable<ISection> SubSections => GlobalSections;

        public List<GlobalSection> GlobalSections { get; set; } = new List<GlobalSection>();

    }
}
