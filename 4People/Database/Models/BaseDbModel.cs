using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace _4People.Database.Models
{
    public class BaseDbModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }
}