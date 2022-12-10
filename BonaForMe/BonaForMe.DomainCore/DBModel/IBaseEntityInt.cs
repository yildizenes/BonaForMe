using System;

namespace BonaForMe.DomainCore.DBModel
{
    public interface IBaseEntityInt
    {
        int Id { get; set; }
        DateTime? DateCreated { get; set; }
        Guid UserCreated { get; set; }
        DateTime? DateModified { get; set; }
        Guid UserModified { get; set; }
        bool IsActive { get; set; }
        bool IsDeleted { get; set; }
    }
}
