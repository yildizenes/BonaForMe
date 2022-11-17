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

namespace BonaForMe.ServiceCore.CurrencyUnitService
{
    public class CurrencyUnitService : ICurrencyUnitService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public CurrencyUnitService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<CurrencyUnitDto> AddCurrencyUnit(CurrencyUnitDto currencyUnitDto)
        {
            Result<CurrencyUnitDto> result = new Result<CurrencyUnitDto>();
            try
            {
                CurrencyUnit currencyUnit = _mapper.Map<CurrencyUnit>(currencyUnitDto);
                if (currencyUnitDto.Id != Guid.Empty)
                {
                    var oldModel = _context.CurrencyUnits.FirstOrDefault(x => x.Id == currencyUnitDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, currencyUnit);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(currencyUnit);
                    }
                    else
                        _context.Add(currencyUnit);
                }
                else
                    _context.Add(currencyUnit);
                _context.SaveChanges();
                result.Data = _mapper.Map<CurrencyUnitDto>(currencyUnit);
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

        public Result DeleteCurrencyUnit(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.CurrencyUnits.FirstOrDefault(d => d.Id == id);
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

        public Result<CurrencyUnitDto> UpdateCurrencyUnit(CurrencyUnitDto currencyUnitDto)
        {
            Result<CurrencyUnitDto> result = new Result<CurrencyUnitDto>();
            try
            {
                CurrencyUnit currencyUnit = _mapper.Map<CurrencyUnit>(currencyUnitDto);
                if (currencyUnitDto.Id != Guid.Empty)
                {
                    var oldModel = _context.CurrencyUnits.FirstOrDefault(x => x.Id == currencyUnitDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, currencyUnit);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(currencyUnit);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<CurrencyUnitDto>(currencyUnit);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<CurrencyUnitDto> GetCurrencyUnitById(Guid id)
        {
            Result<CurrencyUnitDto> result = new Result<CurrencyUnitDto>();
            try
            {
                var model = _context.CurrencyUnits.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<CurrencyUnitDto>(model);
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

        public Result<List<CurrencyUnitDto>> GetAllCurrencyUnit()
        {
            Result<List<CurrencyUnitDto>> result = new Result<List<CurrencyUnitDto>>();
            try
            {
                var model = _context.CurrencyUnits.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<CurrencyUnit>, List<CurrencyUnitDto>>(model);
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
                var currencyUnits = GetAllCurrencyUnit().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    currencyUnits = currencyUnits.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    currencyUnits = currencyUnits.Where(m => m.Name.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = currencyUnits.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = currencyUnits.Count(), recordsTotal = currencyUnits.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}