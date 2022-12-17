using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassTeacher_Assist.Models
{
    public partial class Teaching
    { 
        public int ClassId { get; set; }
        public int TeacherId { get; set; }
        public virtual Class Class { get; set; } = null!;
        public virtual Teacher Teacher { get; set; } = null!;
        
    }
}
