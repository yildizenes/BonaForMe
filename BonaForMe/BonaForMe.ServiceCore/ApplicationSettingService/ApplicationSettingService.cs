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

namespace BonaForMe.ServiceCore.ApplicationSettingService
{
    public class ApplicationSettingService : IApplicationSettingService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public ApplicationSettingService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<ApplicationSettingDto> AddApplicationSetting(ApplicationSettingDto applicationSettingDto)
        {
            Result<ApplicationSettingDto> result = new Result<ApplicationSettingDto>();
            try
            {
                ApplicationSetting applicationSetting = _mapper.Map<ApplicationSetting>(applicationSettingDto);
                if (applicationSettingDto.Id != Guid.Empty)
                {
                    var oldModel = _context.ApplicationSettings.FirstOrDefault(x => x.Id == applicationSettingDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, applicationSetting);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(applicationSetting);
                    }
                    else
                        _context.Add(applicationSetting);
                }
                else
                    _context.Add(applicationSetting);
                _context.SaveChanges();
                result.Data = _mapper.Map<ApplicationSettingDto>(applicationSetting);
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

        public Result DeleteApplicationSetting(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.ApplicationSettings.FirstOrDefault(d => d.Id == id);
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

        public Result<ApplicationSettingDto> UpdateApplicationSetting(ApplicationSettingDto applicationSettingDto)
        {
            Result<ApplicationSettingDto> result = new Result<ApplicationSettingDto>();
            try
            {
                ApplicationSetting applicationSetting = _mapper.Map<ApplicationSetting>(applicationSettingDto);
                if (applicationSettingDto.Id != Guid.Empty)
                {
                    var oldModel = _context.ApplicationSettings.FirstOrDefault(x => x.Id == applicationSettingDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, applicationSetting);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(applicationSetting);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<ApplicationSettingDto>(applicationSetting);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<ApplicationSettingDto> GetApplicationSettingById(Guid id)
        {
            Result<ApplicationSettingDto> result = new Result<ApplicationSettingDto>();
            try
            {
                var model = _context.ApplicationSettings.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<ApplicationSettingDto>(model);
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

        public Result<List<ApplicationSettingDto>> GetAllApplicationSetting()
        {
            Result<List<ApplicationSettingDto>> result = new Result<List<ApplicationSettingDto>>();
            try
            {
                var model = _context.ApplicationSettings.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<ApplicationSetting>, List<ApplicationSettingDto>>(model);
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
                var applicationSettings = GetAllApplicationSetting().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    applicationSettings = applicationSettings.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    applicationSettings = applicationSettings.Where(m => m.AboutUs.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = applicationSettings.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = applicationSettings.Count(), recordsTotal = applicationSettings.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}