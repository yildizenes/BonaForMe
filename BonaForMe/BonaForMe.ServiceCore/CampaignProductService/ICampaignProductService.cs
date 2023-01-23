using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.CampaignProductService
{
    public interface ICampaignProductService
    {
        Result<CampaignProductDto> AddCampaignProduct(CampaignProductDto campaignProductDto);
        Result DeleteCampaignProduct(Guid id);
        Result<CampaignProductDto> UpdateCampaignProduct(CampaignProductDto campaignProductDto);
        Result<CampaignProductDto> GetCampaignProductById(Guid id);
        Result<List<CampaignProductDto>> GetAllCampaignProduct();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
