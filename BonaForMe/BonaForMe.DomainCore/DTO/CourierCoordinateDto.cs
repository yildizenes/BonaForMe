using System;

namespace BonaForMe.DomainCore.DTO
{
    public class CourierCoordinateDto : DtoBaseEntity
    {
        public Guid UserId { get; set; }

        public decimal XCoordinate { get; set; }

        public decimal YCoordinate { get; set; }
    }
}