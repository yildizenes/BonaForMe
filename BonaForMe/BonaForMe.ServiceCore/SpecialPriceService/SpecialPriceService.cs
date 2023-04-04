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

namespace BonaForMe.ServiceCore.SpecialPriceService
{
    public class SpecialPriceService : ISpecialPriceService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public SpecialPriceService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<SpecialPriceDto> AddSpecialPrice(SpecialPriceDto specialPriceDto)
        {
            Result<SpecialPriceDto> result = new Result<SpecialPriceDto>();
            try
            {
                SpecialPrice specialPrice = _mapper.Map<SpecialPrice>(specialPriceDto);
                if (specialPriceDto.Id != Guid.Empty)
                {
                    var oldModel = _context.SpecialPrices.FirstOrDefault(x => x.Id == specialPriceDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, specialPrice);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(specialPrice);
                    }
                    else
                    {
                        result.Message = "A special price has been assigned to the customer for the selected product. Delete the old record to continue.";
                        result.Success = false;
                        return result;
                    }
                }
                else
                {
                    if (!IsThereSpecialPrice(specialPriceDto))
                        _context.Add(specialPrice);
                    else
                    {
                        result.Message = "A special price has been assigned to the customer for the selected product. Delete the old record to continue.";
                        result.Success = false;
                        return result;
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<SpecialPriceDto>(specialPrice);
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

        public Result DeleteSpecialPrice(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.SpecialPrices.FirstOrDefault(d => d.Id == id);
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

        public Result<SpecialPriceDto> UpdateSpecialPrice(SpecialPriceDto specialPriceDto)
        {
            Result<SpecialPriceDto> result = new Result<SpecialPriceDto>();
            try
            {
                SpecialPrice specialPrice = _mapper.Map<SpecialPrice>(specialPriceDto);
                if (specialPriceDto.Id != Guid.Empty)
                {
                    var oldModel = _context.SpecialPrices.FirstOrDefault(x => x.Id == specialPriceDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, specialPrice);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(specialPrice);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<SpecialPriceDto>(specialPrice);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<SpecialPriceDto> GetSpecialPriceById(Guid id)
        {
            Result<SpecialPriceDto> result = new Result<SpecialPriceDto>();
            try
            {
                var model = _context.SpecialPrices.Include(x => x.Product).Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<SpecialPriceDto>(model);
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

        public Result<List<SpecialPriceDto>> GetSpecialPriceByUserId(Guid id)
        {
            Result<List<SpecialPriceDto>> result = new Result<List<SpecialPriceDto>>();
            try
            {
                var model = _context.SpecialPrices.Include(x => x.Product).Where(x => x.UserId == id && x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<SpecialPrice>, List<SpecialPriceDto>>(model);
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

        public Result<List<SpecialPriceDto>> GetSpecialPriceByFilters(Guid userId, Guid categoryId)
        {
            Result<List<SpecialPriceDto>> result = new Result<List<SpecialPriceDto>>();
            try
            {
                var model = _context.SpecialPrices.Include(x => x.Product)
                    .Where(x => x.UserId == userId && x.Product.CategoryId == categoryId && x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<SpecialPrice>, List<SpecialPriceDto>>(model);
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

        public Result<List<SpecialPriceDto>> GetAllSpecialPrice()
        {
            Result<List<SpecialPriceDto>> result = new Result<List<SpecialPriceDto>>();
            try
            {
                var model = _context.SpecialPrices.Include(x=> x.User).Include(x=> x.Product).Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<SpecialPrice>, List<SpecialPriceDto>>(model);
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
        public bool IsThereSpecialPrice(SpecialPriceDto specialPriceDto)
        {
            try
            {
                var model = _context.SpecialPrices
                    .Any(x => x.UserId == specialPriceDto.UserId 
                    && x.ProductId == specialPriceDto.ProductId 
                    && x.IsActive && !x.IsDeleted);
                return model;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public JsonResult FillDataTable(DataTableDto dataTable)
        {
            try
            {
                var specialPrices = GetAllSpecialPrice().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    specialPrices = specialPrices.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    specialPrices = specialPrices.Where(m => m.UserName.ToLower().Contains(dataTable.SearchValue.ToLower()) 
                    || m.ProductName.ToLower().Contains(dataTable.SearchValue.ToLower()) 
                    || m.Price.ToString().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = specialPrices.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = specialPrices.Count(), recordsTotal = specialPrices.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}