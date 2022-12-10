using System;

namespace BonaForMe.DomainCore.DTO
{
    public class DtoBaseEntityInt
    {
        public int Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid UserCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid UserModified { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
