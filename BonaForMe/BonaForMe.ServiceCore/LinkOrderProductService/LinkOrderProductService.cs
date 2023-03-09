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

namespace BonaForMe.ServiceCore.LinkOrderProductService
{
    public class LinkOrderProductService : ILinkOrderProductService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public LinkOrderProductService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<LinkOrderProductDto> AddLinkOrderProduct(LinkOrderProductDto linkOrderProductDto)
        {
            Result<LinkOrderProductDto> result = new Result<LinkOrderProductDto>();
            try
            {
                LinkOrderProduct linkOrderProduct = _mapper.Map<LinkOrderProduct>(linkOrderProductDto);
                if (linkOrderProductDto.Id != Guid.Empty)
                {
                    var oldModel = _context.LinkOrderProducts.FirstOrDefault(x => x.Id == linkOrderProductDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, linkOrderProduct);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(linkOrderProduct);
                    }
                    else
                        _context.Add(linkOrderProduct);
                }
                else
                    _context.Add(linkOrderProduct);
                _context.SaveChanges();
                result.Data = _mapper.Map<LinkOrderProductDto>(linkOrderProduct);
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

        public Result DeleteLinkOrderProduct(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.LinkOrderProducts.FirstOrDefault(d => d.Id == id);
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

        public Result<LinkOrderProductDto> UpdateLinkOrderProduct(LinkOrderProductDto linkOrderProductDto)
        {
            Result<LinkOrderProductDto> result = new Result<LinkOrderProductDto>();
            try
            {
                LinkOrderProduct linkOrderProduct = _mapper.Map<LinkOrderProduct>(linkOrderProductDto);
                if (linkOrderProductDto.Id != Guid.Empty)
                {
                    var oldModel = _context.LinkOrderProducts.FirstOrDefault(x => x.Id == linkOrderProductDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, linkOrderProduct);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(linkOrderProduct);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<LinkOrderProductDto>(linkOrderProduct);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<LinkOrderProductDto> GetLinkOrderProductById(Guid id)
        {
            Result<LinkOrderProductDto> result = new Result<LinkOrderProductDto>();
            try
            {
                var model = _context.LinkOrderProducts.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<LinkOrderProductDto>(model);
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

        public Result<List<LinkOrderProductDto>> GetAllLinkOrderProduct()
        {
            Result<List<LinkOrderProductDto>> result = new Result<List<LinkOrderProductDto>>();
            try
            {
                var model = _context.LinkOrderProducts.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<LinkOrderProduct>, List<LinkOrderProductDto>>(model);
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
                var linkOrderProducts = GetAllLinkOrderProduct().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    linkOrderProducts = linkOrderProducts.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    linkOrderProducts = linkOrderProducts.Where(m => m.Product.Name.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = linkOrderProducts.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = linkOrderProducts.Count(), recordsTotal = linkOrderProducts.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }

        public Result<List<LinkOrderProductDto>> GetAllLinkOrderProductByOrderId(Guid id)
        {
            Result<List<LinkOrderProductDto>> result = new Result<List<LinkOrderProductDto>>();
            try
            {
                var model = _context.LinkOrderProducts.Include(x => x.Order)
                    .Include(x => x.Product).ThenInclude(x=> x.ProductUnit)
                    .Include(x => x.Product).ThenInclude(x=> x.CurrencyUnit)
                    .Where(x => x.OrderId == id && x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<LinkOrderProduct>, List<LinkOrderProductDto>>(model);
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

        public Result<List<LinkOrderProductDto>> GetAllLinkOrderProductByProductId(Guid id)
        {
            Result<List<LinkOrderProductDto>> result = new Result<List<LinkOrderProductDto>>();
            try
            {
                var model = _context.LinkOrderProducts.Where(x => x.ProductId == id && x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<LinkOrderProduct>, List<LinkOrderProductDto>>(model);
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
    }
}