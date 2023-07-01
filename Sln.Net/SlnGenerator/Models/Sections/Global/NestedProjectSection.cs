using System;
using System.Collections.Generic;
using System.Linq;
using VsSolutionGenerator.SlnGenerator.Models.Sections.Project;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections.Global
{
    public class NestedProjectSection : GlobalSection
    {
        public override string Attribute => "NestedProjects";

        public override string Value => ConstTable.PRE_SOLUTION;

        public override IEnumerable<ISection> SubSections
        {
            get
            {
                if (Relations != null)
                {
                    return Relations;
                }

                if (ProjectMap != null)
                {
                    Relations = new List<GeneralSection>();
                    foreach (var kv in ProjectMap)
                    {
                        foreach (var item in kv.Value)
                        {
                            Relations.Add(new GeneralSection(item.Guid.ToString("B"), kv.Key.Guid.ToString("B")));
                        }
                    }
                }

                return Relations;
            }
        }


        public List<GeneralSection> Relations { get; set; }

        public Dictionary<ProjectItemSection, List<ProjectItemSection>> ProjectMap { get; set; }

    }
}
