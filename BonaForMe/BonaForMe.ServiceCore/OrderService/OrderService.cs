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
using BonaForMe.ServiceCore.LinkOrderProductService;

namespace BonaForMe.ServiceCore.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;
        private readonly ILinkOrderProductService _linkOrderProductService;

        public OrderService(BonaForMeDBContext context, IMapper mapper, ILinkOrderProductService linkOrderProductService)
        {
            _context = context;
            _mapper = mapper;
            _linkOrderProductService = linkOrderProductService;
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
                        DBHelper.SetBaseValues(oldModel, order);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(order);
                    }
                    else
                    {
                        order.OrderCode = GenerateOrderCode();
                        _context.Add(order);
                        List<LinkOrderProductDto> productList = orderDto.ProductList.Select(x => { x.OrderId = order.Id; return x; }).ToList();
                        _context.AddRange(_mapper.Map<List<LinkOrderProductDto>, List<LinkOrderProduct>>(productList));
                    }
                }
                else
                {
                    order.OrderCode = GenerateOrderCode();
                    _context.Add(order);
                    List<LinkOrderProductDto> productList = orderDto.ProductList?.Select(x => { x.OrderId = order.Id; return x; }).ToList();
                    _context.AddRange(_mapper.Map<List<LinkOrderProductDto>, List<LinkOrderProduct>>(productList));
                }

                _context.SaveChanges();
                //result.Data = _mapper.Map<OrderDto>(order);
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
                        DBHelper.SetBaseValues(oldModel, order);
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
                var model = _context.Orders.Where(x => x.Id == id && x.IsActive && !x.IsDeleted)
                    .Include(x => x.User).Include(x => x.OrderStatus)
                    .FirstOrDefault();

                result.Data = _mapper.Map<OrderDto>(model);
                if (result.Data != null)
                {
                    result.Data.ProductList = _linkOrderProductService.GetAllLinkOrderProductByOrderId(id).Data;
                }
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
                var model = _context.Orders.Where(x => x.IsActive && !x.IsDeleted).Include(x => x.User).Include(x => x.OrderStatus).OrderByDescending(x => x.DateCreated).ToList();
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

        public Result<List<OrderDto>> GetUserNowOrderDetail()
        {
            Result<List<OrderDto>> result = new Result<List<OrderDto>>();
            try
            {
                var nowOrderStatusList = new List<int> { 1, 2, 3 };
                var model = _context.Orders.Where(x => nowOrderStatusList.Contains(x.OrderStatusId) && x.IsActive && !x.IsDeleted)
                    .Include(x => x.User).Include(x => x.OrderStatus)
                    .OrderByDescending(x => x.DateCreated)
                    .ToList();

                var orderDtos = _mapper.Map<List<Order>, List<OrderDto>>(model);
                foreach (var item in orderDtos)
                {
                    var linkOrderProducts = _context.LinkOrderProducts.Where(x => x.OrderId == item.Id).Include(x => x.Product).ToList();
                    item.ProductList = _mapper.Map<List<LinkOrderProduct>, List<LinkOrderProductDto>>(linkOrderProducts);
                }

                result.Data = orderDtos;
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

        public Result<List<OrderDto>> GetUserLastOrderDetail()
        {
            Result<List<OrderDto>> result = new Result<List<OrderDto>>();
            try
            {
                var model = _context.Orders.Where(x => x.OrderStatusId > 3 && x.IsActive && !x.IsDeleted)
                    .Include(x => x.User).Include(x => x.OrderStatus)
                    .OrderByDescending(x => x.DateCreated)
                    .ToList();

                var orderDtos = _mapper.Map<List<Order>, List<OrderDto>>(model);
                foreach (var item in orderDtos)
                {
                    var linkOrderProducts = _context.LinkOrderProducts.Where(x => x.OrderId == item.Id).Include(x => x.Product).ToList();
                    item.ProductList = _mapper.Map<List<LinkOrderProduct>, List<LinkOrderProductDto>>(linkOrderProducts);
                }

                result.Data = orderDtos;
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

        public Result<List<OrderDto>> GetUserOrderDetail(Guid userId)
        {
            Result<List<OrderDto>> result = new Result<List<OrderDto>>();
            try
            {
                var model = _context.Orders.Where(x => x.UserId == userId && x.IsActive && !x.IsDeleted)
                    .Include(x => x.User).Include(x => x.OrderStatus)
                    .ToList();
                var orderDtos = _mapper.Map<List<Order>, List<OrderDto>>(model);
                foreach (var item in orderDtos)
                {
                    var linkOrderProducts = _context.LinkOrderProducts.Where(x => x.OrderId == item.Id).Include(x => x.Product).ToList();
                    item.ProductList = _mapper.Map<List<LinkOrderProduct>, List<LinkOrderProductDto>>(linkOrderProducts);
                }
                result.Data = orderDtos;
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

        public Result<OrderDto> UpdateOrderStatus(UpdateOrderDto updateOrderDto)
        {
            Result<OrderDto> result = new Result<OrderDto>();
            try
            {
                var model = _context.Orders.Where(x => x.Id == updateOrderDto.OrderId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (model == null)
                {
                    result.Success = false;
                    result.Message = "Order not found.";
                    return result;
                }

                _context.Entry(model).State = EntityState.Modified;
                model.OrderStatusId = updateOrderDto.OrderStatusId;
                _context.Update(model);
                _context.SaveChanges();

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

        public JsonResult FillDataTable(DataTableDto dataTable, int orderStatusId)
        {
            try
            {
                var orders = GetAllOrder().Data.Where(x => x.OrderStatusId == orderStatusId).AsQueryable();
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

        public string GenerateOrderCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 8)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}