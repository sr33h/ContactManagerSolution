using System;
using System.Collections.Generic;
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of 
    /// CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj == null) return false;
            if(obj is CountryResponse c)
            {
                return this.CountryID == c.CountryID && this.CountryName == c.CountryName;
            }
            return false;
        }

        public override int GetHashCode()
        {
           return CountryID.GetHashCode();
        }
    }


    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse
            {
                CountryID = country.CountryID,
                CountryName = country.CountryName,
            };
        }
    }

}
