using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections.Project
{
    /// <summary>
    /// 
    /// </summary>
    public class ProjectDependenceSection : ProjectSection
    {
        public override string Attribute => "ProjectDependencies";

        public override string Value => ConstTable.POST_PROJECT;


        public override IEnumerable<ISection> SubSections
        {
            get
            {
                if (ProjectDependencies == null || ProjectDependencies.Count == 0)
                    return null;

                return _dependences;
            }
        }

        public ProjectItemSection ParentProject => Parent as ProjectItemSection;

        private IEnumerable<GeneralSection> _dependences;
        private List<ProjectItemSection> _projDependence;

        public List<ProjectItemSection> ProjectDependencies
        {
            get => _projDependence;
            set
            {
                _projDependence = value;
                _dependences = ProjectDependencies.Select(p => new GeneralSection(p.Guid.ToString("B"), p.Guid.ToString("B"))).ToList();
            }
        }
    }
}
