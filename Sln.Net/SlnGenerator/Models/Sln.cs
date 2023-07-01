using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VsSolutionGenerator.SlnGenerator.Models.Sections.Global;
using VsSolutionGenerator.SlnGenerator.Models.Sections.Project;

namespace VsSolutionGenerator.SlnGenerator.Models
{
    /// <summary>
    /// Sln 文件
    /// </summary>
    public class Sln
    {
        public string FileName { get; set; }

        public VisualStudioHeader Header { get; set; }

        public List<ProjectItemSection> Projects { get; set; }

        public GlobalItemSection Global { get; set; }
    }
}
