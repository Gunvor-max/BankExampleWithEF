﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BankLib.Model;

[Table("BankExampleWithEF_Department")]
public partial class Department
{
    public Department(int departmentId, string name, string description)
    {
        DepartmentId = departmentId;
        Name = name;
        Description = description;
    }

    public Department():this(0,"DefaultAfdeling","DefaultBeskrivelse")
    {
        
    }

    [Key]
    [Column("Department_Id")]
    public int DepartmentId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [InverseProperty("Department")]
    public virtual ICollection<Employee> BankExampleWithEfEmployees { get; set; } = new List<Employee>();
}