using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonaForMe.DomainCore.DBModel
{
    public class OrderHour : BaseEntity
    {
        [Required]
        public string Text { get; set; }

        public string Description { get; set; }
    }
}