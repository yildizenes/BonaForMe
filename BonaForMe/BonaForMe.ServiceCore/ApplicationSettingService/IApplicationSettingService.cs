using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.ApplicationSettingService
{
    public interface IApplicationSettingService
    {
        Result<ApplicationSettingDto> AddApplicationSetting(ApplicationSettingDto applicationSettingDto);
        Result DeleteApplicationSetting(Guid id);
        Result<ApplicationSettingDto> UpdateApplicationSetting(ApplicationSettingDto applicationSettingDto);
        Result<ApplicationSettingDto> GetApplicationSetting();
        Result<List<ApplicationSettingDto>> GetAllApplicationSetting();
        JsonResult FillDataTable(DataTableDto dataTable);
        Result<ApplicationSettingDto> SaveMinimumOrderValue(int minimumOrderValue);
    }
}
