using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class UpdateVipRequestDTO 
    {
        public int NumberMonthsRegis { get; set; }
        public decimal Amount { get; set; } = 100000; //100k 1 thang
    }
}
