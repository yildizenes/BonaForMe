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

namespace BonaForMe.ServiceCore.CourierCoordinateService
{
    public class CourierCoordinateService : ICourierCoordinateService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public CourierCoordinateService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<CourierCoordinateDto> AddCourierCoordinate(CourierCoordinateDto courierCoordinateDto)
        {
            Result<CourierCoordinateDto> result = new Result<CourierCoordinateDto>();
            bool activeStatus = courierCoordinateDto.IsActive;
            try
            {
                CourierCoordinate courierCoordinate = _mapper.Map<CourierCoordinate>(courierCoordinateDto);
                if (courierCoordinateDto.UserId != Guid.Empty)
                {
                    var oldModel = _context.CourierCoordinates.FirstOrDefault(x => x.UserId == courierCoordinateDto.UserId);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, courierCoordinate);
                        oldModel.IsActive = activeStatus;
                        oldModel.XCoordinate = courierCoordinate.XCoordinate;
                        oldModel.YCoordinate = courierCoordinate.YCoordinate;
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(oldModel);
                    }
                    else
                        _context.Add(courierCoordinate);
                }
                else
                    _context.Add(courierCoordinate);
                _context.SaveChanges();
                courierCoordinate.IsActive = activeStatus;
                result.Data = _mapper.Map<CourierCoordinateDto>(courierCoordinate);
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

        public Result DeleteCourierCoordinate(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.CourierCoordinates.FirstOrDefault(d => d.Id == id);
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

        public Result<CourierCoordinateDto> UpdateCourierCoordinate(CourierCoordinateDto courierCoordinateDto)
        {
            Result<CourierCoordinateDto> result = new Result<CourierCoordinateDto>();
            try
            {
                CourierCoordinate courierCoordinate = _mapper.Map<CourierCoordinate>(courierCoordinateDto);
                if (courierCoordinateDto.Id != Guid.Empty)
                {
                    var oldModel = _context.CourierCoordinates.FirstOrDefault(x => x.Id == courierCoordinateDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, courierCoordinate);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(courierCoordinate);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<CourierCoordinateDto>(courierCoordinate);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<CourierCoordinateDto> GetCourierCoordinateById(Guid id)
        {
            Result<CourierCoordinateDto> result = new Result<CourierCoordinateDto>();
            try
            {
                var model = _context.CourierCoordinates.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<CourierCoordinateDto>(model);
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

        public Result<CourierCoordinateDto> GetCourierCoordinateByCourierId(Guid id)
        {
            Result<CourierCoordinateDto> result = new Result<CourierCoordinateDto>();
            try
            {
                var model = _context.CourierCoordinates.Where(x => x.UserId == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<CourierCoordinateDto>(model);
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

        public Result<List<CourierCoordinateDto>> GetAllCourierCoordinate()
        {
            Result<List<CourierCoordinateDto>> result = new Result<List<CourierCoordinateDto>>();
            try
            {
                var model = _context.CourierCoordinates.Where(x => x.IsActive && !x.IsDeleted).Include(x=> x.User).ToList();
                result.Data = _mapper.Map<List<CourierCoordinate>, List<CourierCoordinateDto>>(model);
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
                var courierCoordinates = GetAllCourierCoordinate().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    courierCoordinates = courierCoordinates.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    //courierCoordinates = courierCoordinates.Where(m => m.Name.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = courierCoordinates.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = courierCoordinates.Count(), recordsTotal = courierCoordinates.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}