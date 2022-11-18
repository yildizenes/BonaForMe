using BonaForMe.DomainCore.DBModel;
using System;

namespace BonaForMe.DomainCore.DTO
{
    public class AnnouncementDto : DtoBaseEntity
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }

        public virtual Category Category { get; set; }
    }
}