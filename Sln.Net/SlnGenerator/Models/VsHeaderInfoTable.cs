
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models
{
    public class VsHeaderInfoTable
    {

        public static VisualStudioHeader VS_2022_Enterprise_17_6
        {
            get
            {
                VisualStudioHeader header = new VisualStudioHeader();
                header.FormatVersion = "12.00";
                header.VisualStudioVersion = "17.6.33801.468";
                header.MinimumVisualStudioVersion = "10.0.40219.1";
                return header;
            }
        }


    }
}
