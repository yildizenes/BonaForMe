using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.AnnouncementService
{
    public interface IAnnouncementService
    {
        Result<AnnouncementDto> AddAnnouncement(AnnouncementDto announcementDto);
        Result DeleteAnnouncement(Guid id);
        Result<AnnouncementDto> UpdateAnnouncement(AnnouncementDto announcementDto);
        Result<AnnouncementDto> GetAnnouncementById(Guid id);
        Result<List<AnnouncementDto>> GetAllAnnouncement();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
