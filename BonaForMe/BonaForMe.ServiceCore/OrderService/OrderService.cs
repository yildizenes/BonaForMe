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
using System.IO;
using BonaForMe.DomainCore.DTO.PDFModels;
using BonaForMe.ServiceCore.PDFServices;
using BonaForMe.ServiceCore.PaymentInfoService;
using BonaForMe.ServiceCore.ProductService;
using BonaForMe.ServiceCore.CampaignProductService;
using BonaForMe.ServiceCore.MailSenderService;

namespace BonaForMe.ServiceCore.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;
        private readonly ILinkOrderProductService _linkOrderProductService;
        private readonly IPaymentInfoService _paymentInfoService;
        private readonly IProductService _productService;
        private readonly ICampaignProductService _campaignProductService;
        private readonly IMailSenderService _mailSenderService;

        public OrderService(BonaForMeDBContext context, IMapper mapper,
            ILinkOrderProductService linkOrderProductService, IPaymentInfoService paymentInfoService,
            IProductService productService, ICampaignProductService campaignProductService, IMailSenderService mailSenderService)
        {
            _context = context;
            _mapper = mapper;
            _linkOrderProductService = linkOrderProductService;
            _paymentInfoService = paymentInfoService;
            _productService = productService;
            _campaignProductService = campaignProductService;
            _mailSenderService = mailSenderService;
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

                        if (orderDto.OrderStatusId == 7) // 7 - Order complete status
                        {
                            var orderCompleteResult = CompleteTheOrder(orderDto.Id);
                            if (!orderCompleteResult.Success)
                            {
                                result.Success = true;
                                result.Message = "Order complete unsuccessful.";
                                return result;
                            }
                        }
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
                CreateInvoice(order.Id);
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
                CreateInvoice(order.Id);
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
                    .Include(x => x.User).Include(x => x.OrderStatus).Include(x => x.OrderHour)
                    .FirstOrDefault();

                result.Data = _mapper.Map<OrderDto>(model);
                result.Data.OrderDetail = _context.OrderDetails.Include(x => x.Courier).FirstOrDefault(x => x.OrderId == id);
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
                var model = _context.Orders.Where(x => x.IsActive && !x.IsDeleted)
                    .Include(x => x.User).Include(x => x.OrderStatus).Include(x => x.OrderHour)
                    .OrderByDescending(x => x.DateCreated).ToList();
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
        public Result<List<OrderDto>> GetAllOrderByStatusId(List<int> orderStatuses, Guid userId)
        {
            Result<List<OrderDto>> result = new Result<List<OrderDto>>();
            try
            {
                var model = new List<Order>();
                if (userId != Guid.Empty)
                {
                    model = _context.Orders.Where(x => userId == x.UserId && orderStatuses.Contains(x.OrderStatusId) && x.IsActive && !x.IsDeleted)
                        .Include(x => x.User).Include(x => x.OrderStatus).Include(x => x.OrderHour)
                        .OrderByDescending(x => x.DateCreated)
                        .ToList();
                }
                else
                {
                    model = _context.Orders.Where(x => orderStatuses.Contains(x.OrderStatusId) && x.IsActive && !x.IsDeleted)
                        .Include(x => x.User).Include(x => x.OrderStatus).Include(x => x.OrderHour)
                        .OrderByDescending(x => x.DateCreated)
                        .ToList();
                }

                var orderDtos = _mapper.Map<List<Order>, List<OrderDto>>(model);
                foreach (var item in orderDtos)
                    item.ProductList = _linkOrderProductService.GetAllLinkOrderProductByOrderId(item.Id).Data;

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

        //public Result<List<OrderDto>> GetUserNowOrderDetail()
        //{
        //    Result<List<OrderDto>> result = new Result<List<OrderDto>>();
        //    try
        //    {
        //        var nowOrderStatusList = new List<int> { 1, 2, 3 };
        //        var model = _context.Orders.Where(x => nowOrderStatusList.Contains(x.OrderStatusId) && x.IsActive && !x.IsDeleted)
        //            .Include(x => x.User).Include(x => x.OrderStatus)
        //            .OrderByDescending(x => x.DateCreated)
        //            .ToList();

        //        var orderDtos = _mapper.Map<List<Order>, List<OrderDto>>(model);
        //        foreach (var item in orderDtos)
        //        {
        //            var linkOrderProducts = _context.LinkOrderProducts.Where(x => x.OrderId == item.Id).Include(x => x.Product).ToList();
        //            item.ProductList = _mapper.Map<List<LinkOrderProduct>, List<LinkOrderProductDto>>(linkOrderProducts);
        //        }

        //        result.Data = orderDtos;
        //        result.Success = true;
        //        result.Message = ResultMessages.Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.Message;
        //        result.Success = false;
        //    }
        //    return result;
        //}

        //public Result<List<OrderDto>> GetUserLastOrderDetail()
        //{
        //    Result<List<OrderDto>> result = new Result<List<OrderDto>>();
        //    try
        //    {
        //        var model = _context.Orders.Where(x => x.OrderStatusId > 3 && x.IsActive && !x.IsDeleted)
        //            .Include(x => x.User).Include(x => x.OrderStatus)
        //            .OrderByDescending(x => x.DateCreated)
        //            .ToList();

        //        var orderDtos = _mapper.Map<List<Order>, List<OrderDto>>(model);
        //        foreach (var item in orderDtos)
        //        {
        //            var linkOrderProducts = _context.LinkOrderProducts.Where(x => x.OrderId == item.Id).Include(x => x.Product).ToList();
        //            item.ProductList = _mapper.Map<List<LinkOrderProduct>, List<LinkOrderProductDto>>(linkOrderProducts);
        //        }

        //        result.Data = orderDtos;
        //        result.Success = true;
        //        result.Message = ResultMessages.Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.Message;
        //        result.Success = false;
        //    }
        //    return result;
        //}

        public Result<List<OrderDto>> GetUserOrderDetail(Guid userId)
        {
            Result<List<OrderDto>> result = new Result<List<OrderDto>>();
            try
            {
                var model = _context.Orders.Where(x => x.UserId == userId && x.IsActive && !x.IsDeleted)
                    .Include(x => x.User).Include(x => x.OrderStatus).Include(x => x.OrderHour)
                    .ToList();
                var orderDtos = _mapper.Map<List<Order>, List<OrderDto>>(model);
                foreach (var item in orderDtos)
                {
                    item.ProductList = _linkOrderProductService.GetAllLinkOrderProductByOrderId(item.Id).Data;
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

                if (updateOrderDto.OrderStatusId == 7) // 7 - Order complete status
                {
                    var orderCompleteResult = CompleteTheOrder(updateOrderDto.OrderId);
                    if (!orderCompleteResult.Success)
                    {
                        result.Success = false;
                        result.Message = "Order complete unsuccessful.";
                        return result;
                    }
                    var createInvoiceResult = CreateInvoice(updateOrderDto.OrderId);
                    if (!createInvoiceResult.Success)
                    {
                        result.Success = false;
                        result.Message = createInvoiceResult.Message;
                        return result;
                    }
                    var order = GetOrderById(updateOrderDto.OrderId).Data;
                    _mailSenderService.SendMail(order.User.UserMail, MailTypes.InvoiceDelivery, order.OrderCode);
                }

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

        public Result CompleteTheOrder(Guid orderId)
        {
            Result result = new Result();
            try
            {
                var orderProductResult = _linkOrderProductService.GetAllLinkOrderProductByOrderId(orderId);
                var orderProducts = new List<LinkOrderProductDto>();

                if (orderProductResult.Success)
                {
                    orderProducts = orderProductResult.Data;
                    foreach (var item in orderProducts) // Add OrderLog
                    {
                        var order = new OrderLog
                        {
                            OrderId = orderId,
                            ProductId = item.ProductId,
                            Price = item.Price,
                            Count = item.Count,
                        };
                        _context.Add(order);
                        _productService.UpdateProductStock(item.ProductId, 0, item.Count);
                        if (item.IsCampaignProduct)
                            _campaignProductService.UpdateProductStock(item.ProductId, item.Count);
                    }

                    _context.SaveChanges();
                    result.Success = true;
                    return result;
                }
                result.Success = false;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public JsonResult FillDataTable(DataTableDto dataTable, byte type, int orderStatusId)
        {
            try
            {
                var orders = GetAllOrder().Data.AsQueryable();
                if (type == 1)
                    orders = orders.Where(x => x.OrderStatusId == orderStatusId).AsQueryable();
                if (type == 2)
                {
                    var todaysDate = DateTime.Today;
                    orders = orders.Where(x =>
                        x.DateCreated.Value.Day == todaysDate.Day &&
                        x.DateCreated.Value.Month == todaysDate.Month &&
                        x.DateCreated.Value.Year == todaysDate.Year
                    ).AsQueryable();
                }

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

        public JsonResult FillInvoiceDataTable(DataTableDto dataTable, ReportDateDto reportDateDto)
        {
            try
            {
                var result = _context.Orders.Where(x => x.OrderStatusId == 7 &&
                    x.DateCreated >= (DateTime)reportDateDto.StartDate &&
                    x.DateCreated <= (DateTime)reportDateDto.EndDate &&
                    x.IsActive && !x.IsDeleted).Include(x => x.User).ToList();

                var orders = _mapper.Map<List<Order>, List<OrderDto>>(result).AsQueryable();
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

        public Result CreateInvoice(Guid orderId)
        {
            Result result = new Result();
            var path = Path.Combine(Directory.GetCurrentDirectory()).Replace("api.bonameforme.com", "httpdocs") + @"\";
            var order = GetOrderById(orderId);
            var itemList = new List<ItemRow>();
            decimal subTotal = 0, totalVAT = 0;
            var paymentInfos = _paymentInfoService.GetAllPaymentInfo().Data.OrderBy(x => x.DateCreated).Select(x => x.Description).ToArray();

            var billUser = order.Data.User;
            foreach (var item in order.Data.ProductList)
            {
                var product = item.Product;
                itemList.Add(ItemRow.Make(item.ProductId.ToString().Split('-').Last().ToUpper(), product.Name, item.Price, item.Count, item.Price, item.Price * item.Count, product.TaxRate)); ;
                subTotal += item.Price * item.Count;
                if (product.TaxRate != 0)
                    totalVAT += (item.Price * item.Count * product.TaxRate) / 100;
            }
            try
            {
                new InvoicerApi(SizeOption.A4, OrientationOption.Portrait, "€", order.Data.OrderCode)
                    .TextColor("#CC0000")
                    .Image(path + @"wwwroot\images\bonameformelogo.jpg", 140, 100)
                    .Company(Address.Make("FROM", new string[] {
                        "Solmaz Packaging",
                        "Unit 9-10, The New Sunbeam Ind",
                        "Est. Blackpool. Cork.",
                        "Phone: 087 353 33 35",
                        "Email: info@bonameforme.com",
                        "Website: bonameforme.com",
                        "VAT No: 3933414LH"
                    }))
                    .Client(Address.Make("BILL TO", new string[]
                    {
                        billUser.FullName.ToUpper(),
                        "Company : " + billUser.CompanyName,
                        "Address : " + billUser.Address.ToUpper(),
                        "Phone   : " + billUser.UserPhone,
                        "Email   : " + billUser.UserMail
                    }))
                    .Items(itemList)
                    .Totals(new List<TotalRow> {
                    TotalRow.Make("Sub Total", subTotal),
                    TotalRow.Make("Total VAT", totalVAT),
                    TotalRow.Make("Total", subTotal + totalVAT, true),
                    })
                    //.Details(new List<DetailRow> {
                    //DetailRow.Make("PAYMENT INFORMATION", new string[] {
                    //"Customer Signature : __________________________________",
                    //"Customer Name      : __________________________________",
                    //"Amount Paid        : __________________________________",
                    //"Payment Type       : □ Cash   □ Cheque   □ Credit Card   □ Not Paid ",
                    //"Driver  Name       : __________________________________",
                    //}),
                    //DetailRow.Make("BANK INFORMATION", paymentInfos)
                    //})
                    .Footer("")
                    .Save();

                result.Success = true;
                result.Message = "Process successfully.";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
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