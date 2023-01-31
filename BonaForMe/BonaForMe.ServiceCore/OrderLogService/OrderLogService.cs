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

namespace BonaForMe.ServiceCore.OrderLogService
{
    public class OrderLogService : IOrderLogService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public OrderLogService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<OrderLogDto> AddOrderLog(OrderLogDto orderLogDto)
        {
            Result<OrderLogDto> result = new Result<OrderLogDto>();
            try
            {
                OrderLog orderLog = _mapper.Map<OrderLog>(orderLogDto);
                if (orderLogDto.Id != Guid.Empty)
                {
                    var oldModel = _context.OrderLogs.FirstOrDefault(x => x.Id == orderLogDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, orderLog);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(orderLog);
                    }
                    else
                        _context.Add(orderLog);
                }
                else
                    _context.Add(orderLog);
                _context.SaveChanges();
                result.Data = _mapper.Map<OrderLogDto>(orderLog);
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

        public Result DeleteOrderLog(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.OrderLogs.FirstOrDefault(d => d.Id == id);
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

        public Result<OrderLogDto> UpdateOrderLog(OrderLogDto orderLogDto)
        {
            Result<OrderLogDto> result = new Result<OrderLogDto>();
            try
            {
                OrderLog orderLog = _mapper.Map<OrderLog>(orderLogDto);
                if (orderLogDto.Id != Guid.Empty)
                {
                    var oldModel = _context.OrderLogs.FirstOrDefault(x => x.Id == orderLogDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, orderLog);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(orderLog);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<OrderLogDto>(orderLog);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<OrderLogDto> GetOrderLogById(Guid id)
        {
            Result<OrderLogDto> result = new Result<OrderLogDto>();
            try
            {
                var model = _context.OrderLogs.Include(x => x.Product).Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<OrderLogDto>(model);
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

        public Result<List<OrderLogDto>> GetAllOrderLog()
        {
            Result<List<OrderLogDto>> result = new Result<List<OrderLogDto>>();
            try
            {
                var model = _context.OrderLogs.Include(x=> x.Product).Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<OrderLog>, List<OrderLogDto>>(model);
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
                var orderLogs = GetAllOrderLog().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    orderLogs = orderLogs.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    //orderLogs = orderLogs.Where(m => m.Name.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = orderLogs.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = orderLogs.Count(), recordsTotal = orderLogs.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}