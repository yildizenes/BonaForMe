using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.CourierCoordinateService
{
    public interface ICourierCoordinateService
    {
        Result<CourierCoordinateDto> AddCourierCoordinate(CourierCoordinateDto courierCoordinateDto);
        Result DeleteCourierCoordinate(Guid id);
        Result<CourierCoordinateDto> UpdateCourierCoordinate(CourierCoordinateDto courierCoordinateDto);
        Result<CourierCoordinateDto> GetCourierCoordinateById(Guid id);
        Result<CourierCoordinateDto> GetCourierCoordinateByCourierId(Guid id);
        Result<List<CourierCoordinateDto>> GetAllCourierCoordinate();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
