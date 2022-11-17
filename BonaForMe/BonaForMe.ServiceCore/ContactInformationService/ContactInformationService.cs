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

namespace BonaForMe.ServiceCore.ContactInformationService
{
    public class ContactInformationService : IContactInformationService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public ContactInformationService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<ContactInformationDto> AddContactInformation(ContactInformationDto contactInformationDto)
        {
            Result<ContactInformationDto> result = new Result<ContactInformationDto>();
            try
            {
                ContactInformation contactInformation = _mapper.Map<ContactInformation>(contactInformationDto);
                if (contactInformationDto.Id != Guid.Empty)
                {
                    var oldModel = _context.ContactInformations.FirstOrDefault(x => x.Id == contactInformationDto.Id);
                    if (oldModel != null)
                    {
                        
                        DBHelper.SetBaseValues(oldModel, contactInformation);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(contactInformation);
                    }
                    else
                        _context.Add(contactInformation);
                }
                else
                    _context.Add(contactInformation);
                _context.SaveChanges();
                result.Data = _mapper.Map<ContactInformationDto>(contactInformation);
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

        public Result DeleteContactInformation(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.ContactInformations.FirstOrDefault(d => d.Id == id);
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

        public Result<ContactInformationDto> UpdateContactInformation(ContactInformationDto contactInformationDto)
        {
            Result<ContactInformationDto> result = new Result<ContactInformationDto>();
            try
            {
                ContactInformation contactInformation = _mapper.Map<ContactInformation>(contactInformationDto);
                if (contactInformationDto.Id != Guid.Empty)
                {
                    var oldModel = _context.ContactInformations.FirstOrDefault(x => x.Id == contactInformationDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, contactInformation);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(contactInformation);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<ContactInformationDto>(contactInformation);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<ContactInformationDto> GetContactInformationById(Guid id)
        {
            Result<ContactInformationDto> result = new Result<ContactInformationDto>();
            try
            {
                var model = _context.ContactInformations.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<ContactInformationDto>(model);
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

        public Result<List<ContactInformationDto>> GetAllContactInformation()
        {
            Result<List<ContactInformationDto>> result = new Result<List<ContactInformationDto>>();
            try
            {
                var model = _context.ContactInformations.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<ContactInformation>, List<ContactInformationDto>>(model);
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
                var contactInformations = GetAllContactInformation().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    contactInformations = contactInformations.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    contactInformations = contactInformations.Where(m => m.EmailInfo.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = contactInformations.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = contactInformations.Count(), recordsTotal = contactInformations.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}