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

namespace BonaForMe.ServiceCore.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public CategoryService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<CategoryDto> AddCategory(CategoryDto categoryDto)
        {
            Result<CategoryDto> result = new Result<CategoryDto>();
            try
            {
                Category category = _mapper.Map<Category>(categoryDto);
                if (categoryDto.Id != Guid.Empty)
                {
                    var oldModel = _context.Categories.FirstOrDefault(x => x.Id == categoryDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, category);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(category);
                    }
                    else
                        _context.Add(category);
                }
                else
                    _context.Add(category);
                _context.SaveChanges();
                result.Data = _mapper.Map<CategoryDto>(category);
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

        public Result DeleteCategory(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.Categories.FirstOrDefault(d => d.Id == id);
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

        public Result<CategoryDto> UpdateCategory(CategoryDto categoryDto)
        {
            Result<CategoryDto> result = new Result<CategoryDto>();
            try
            {
                Category category = _mapper.Map<Category>(categoryDto);
                if (categoryDto.Id != Guid.Empty)
                {
                    var oldModel = _context.Categories.FirstOrDefault(x => x.Id == categoryDto.Id);
                    if (oldModel != null)
                    {
                        DBHelper.SetBaseValues(oldModel, category);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(category);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<CategoryDto>(category);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<CategoryDto> GetCategoryById(Guid id)
        {
            Result<CategoryDto> result = new Result<CategoryDto>();
            try
            {
                var model = _context.Categories.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<CategoryDto>(model);
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

        public Result<List<CategoryDto>> GetAllCategory()
        {
            Result<List<CategoryDto>> result = new Result<List<CategoryDto>>();
            try
            {
                var model = _context.Categories.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<Category>, List<CategoryDto>>(model);
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
                var categorys = GetAllCategory().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    categorys = categorys.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    categorys = categorys.Where(m => m.Description.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = categorys.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = categorys.Count(), recordsTotal = categorys.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}