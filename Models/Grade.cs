using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassTeacher_Assist.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public int StudentId { get; set; }

    public int TeacherId { get; set; }

    public int Value { get; set; }

    public DateTime ReceivingDate { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
    [NotMapped]
    public string GradeInfo { get { return $"{Student.FullName} {ReceivingDateNormal} {Value} {Teacher.Subject.Code}"; } }
    [NotMapped]
    public string ReceivingDateNormal { get { return ReceivingDate.ToString("dd.MM.yyyy"); } }
}
