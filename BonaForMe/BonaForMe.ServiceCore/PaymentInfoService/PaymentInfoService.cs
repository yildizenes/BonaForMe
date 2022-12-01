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

namespace BonaForMe.ServiceCore.PaymentInfoService
{
    public class PaymentInfoService : IPaymentInfoService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public PaymentInfoService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<PaymentInfoDto> AddPaymentInfo(PaymentInfoDto paymentInfoDto)
        {
            Result<PaymentInfoDto> result = new Result<PaymentInfoDto>();
            try
            {
                PaymentInfo paymentInfo = _mapper.Map<PaymentInfo>(paymentInfoDto);
                if (paymentInfoDto.Id != Guid.Empty)
                {
                    var oldModel = _context.PaymentInfos.FirstOrDefault(x => x.Id == paymentInfoDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, paymentInfo);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(paymentInfo);
                    }
                    else
                        _context.Add(paymentInfo);
                }
                else
                    _context.Add(paymentInfo);
                _context.SaveChanges();
                result.Data = _mapper.Map<PaymentInfoDto>(paymentInfo);
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

        public Result DeletePaymentInfo(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.PaymentInfos.FirstOrDefault(d => d.Id == id);
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

        public Result<PaymentInfoDto> UpdatePaymentInfo(PaymentInfoDto paymentInfoDto)
        {
            Result<PaymentInfoDto> result = new Result<PaymentInfoDto>();
            try
            {
                PaymentInfo paymentInfo = _mapper.Map<PaymentInfo>(paymentInfoDto);
                if (paymentInfoDto.Id != Guid.Empty)
                {
                    var oldModel = _context.PaymentInfos.FirstOrDefault(x => x.Id == paymentInfoDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, paymentInfo);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(paymentInfo);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<PaymentInfoDto>(paymentInfo);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<PaymentInfoDto> GetPaymentInfoById(Guid id)
        {
            Result<PaymentInfoDto> result = new Result<PaymentInfoDto>();
            try
            {
                var model = _context.PaymentInfos.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<PaymentInfoDto>(model);
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

        public Result<List<PaymentInfoDto>> GetAllPaymentInfo()
        {
            Result<List<PaymentInfoDto>> result = new Result<List<PaymentInfoDto>>();
            try
            {
                var model = _context.PaymentInfos.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<PaymentInfo>, List<PaymentInfoDto>>(model);
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
                var paymentInfos = GetAllPaymentInfo().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    paymentInfos = paymentInfos.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    paymentInfos = paymentInfos.Where(m => m.Description.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = paymentInfos.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = paymentInfos.Count(), recordsTotal = paymentInfos.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}