using System;

namespace BonaForMe.DomainCore.DTO
{
    public class DtoBaseEntity
    {
        public Guid Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid UserCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid UserModified { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
