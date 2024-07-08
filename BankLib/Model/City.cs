﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BankLib.Model;

[Table("BankExampleWithEF_City")]
public partial class City
{
    public City(int cityId, int zipCodeId, string cityName, ZipCodeTable zipCode)
    {
        CityId = cityId;
        ZipCodeId = zipCodeId;
        CityName = cityName;
        ZipCode = zipCode;
    }

    public City():this(0,0,"Default",new ZipCodeTable())
    {
        
    }

    [Key]
    [Column("City_Id")]
    public int CityId { get; set; }

    [Column("ZipCode_ID")]
    public int ZipCodeId { get; set; }

    [Required]
    [StringLength(30)]
    public string CityName { get; set; }

    [InverseProperty("City")]
    public virtual ICollection<Address> BankExampleWithEfAddresses { get; set; } = new List<Address>();

    [ForeignKey("ZipCodeId")]
    [InverseProperty("BankExampleWithEfCities")]
    public virtual ZipCodeTable ZipCode { get; set; }

    public override string ToString()
    {
        return $"{{Class=City,{nameof(ZipCodeId)}={ZipCodeId.ToString()},{nameof(CityName)}={CityName},{nameof(ZipCode)}={ZipCode}}}";
    }
}