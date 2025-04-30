using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;


namespace CRM_system.Models
{
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(100), SQLite.NotNull]
        public string Name { get; set; } = string.Empty;

        [SQLite.NotNull]
        public decimal WholesalePrice { get; set; }

        [SQLite.NotNull]
        public decimal RetailPrice { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
