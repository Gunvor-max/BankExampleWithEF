﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BankLib.Model;

[Table("BankExampleWithEF_Account")]
public partial class Account
{
    public Account(int accountId, int mainAccountId, string name, string type, int balance, decimal interestrate)
    {
        AccountId = accountId;
        MainAccountId = mainAccountId;
        Name = name;
        Type = type;
        Balance = balance;
        Interestrate = interestrate;
    }

    public Account():this(0,0,"Default","Default",0,0)
    {
    }

    [Key]
    [Column("Account_Id")]
    public int AccountId { get; set; }

    [Column("MainAccount_Id")]
    public int MainAccountId { get; set; }

    [Required]
    [StringLength(30)]
    public string Name { get; set; }

    [Required]
    public string Type { get; set; }

    public int Balance { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal Interestrate { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<Transaction> BankExampleWithEfTransactions { get; set; } = new List<Transaction>();

    [ForeignKey("MainAccountId")]
    [InverseProperty("BankExampleWithEfAccounts")]
    public virtual MainAccount MainAccount { get; set; }
}