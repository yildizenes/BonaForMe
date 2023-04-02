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

namespace BonaForMe.ServiceCore.OrderStatusService
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public OrderStatusService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<OrderStatusDto> AddOrderStatus(OrderStatusDto orderStatusDto)
        {
            Result<OrderStatusDto> result = new Result<OrderStatusDto>();
            try
            {
                OrderStatus orderStatus = _mapper.Map<OrderStatus>(orderStatusDto);
                if (orderStatusDto.Id != 0)
                {
                    var oldModel = _context.OrderStatuses.FirstOrDefault(x => x.Id == orderStatusDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValuesInt(oldModel, orderStatus);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(orderStatus);
                    }
                    else
                        _context.Add(orderStatus);
                }
                else
                    _context.Add(orderStatus);
                _context.SaveChanges();
                result.Data = _mapper.Map<OrderStatusDto>(orderStatus);
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

        public Result DeleteOrderStatus(int id)
        {
            Result result = new Result();
            try
            {
                var model = _context.OrderStatuses.FirstOrDefault(d => d.Id == id);
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

        public Result<OrderStatusDto> UpdateOrderStatus(OrderStatusDto orderStatusDto)
        {
            Result<OrderStatusDto> result = new Result<OrderStatusDto>();
            try
            {
                OrderStatus orderStatus = _mapper.Map<OrderStatus>(orderStatusDto);
                if (orderStatusDto.Id != 0)
                {
                    var oldModel = _context.OrderStatuses.FirstOrDefault(x => x.Id == orderStatusDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValuesInt(oldModel, orderStatus);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(orderStatus);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<OrderStatusDto>(orderStatus);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<OrderStatusDto> GetOrderStatusById(int id)
        {
            Result<OrderStatusDto> result = new Result<OrderStatusDto>();
            try
            {
                var model = _context.OrderStatuses.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<OrderStatusDto>(model);
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

        public Result<List<OrderStatusDto>> GetAllOrderStatus()
        {
            Result<List<OrderStatusDto>> result = new Result<List<OrderStatusDto>>();
            try
            {
                var model = _context.OrderStatuses.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<OrderStatus>, List<OrderStatusDto>>(model);
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
                var orderStatuss = GetAllOrderStatus().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    orderStatuss = orderStatuss.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    orderStatuss = orderStatuss.Where(m => m.Name.ToLower().Contains(dataTable.SearchValue.ToLower()) 
                    || m.ColorCode.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = orderStatuss.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = orderStatuss.Count(), recordsTotal = orderStatuss.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}