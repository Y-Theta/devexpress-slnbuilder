using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections.Project
{
    /// <summary>
    /// 解决方案项目项
    /// </summary>
    public class ProjectItemSection : ISection, IEquatable<ProjectItemSection>
    {
        public virtual string TagName => "Project";
        public virtual string Attribute => $"\"{DType.ToString("B")}\"";

        public virtual string Value
        {
            get
            {
                List<string> str = new List<string>();
                str.Add($"\"{Name}\"");
                str.Add($"\"{Path}\"");
                str.Add($"\"{Guid.ToString("B")}\"");
                return string.Join(", ", str);
            }
        }
        public ISection Parent => null;

        public virtual IEnumerable<ISection> SubSections => ProjectSections;

        public Guid DType { get; set; } = ConstTable.PROJ_TYPE_C_SHARP;

        public string Name { get; set; }

        public string OutputName { get; set; }

        public string Path { get; set; }

        public Guid Guid { get; private set; } = Guid.NewGuid();

        public List<ProjectConfigPlatform> PlatformConfigs { get; set; }

        public List<ProjectSection> ProjectSections { get; set; } = new List<ProjectSection>();

        public bool Equals(ProjectItemSection other)
        {
            return other.DType == DType;
        }

        public void SetGuid(Guid guid)
        {
            Guid = guid;
        }

        #region   
        public static readonly ProjectItemSectionCompare Comparer = new ProjectItemSectionCompare();

        public class ProjectItemSectionCompare : IEqualityComparer<ProjectItemSection>
        {
            public bool Equals(ProjectItemSection x, ProjectItemSection y)
            {
                return x.Guid == y.Guid;
            }

            public int GetHashCode(ProjectItemSection obj)
            {
                return obj.Guid.GetHashCode();
            }
        }
        #endregion

    }
}
