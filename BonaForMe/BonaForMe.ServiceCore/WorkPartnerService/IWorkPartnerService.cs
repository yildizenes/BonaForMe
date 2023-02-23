using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.WorkPartnerService
{
    public interface IWorkPartnerService
    {
        Result<WorkPartnerDto> AddWorkPartner(WorkPartnerDto workPartnerDto);
        Result DeleteWorkPartner(Guid id);
        Result<WorkPartnerDto> UpdateWorkPartner(WorkPartnerDto workPartnerDto);
        Result<WorkPartnerDto> GetWorkPartnerById(Guid id);
        Result<List<WorkPartnerDto>> GetAllWorkPartner();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
