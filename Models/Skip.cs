using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassTeacher_Assist.Models;

public partial class Skip
{
    public int SkipId { get; set; }

    public int StudentId { get; set; }

    public int TeacherId { get; set; }

    public string Reason { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime ReceivingDate { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
    [NotMapped]
    public string SkipInfo { get { return $"{Student.FullName} {ReceivingDateNormal} {Reason}"; } }
    [NotMapped]
    public string ReceivingDateNormal { get { return ReceivingDate.ToString("dd.MM.yyyy"); } }
}
