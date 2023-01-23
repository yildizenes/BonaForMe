using AutoMapper;
using BonaForMe.DataAccessCore;
using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DBModel;
using BonaForMe.DomainCore.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;

namespace BonaForMe.ServiceCore.CampaignProductService
{
    public class CampaignProductService : ICampaignProductService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public CampaignProductService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<CampaignProductDto> AddCampaignProduct(CampaignProductDto campaignProductDto)
        {
            Result<CampaignProductDto> result = new Result<CampaignProductDto>();
            try
            {
                CampaignProduct campaignProduct = _mapper.Map<CampaignProduct>(campaignProductDto);
                if (campaignProductDto.Id != Guid.Empty)
                {
                    var oldModel = _context.CampaignProducts.FirstOrDefault(x => x.Id == campaignProductDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, campaignProduct);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(campaignProduct);
                    }
                    else
                        _context.Add(campaignProduct);
                }
                else
                    _context.Add(campaignProduct);
                _context.SaveChanges();
                result.Data = _mapper.Map<CampaignProductDto>(campaignProduct);
                result.Success = true;
                result.Message = ResultMessages.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result DeleteCampaignProduct(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.CampaignProducts.FirstOrDefault(d => d.Id == id);
                if (model is null)
                {
                    result.Success = false;
                    result.Message = ResultMessages.NonExistingData;
                    return result;
                }
                model.IsDeleted = true;
                _context.Update(model);
                _context.SaveChanges();
                result.Success = true;
                result.Message = ResultMessages.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<CampaignProductDto> UpdateCampaignProduct(CampaignProductDto campaignProductDto)
        {
            Result<CampaignProductDto> result = new Result<CampaignProductDto>();
            try
            {
                CampaignProduct campaignProduct = _mapper.Map<CampaignProduct>(campaignProductDto);
                if (campaignProductDto.Id != Guid.Empty)
                {
                    var oldModel = _context.CampaignProducts.FirstOrDefault(x => x.Id == campaignProductDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, campaignProduct);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(campaignProduct);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<CampaignProductDto>(campaignProduct);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<CampaignProductDto> GetCampaignProductById(Guid id)
        {
            Result<CampaignProductDto> result = new Result<CampaignProductDto>();
            try
            {
                var model = _context.CampaignProducts.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<CampaignProductDto>(model);
                result.Success = true;
                result.Message = ResultMessages.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<List<CampaignProductDto>> GetAllCampaignProduct()
        {
            Result<List<CampaignProductDto>> result = new Result<List<CampaignProductDto>>();
            try
            {
                var model = _context.CampaignProducts.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<CampaignProduct>, List<CampaignProductDto>>(model);
                result.Success = true;
                result.Message = ResultMessages.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public JsonResult FillDataTable(DataTableDto dataTable)
        {
            try
            {
                var campaignProducts = GetAllCampaignProduct().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    campaignProducts = campaignProducts.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    //campaignProducts = campaignProducts.Where(m => m.Name.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = campaignProducts.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = campaignProducts.Count(), recordsTotal = campaignProducts.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}