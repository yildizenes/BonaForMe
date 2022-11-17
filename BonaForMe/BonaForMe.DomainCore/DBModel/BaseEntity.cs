﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BonaForMe.DomainCore.DBModel
{
    public class BaseEntity : IBaseEntity
    {
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }
        [Required]
        [Column("DateCreated")]
        public DateTime? DateCreated { get; set; }
        [Required]
        [MaxLength(64)]
        [Column("UserCreated")]
        public Guid UserCreated { get; set; }
        [Required]
        [Column("DateModified")]
        public DateTime? DateModified { get; set; }
        [Required]
        [MaxLength(64)]
        [Column("UserModified")]
        public Guid UserModified { get; set; }
        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; }
        [Required]
        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
}
