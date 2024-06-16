﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BankLib.Model;

[Table("BankExampleWithEF_Position")]
public partial class Position
{
    public Position(int positionId, string title, string description, int accessLevel)
    {
        PositionId = positionId;
        Title = title;
        Description = description;
        AccessLevel = accessLevel;
    }

    public Position():this(0,"DefaultTitel","DefaultBeskrivelse",0)
    {
        
    }

    [Key]
    [Column("Position_Id")]
    public int PositionId { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public int AccessLevel { get; set; }

    [InverseProperty("Position")]
    public virtual ICollection<Employee> BankExampleWithEfEmployees { get; set; } = new List<Employee>();
}