using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.PaymentInfoService
{
    public interface IPaymentInfoService
    {
        Result<PaymentInfoDto> AddPaymentInfo(PaymentInfoDto paymentInfoDto);
        Result DeletePaymentInfo(Guid id);
        Result<PaymentInfoDto> UpdatePaymentInfo(PaymentInfoDto paymentInfoDto);
        Result<PaymentInfoDto> GetPaymentInfoById(Guid id);
        Result<List<PaymentInfoDto>> GetAllPaymentInfo();
        JsonResult FillDataTable(DataTableDto dataTable);

    }
}
