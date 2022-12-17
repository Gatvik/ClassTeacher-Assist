using System;
using System.Collections.Generic;

namespace ClassTeacher_Assist.Models;

public partial class Class
{
    public string ClassCode { get; set; } = null!;

    public int TeacherId { get; set; }

    public int StudentsNumber { get; set; }

    public int ClassId { get; set; }

    public virtual ICollection<Student> Students { get; } = new List<Student>();

    public virtual Teacher Teacher { get; set; } = null!;
    public virtual ICollection<Teaching> Teachings { get; } = new List<Teaching>();
}
