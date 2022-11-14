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

namespace BonaForMe.ServiceCore.ProductUnitService
{
    public class ProductUnitService : IProductUnitService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public ProductUnitService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<ProductUnitDto> AddProductUnit(ProductUnitDto productUnitDto)
        {
            Result<ProductUnitDto> result = new Result<ProductUnitDto>();
            try
            {
                ProductUnit productUnit = _mapper.Map<ProductUnit>(productUnitDto);
                if (productUnitDto.Id != Guid.Empty)
                {
                    var oldModel = _context.ProductUnits.FirstOrDefault(x => x.Id == productUnitDto.Id);
                    if (oldModel != null)
                    {
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(productUnit);
                    }
                    else
                        _context.Add(productUnit);
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<ProductUnitDto>(productUnit);
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

        public Result DeleteProductUnit(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.ProductUnits.FirstOrDefault(d => d.Id == id);
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

        public Result<ProductUnitDto> UpdateProductUnit(ProductUnitDto productUnitDto)
        {
            Result<ProductUnitDto> result = new Result<ProductUnitDto>();
            try
            {
                ProductUnit productUnit = _mapper.Map<ProductUnit>(productUnitDto);
                if (productUnitDto.Id != Guid.Empty)
                {
                    var oldModel = _context.ProductUnits.FirstOrDefault(x => x.Id == productUnitDto.Id);
                    if (oldModel != null)
                    {
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(productUnit);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<ProductUnitDto>(productUnit);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<ProductUnitDto> GetProductUnitById(Guid id)
        {
            Result<ProductUnitDto> result = new Result<ProductUnitDto>();
            try
            {
                var model = _context.ProductUnits.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefault();
                result.Data = _mapper.Map<ProductUnitDto>(model);
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

        public Result<List<ProductUnitDto>> GetAllProductUnit()
        {
            Result<List<ProductUnitDto>> result = new Result<List<ProductUnitDto>>();
            try
            {
                var model = _context.ProductUnits.Where(x => x.IsActive && !x.IsDeleted).ToList();
                result.Data = _mapper.Map<List<ProductUnit>, List<ProductUnitDto>>(model);
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
                var productUnits = GetAllProductUnit().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    productUnits = productUnits.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    productUnits = productUnits.Where(m => m.Name.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = productUnits.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = productUnits.Count(), recordsTotal = productUnits.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }
    }
}