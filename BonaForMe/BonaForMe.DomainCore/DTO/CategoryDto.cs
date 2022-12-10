using Microsoft.AspNetCore.Http;

namespace BonaForMe.DomainCore.DTO
{
    public class CategoryDto : DtoBaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public IFormFile FormFile { get; set; }
    }
}