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
using Microsoft.AspNetCore.Http;
using System.IO;

namespace BonaForMe.ServiceCore.WorkPartnerService
{
    public class WorkPartnerService : IWorkPartnerService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public WorkPartnerService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<WorkPartnerDto> AddWorkPartner(WorkPartnerDto workPartnerDto)
        {
            Result<WorkPartnerDto> result = new Result<WorkPartnerDto>();
            try
            {
                WorkPartner workPartner = _mapper.Map<WorkPartner>(workPartnerDto);
                if (workPartnerDto.Id != Guid.Empty)
                {
                    var oldModel = _context.WorkPartners.FirstOrDefault(x => x.Id == workPartnerDto.Id);
                    if (oldModel != null)
                    {
                        if (workPartnerDto.FormFile == null)
                            workPartner.ImagePath = oldModel.ImagePath;

                        DBHelper.SetBaseValues(oldModel, workPartner);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(workPartner);
                    }
                    else
                        _context.Add(workPartner);
                }
                else
                    _context.Add(workPartner);
                _context.SaveChanges();

                if (workPartnerDto.FormFile != null)
                    SaveImage(workPartnerDto.FormFile, workPartner);
                result.Data = _mapper.Map<WorkPartnerDto>(workPartner);
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

        public Result DeleteWorkPartner(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.WorkPartners.FirstOrDefault(d => d.Id == id);
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

        public Result<WorkPartnerDto> UpdateWorkPartner(WorkPartnerDto workPartnerDto)
        {
            Result<WorkPartnerDto> result = new Result<WorkPartnerDto>();
            try
            {
                WorkPartner workPartner = _mapper.Map<WorkPartner>(workPartnerDto);
                if (workPartnerDto.Id != Guid.Empty)
                {
                    var oldModel = _context.WorkPartners.FirstOrDefault(x => x.Id == workPartnerDto.Id);
                    if (oldModel != null)
                    {
                        if (workPartnerDto.FormFile == null)
                            workPartner.ImagePath = oldModel.ImagePath;

                        DBHelper.SetBaseValues(oldModel, workPartner);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(workPartner);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<WorkPartnerDto>(workPartner);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<WorkPartnerDto> GetWorkPartnerById(Guid id)
        {
            Result<WorkPartnerDto> result = new Result<WorkPartnerDto>();
            try
            {
                var model = _context.WorkPartners.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<WorkPartnerDto>(model);
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

        public Result<List<WorkPartnerDto>> GetAllWorkPartner()
        {
            Result<List<WorkPartnerDto>> result = new Result<List<WorkPartnerDto>>();
            try
            {
                var model = _context.WorkPartners.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<WorkPartner>, List<WorkPartnerDto>>(model);
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
                var workPartners = GetAllWorkPartner().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    workPartners = workPartners.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    workPartners = workPartners.Where(m => m.Description.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = workPartners.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = workPartners.Count(), recordsTotal = workPartners.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }

        private void SaveImage(IFormFile formFile, WorkPartner workPartner)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory()).Replace("api.bonameforme.com", "httpdocs") + @"\";
            byte[] picture = null;
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                picture = ms.ToArray();
            }

            var fileExtension = "." + formFile.FileName.Split('.').Last();
            var filePath = @"\images\WorkPartner\" + workPartner.Id + fileExtension;
            var fullpath = path + "wwwroot" + filePath;

            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
            File.WriteAllBytes(fullpath, picture);

            workPartner.ImagePath = filePath;
            UpdateWorkPartner(_mapper.Map<WorkPartnerDto>(workPartner));
        }
    }
}