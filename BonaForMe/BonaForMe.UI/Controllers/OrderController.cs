using BonaForMe.DomainCore.DTO;
using BonaForMe.DomainCore.DTO.PDFModels;
using BonaForMe.ServiceCore.OrderService;
using BonaForMe.ServiceCore.PaymentInfoService;
using BonaForMe.ServiceCore.PDFServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BonaForMe.UI.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentInfoService _paymentInfoService;
        public OrderController(IOrderService orderService, IPaymentInfoService paymentInfoService)
        {
            _orderService = orderService;
            _paymentInfoService = paymentInfoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Save(OrderDto orderDto)
        {
            try
            {
                var result = _orderService.AddOrder(orderDto);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Update(OrderDto orderDto)
        {
            try
            {
                var result = _orderService.UpdateOrder(orderDto);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            try
            {
                var result = _orderService.DeleteOrder(id);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult GetOrderById(Guid id)
        {
            try
            {
                var result = _orderService.GetOrderById(id);
                if (result != null)
                {
                    return new JsonResult(result.Data);
                }
                return Json(new { success = false });
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetAllOrder()
        {
            try
            {
                var result = _orderService.GetAllOrder();
                return new JsonResult(result.Data);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult LoadOrderData(int orderStatusId)
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                //Paging Size (10,20,50,100)
                int pageSize = Convert.ToInt32(length) != -1 ? Convert.ToInt32(length) : 100;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                DataTableDto dataTable = new DataTableDto()
                {
                    Draw = draw,
                    PageSize = pageSize,
                    Skip = skip,
                    SearchValue = searchValue,
                    SortColumnDirection = sortColumnDirection,
                    SortColumn = sortColumn
                };
                return _orderService.FillDataTable(dataTable, orderStatusId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult CreateInvoice(Guid orderId)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory()) + @"\";
            var order = _orderService.GetOrderById(orderId);
            var itemList = new List<ItemRow>();
            decimal subTotal = 0, totalVAT = 0;
            var paymentInfos = _paymentInfoService.GetAllPaymentInfo().Data.OrderBy(x=> x.DateCreated).Select(x => x.Description).ToArray();

            var billUser = order.Data.User;

            foreach (var item in order.Data.ProductList)
            {
                var product = item.Product;
                itemList.Add(ItemRow.Make(item.ProductId.ToString().Split('-').Last().ToUpper(), product.Name, product.Price, item.Count, product.Price, product.Price * item.Count, product.TaxRate)); ;
                subTotal += product.Price * item.Count;
                if (product.TaxRate != 0)
                    totalVAT += (product.Price * item.Count * product.TaxRate) / 100;
            }
            try
            {
                new InvoicerApi(SizeOption.A4, OrientationOption.Portrait, "€", order.Data.OrderCode)
                    .TextColor("#CC0000")
                    .Image(path + @"wwwroot\images\bonaformelogo.jpg", 100, 100)
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
                    .Details(new List<DetailRow> {
                    DetailRow.Make("PAYMENT INFORMATION", new string[] {
                    "Customer Signature : __________________________________",
                    "Customer Name      : __________________________________",
                    "Amount Paid        : __________________________________",
                    "Payment Type       : □ Cash   □ Cheque   □ Credit Card   □ Not Paid ",
                    "Driver  Name       : __________________________________",
                    }),
                    DetailRow.Make("BANK INFORMATION", paymentInfos)
                    })
                    .Footer("")
                    .Save();

                return new JsonResult("okey");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}