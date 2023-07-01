using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections.Global
{
    public class GlobalSection : ISection
    {
        public string TagName => "GlobalSection";

        public virtual string Attribute { get; }

        public virtual string Value { get; }

        public virtual IEnumerable<ISection> SubSections { get; }

        private GlobalItemSection _parent;
        public ISection Parent => _parent;

        public void SetParent(GlobalItemSection parent)
        {
            _parent = parent;
        }

        public GlobalSection() { }

        public GlobalSection(GlobalItemSection parent)
        {
            _parent = parent;
        }

        public GlobalSection(GlobalItemSection parent, string attribute, string value, List<ISection> sections)
            : this(parent)
        {
            Attribute = attribute;
            Value = value;
            SubSections = sections;
        }
    }
}
