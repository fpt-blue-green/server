using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class JobPlatFormPieChartDTO
    {
        public EPlatform Platform { get; set; }
        public long Value { get; set; } 
    }

    public class CommomPieChartDTO
    {
        public string Label { get; set; }
        public long Value { get; set; }
    }
}
