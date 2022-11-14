using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.CategoryService
{
    public interface ICategoryService
    {
        Result<CategoryDto> AddCategory(CategoryDto categoryDto);
        Result DeleteCategory(Guid id);
        Result<CategoryDto> UpdateCategory(CategoryDto categoryDto);
        Result<CategoryDto> GetCategoryById(Guid id);
        Result<List<CategoryDto>> GetAllCategory();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
