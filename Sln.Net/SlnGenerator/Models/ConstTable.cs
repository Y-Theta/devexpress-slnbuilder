using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models
{
    public class ConstTable
    {
        public const string PRE_PROJECT = "preProject";
        public const string POST_PROJECT = "postProject";

        public const string PRE_SOLUTION = "preSolution";
        public const string POST_SOLUTION = "postSolution";

        public static readonly Guid PROJ_TYPE_C_SHARP = new Guid("9A19103F-16F7-4668-BE54-9A1E7A4F7556");
        public static readonly Guid PROJ_TYPE_FOLDER = new Guid("2150E333-8FDC-42A3-9474-1A3956D46DE8");
    }
}
