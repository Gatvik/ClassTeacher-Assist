using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassTeacher_Assist.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public int SubjectId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Patronymic { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual Class? Class { get; set; }

    public virtual ICollection<Grade> Grades { get; } = new List<Grade>();

    public virtual ICollection<Skip> Skips { get; } = new List<Skip>();

    public virtual Subject Subject { get; set; } = null!;

    //public virtual ICollection<Class> Classes { get; } = new List<Class>();
    [NotMapped]
    public string FullNameAndClass { get { return $"{LastName} {FirstName} {Patronymic} {Common.GetTeachersClassCode(TeacherId)}"; } }
    public virtual ICollection<Teaching> Teachings { get; } = new List<Teaching>();
}
