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

namespace BonaForMe.ServiceCore.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public OrderService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<OrderDto> AddOrder(OrderDto orderDto)
        {
            Result<OrderDto> result = new Result<OrderDto>();
            try
            {
                Order order = _mapper.Map<Order>(orderDto);
                if (orderDto.Id != Guid.Empty)
                {
                    var oldModel = _context.Orders.FirstOrDefault(x => x.Id == orderDto.Id);
                    if (oldModel != null)
                    {
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(order);
                    }
                    else
                        _context.Add(order);
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<OrderDto>(order);
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

        public Result DeleteOrder(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.Orders.FirstOrDefault(d => d.Id == id);
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

        public Result<OrderDto> UpdateOrder(OrderDto orderDto)
        {
            Result<OrderDto> result = new Result<OrderDto>();
            try
            {
                Order order = _mapper.Map<Order>(orderDto);
                if (orderDto.Id != Guid.Empty)
                {
                    var oldModel = _context.Orders.FirstOrDefault(x => x.Id == orderDto.Id);
                    if (oldModel != null)
                    {
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(order);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<OrderDto>(order);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<OrderDto> GetOrderById(Guid id)
        {
            Result<OrderDto> result = new Result<OrderDto>();
            try
            {
                var model = _context.Orders.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<OrderDto>(model);
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

        public Result<List<OrderDto>> GetAllOrder()
        {
            Result<List<OrderDto>> result = new Result<List<OrderDto>>();
            try
            {
                var model = _context.Orders.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<Order>, List<OrderDto>>(model);
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
                var orders = GetAllOrder().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    orders = orders.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    orders = orders.Where(m => m.OrderCode.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = orders.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = orders.Count(), recordsTotal = orders.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}