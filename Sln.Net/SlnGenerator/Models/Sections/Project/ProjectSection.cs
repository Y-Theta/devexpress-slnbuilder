using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections.Project
{
    /// <summary>
    /// 解决方案项目项子项
    /// </summary>
    public class ProjectSection : ISection
    {
        public string TagName => "ProjectSection";

        public virtual string Attribute { get; }

        public virtual string Value { get; }

        public virtual IEnumerable<ISection> SubSections { get; }

        private ProjectItemSection _parent;
        public ISection Parent => _parent;

        public void SetParent(ProjectItemSection parent)
        {
            _parent = parent;
        }

        public ProjectSection() { }

        public ProjectSection(ProjectItemSection parent)
        {
            _parent = parent;
        }

        public ProjectSection(ProjectItemSection parent, string attribute, string value, List<ISection> sections)
            : this(parent)
        {
            Attribute = attribute;
            Value = value;
            SubSections = sections;
        }
    }
}
