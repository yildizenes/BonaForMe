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

namespace BonaForMe.ServiceCore.OrderDetailService
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public OrderDetailService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<OrderDetailDto> AddOrderDetail(OrderDetailDto orderDetailDto)
        {
            Result<OrderDetailDto> result = new Result<OrderDetailDto>();
            try
            {
                OrderDetail orderDetail = _mapper.Map<OrderDetail>(orderDetailDto);
                if (orderDetailDto.Id != Guid.Empty)
                {
                    var oldModel = _context.OrderDetails.FirstOrDefault(x => x.Id == orderDetailDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, orderDetail);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(orderDetail);
                    }
                    else
                        _context.Add(orderDetail);
                }
                else
                    _context.Add(orderDetail);
                _context.SaveChanges();
                result.Data = _mapper.Map<OrderDetailDto>(orderDetail);
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

        public Result DeleteOrderDetail(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.OrderDetails.FirstOrDefault(d => d.Id == id);
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

        public Result<OrderDetailDto> UpdateOrderDetail(OrderDetailDto orderDetailDto)
        {
            Result<OrderDetailDto> result = new Result<OrderDetailDto>();
            try
            {
                OrderDetail orderDetail = _mapper.Map<OrderDetail>(orderDetailDto);
                if (orderDetailDto.Id != Guid.Empty)
                {
                    var oldModel = _context.OrderDetails.FirstOrDefault(x => x.Id == orderDetailDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, orderDetail);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(orderDetail);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<OrderDetailDto>(orderDetail);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<OrderDetailDto> GetOrderDetailById(Guid id)
        {
            Result<OrderDetailDto> result = new Result<OrderDetailDto>();
            try
            {
                var model = _context.OrderDetails.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<OrderDetailDto>(model);
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

        public Result<List<OrderDetailDto>> GetAllOrderDetail()
        {
            Result<List<OrderDetailDto>> result = new Result<List<OrderDetailDto>>();
            try
            {
                var model = _context.OrderDetails.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<OrderDetail>, List<OrderDetailDto>>(model);
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
                var orderDetails = GetAllOrderDetail().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    orderDetails = orderDetails.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    orderDetails = orderDetails.Where(m => m.PayType.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = orderDetails.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = orderDetails.Count(), recordsTotal = orderDetails.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}