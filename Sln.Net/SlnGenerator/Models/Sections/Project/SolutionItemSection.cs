using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections.Project
{
    /// <summary>
    /// 解决方案项
    /// </summary>
    public class SolutionItemSection : ProjectSection
    {
        public override string Attribute => "SolutionItems";

        public override string Value => ConstTable.PRE_PROJECT;

        public override IEnumerable<ISection> SubSections
        {
            get
            {
                if (ItemPaths == null || !ItemPaths.Any())
                {
                    return null;
                }
                return ItemPaths.Select(path => new GeneralSection(path, path));
            }
        }

        public List<string> ItemPaths { get; set; }
    }
}
