using BonaForMe.DomainCore.DBModel;
using System;

namespace BonaForMe.DomainCore.DTO
{
    public class CourierCoordinateDto : DtoBaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public decimal XCoordinate { get; set; }

        public decimal YCoordinate { get; set; }
    }
}