using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BonaForMe.DomainCore.DBModel
{
    public interface IBaseEntity
    {
        Guid Id { get; set; }
        DateTime DateCreated { get; set; }
        Guid UserCreated { get; set; }
        DateTime? DateModified { get; set; }
        Guid UserModified { get; set; }
        bool IsActive { get; set; }
        bool IsDeleted { get; set; }
    }
}
