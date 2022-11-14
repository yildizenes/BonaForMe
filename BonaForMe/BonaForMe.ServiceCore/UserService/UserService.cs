using AutoMapper;
using BonaForMe.DataAccessCore;
using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DBModel;
using BonaForMe.DomainCore.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Microsoft.AspNetCore.Mvc;

namespace BonaForMe.ServiceCore.UserService
{
    public class UserService : IUserService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public UserService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<UserDto> AddUser(UserDto userDto)
        {
            Result<UserDto> result = new Result<UserDto>();
            try
            {
                User user = _mapper.Map<User>(userDto);
                if (userDto.Id != Guid.Empty)
                {
                    var oldModel = _context.Users.FirstOrDefault(x => x.Id == userDto.Id);
                    if (oldModel != null)
                    {
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(user);
                    }
                    else
                        _context.Add(user);
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<UserDto>(user);
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

        public Result DeleteUser(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.Users.FirstOrDefault(d => d.Id == id);
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

        public Result<UserDto> UpdateUser(UserDto userDto)
        {
            Result<UserDto> result = new Result<UserDto>();
            try
            {
                User user = _mapper.Map<User>(userDto);
                if (userDto.Id != Guid.Empty)
                {
                    var oldModel = _context.Users.FirstOrDefault(x => x.Id == userDto.Id);
                    if (oldModel != null)
                    {
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(user);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<UserDto>(user);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<UserDto> GetUserById(Guid id)
        {
            Result<UserDto> result = new Result<UserDto>();
            try
            {
                var model = _context.Users.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<UserDto>(model);
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
        public Result<List<UserDto>> GetAllUser()
        {
            Result<List<UserDto>> result = new Result<List<UserDto>>();
            try
            {
                var model = _context.Users.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<User>, List<UserDto>>(model);
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
                var users = GetAllUser().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    users = users.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    users = users.Where(m => m.FirstName.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = users.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = users.Count(), recordsTotal = users.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
        
        public Result<UserDto> GetUserByEmail(string userMail)
        {
            Result<UserDto> result = new Result<UserDto>();
            try
            {
                var model = _context.Users.Where(x => x.UserMail == userMail && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<UserDto>(model);
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
    }
}