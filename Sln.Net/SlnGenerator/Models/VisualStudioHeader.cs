using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models
{
    public class VisualStudioHeader
    {
        public string FormatVersion { get; set; }

        public string VisualStudioVersion { get; set; }

        public string MinimumVisualStudioVersion { get; set; }
    }
}
