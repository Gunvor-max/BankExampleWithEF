﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BankLib.Model;

[Table("BankExampleWithEF_EmployeeLog")]
public partial class EmployeeLog
{
    [Key]
    [Column("Log_Id")]
    public int LogId { get; set; }

    [Column("ResponsibleEmployee_Id")]
    public int ResponsibleEmployeeId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    [Required]
    [StringLength(50)]
    public string Activity { get; set; }

    [Column("AffectedEmployee_Id")]
    public int? AffectedEmployeeId { get; set; }

    [Column("AffectedCustomer_Id")]
    public int? AffectedCustomerId { get; set; }

    [ForeignKey("AffectedCustomerId")]
    [InverseProperty("BankExampleWithEfEmployeeLogs")]
    public virtual Customer AffectedCustomer { get; set; }

    [ForeignKey("AffectedEmployeeId")]
    [InverseProperty("BankExampleWithEfEmployeeLogAffectedEmployees")]
    public virtual Employee AffectedEmployee { get; set; }

    [ForeignKey("ResponsibleEmployeeId")]
    [InverseProperty("BankExampleWithEfEmployeeLogResponsibleEmployees")]
    public virtual Employee ResponsibleEmployee { get; set; }
}