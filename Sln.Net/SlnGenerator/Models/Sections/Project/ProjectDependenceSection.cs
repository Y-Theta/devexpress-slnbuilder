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

                return ProjectDependencies.Select(p => new GeneralSection(ParentProject.Guid.ToString("B"), p.Guid.ToString("B")));
            }
        }

        public ProjectItemSection ParentProject => Parent as ProjectItemSection;

        public List<ProjectItemSection> ProjectDependencies { get; set; }
    }
}
