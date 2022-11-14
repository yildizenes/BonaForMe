using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.ContactInformationService
{
    public interface IContactInformationService
    {
        Result<ContactInformationDto> AddContactInformation(ContactInformationDto contactInformationDto);
        Result DeleteContactInformation(Guid id);
        Result<ContactInformationDto> UpdateContactInformation(ContactInformationDto contactInformationDto);
        Result<ContactInformationDto> GetContactInformationById(Guid id);
        Result<List<ContactInformationDto>> GetAllContactInformation();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
