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

namespace BonaForMe.ServiceCore.AnnouncementService
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public AnnouncementService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<AnnouncementDto> AddAnnouncement(AnnouncementDto announcementDto)
        {
            Result<AnnouncementDto> result = new Result<AnnouncementDto>();
            try
            {
                Announcement announcement = _mapper.Map<Announcement>(announcementDto);
                if (announcementDto.Id != Guid.Empty)
                {
                    var oldModel = _context.Announcements.FirstOrDefault(x => x.Id == announcementDto.Id);
                    if (oldModel != null)
                    {
                        if (announcementDto.FormFile == null)
                            announcement.ImagePath = oldModel.ImagePath;

                        DBHelper.SetBaseValues(oldModel, announcement);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(announcement);
                    }
                    else
                        _context.Add(announcement);
                }
                else
                    _context.Add(announcement);
                _context.SaveChanges();

                if (announcementDto.FormFile != null)
                    SaveImage(announcementDto.FormFile, announcement);
                result.Data = _mapper.Map<AnnouncementDto>(announcement);
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

        public Result DeleteAnnouncement(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.Announcements.FirstOrDefault(d => d.Id == id);
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

        public Result<AnnouncementDto> UpdateAnnouncement(AnnouncementDto announcementDto)
        {
            Result<AnnouncementDto> result = new Result<AnnouncementDto>();
            try
            {
                Announcement announcement = _mapper.Map<Announcement>(announcementDto);
                if (announcementDto.Id != Guid.Empty)
                {
                    var oldModel = _context.Announcements.FirstOrDefault(x => x.Id == announcementDto.Id);
                    if (oldModel != null)
                    {
                        if (announcementDto.FormFile == null)
                            announcement.ImagePath = oldModel.ImagePath;

                        DBHelper.SetBaseValues(oldModel, announcement);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(announcement);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<AnnouncementDto>(announcement);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<AnnouncementDto> GetAnnouncementById(Guid id)
        {
            Result<AnnouncementDto> result = new Result<AnnouncementDto>();
            try
            {
                var model = _context.Announcements.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).Include(x => x.Category).FirstOrDefault();
                result.Data = _mapper.Map<AnnouncementDto>(model);
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

        public Result<List<AnnouncementDto>> GetAllAnnouncement()
        {
            Result<List<AnnouncementDto>> result = new Result<List<AnnouncementDto>>();
            try
            {
                var model = _context.Announcements.Where(x => x.IsActive && !x.IsDeleted).Include(x => x.Category).ToList();
                result.Data = _mapper.Map<List<Announcement>, List<AnnouncementDto>>(model);
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
                var announcements = GetAllAnnouncement().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    announcements = announcements.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    announcements = announcements.Where(m => m.Description.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = announcements.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = announcements.Count(), recordsTotal = announcements.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }

        private void SaveImage(IFormFile formFile, Announcement announcement)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory()) + @"\";
            byte[] picture = null;
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                picture = ms.ToArray();
            }

            var fileExtension = "." + formFile.FileName.Split('.').Last();
            var filePath = @"\images\Announcement\" + announcement.Id + fileExtension;
            var fullpath = path + "wwwroot" + filePath;

            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
            File.WriteAllBytes(fullpath, picture);

            announcement.ImagePath = filePath;
            UpdateAnnouncement(_mapper.Map<AnnouncementDto>(announcement));
        }
    }
}