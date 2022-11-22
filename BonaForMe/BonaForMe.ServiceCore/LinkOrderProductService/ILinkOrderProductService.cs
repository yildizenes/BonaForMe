using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.LinkOrderProductService
{
    public interface ILinkOrderProductService
    {
        Result<LinkOrderProductDto> AddLinkOrderProduct(LinkOrderProductDto linkOrderProductDto);
        Result DeleteLinkOrderProduct(Guid id);
        Result<LinkOrderProductDto> UpdateLinkOrderProduct(LinkOrderProductDto linkOrderProductDto);
        Result<LinkOrderProductDto> GetLinkOrderProductById(Guid id);
        Result<List<LinkOrderProductDto>> GetAllLinkOrderProduct();
        Result<List<LinkOrderProductDto>> GetAllLinkOrderProductByOrderId(Guid id);
        Result<List<LinkOrderProductDto>> GetAllLinkOrderProductByProductId(Guid id);
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
