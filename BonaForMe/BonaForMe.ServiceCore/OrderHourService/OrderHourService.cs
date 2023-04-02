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

namespace BonaForMe.ServiceCore.OrderHourService
{
    public class OrderHourService : IOrderHourService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public OrderHourService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<OrderHourDto> AddOrderHour(OrderHourDto orderHourDto)
        {
            Result<OrderHourDto> result = new Result<OrderHourDto>();
            try
            {
                OrderHour orderHour = _mapper.Map<OrderHour>(orderHourDto);
                if (orderHourDto.Id != Guid.Empty)
                {
                    var oldModel = _context.OrderHours.FirstOrDefault(x => x.Id == orderHourDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, orderHour);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(orderHour);
                    }
                    else
                        _context.Add(orderHour);
                }
                else
                    _context.Add(orderHour);
                _context.SaveChanges();
                result.Data = _mapper.Map<OrderHourDto>(orderHour);
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

        public Result DeleteOrderHour(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.OrderHours.FirstOrDefault(d => d.Id == id);
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

        public Result<OrderHourDto> UpdateOrderHour(OrderHourDto orderHourDto)
        {
            Result<OrderHourDto> result = new Result<OrderHourDto>();
            try
            {
                OrderHour orderHour = _mapper.Map<OrderHour>(orderHourDto);
                if (orderHourDto.Id != Guid.Empty)
                {
                    var oldModel = _context.OrderHours.FirstOrDefault(x => x.Id == orderHourDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, orderHour);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(orderHour);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<OrderHourDto>(orderHour);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<OrderHourDto> GetOrderHourById(Guid id)
        {
            Result<OrderHourDto> result = new Result<OrderHourDto>();
            try
            {
                var model = _context.OrderHours.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<OrderHourDto>(model);
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

        public Result<List<OrderHourDto>> GetAllOrderHour()
        {
            Result<List<OrderHourDto>> result = new Result<List<OrderHourDto>>();
            try
            {
                var model = _context.OrderHours.Where(x => x.IsActive && !x.IsDeleted).OrderByDescending(x=> x.DateCreated).ToList();
                result.Data = _mapper.Map<List<OrderHour>, List<OrderHourDto>>(model);
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

        public Result ChangeActive(Guid orderHourId, bool isActive)
        {
            Result result = new Result();
            try
            {
                var model = _context.OrderHours.FirstOrDefault(x => x.Id == orderHourId);
                if (model != null)
                {
                    model.IsActive = isActive;
                    _context.Entry(model).State = EntityState.Detached;
                    _context.Update(model);
                }
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

        public JsonResult FillDataTable(DataTableDto dataTable)
        {
            try
            {
                var orderHours = _context.OrderHours.Where(x => !x.IsDeleted).AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    orderHours = orderHours.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    orderHours = orderHours.Where(m => m.Description.ToLower().Contains(dataTable.SearchValue.ToLower()) 
                    || m.Text.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = orderHours.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = orderHours.Count(), recordsTotal = orderHours.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}