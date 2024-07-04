using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKAC.Repositories
{
    public class Shift
    {
        public int ShiftID { get; set; }

        [Column(TypeName = "Nvarchar")]
        [MaxLength(50)]
        public string ShiftName { get; set; }

        [Column(TypeName = "Time")]
        public TimeSpan TimeStart { get; set; }
        [Column(TypeName = "Time")]
        public TimeSpan TimeEnd { get; set; }

        public List<Orders> orders { get; set; }
    }
}
