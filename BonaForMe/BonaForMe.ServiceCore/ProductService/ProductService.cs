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

namespace BonaForMe.ServiceCore.ProductService
{
    public class ProductService : IProductService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public ProductService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<ProductDto> AddProduct(ProductDto productDto)
        {
            Result<ProductDto> result = new Result<ProductDto>();
            try
            {
                Product product = _mapper.Map<Product>(productDto);
                if (productDto.Id != Guid.Empty)
                {
                    var oldModel = _context.Products.FirstOrDefault(x => x.Id == productDto.Id);
                    if (oldModel != null)
                    {
                        if (productDto.FormFile == null)
                            product.ImagePath = oldModel.ImagePath;

                        DBHelper.SetBaseValues(oldModel, product);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(product);
                    }
                    else
                        _context.Add(product);
                }
                else
                    _context.Add(product);
                _context.SaveChanges();

                if (productDto.FormFile != null)
                    SaveImage(productDto.FormFile, product);
                result.Data = _mapper.Map<ProductDto>(product);
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

        public Result DeleteProduct(Guid id)
        {
            Result result = new Result();
            try
            {
                var model = _context.Products.FirstOrDefault(d => d.Id == id);
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

        public Result<ProductDto> UpdateProduct(ProductDto productDto)
        {
            Result<ProductDto> result = new Result<ProductDto>();
            try
            {
                Product product = _mapper.Map<Product>(productDto);
                if (productDto.Id != Guid.Empty)
                {
                    var oldModel = _context.Products.FirstOrDefault(x => x.Id == productDto.Id);
                    if (oldModel != null)
                    {
                        if (productDto.FormFile == null)
                            product.ImagePath = oldModel.ImagePath;

                        DBHelper.SetBaseValues(oldModel, product);
                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(product);
                    }
                }
                _context.SaveChanges();
                result.Data = _mapper.Map<ProductDto>(product);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<ProductDto> GetProductById(Guid id)
        {
            Result<ProductDto> result = new Result<ProductDto>();
            try
            {
                var model = _context.Products.Where(x => x.Id == id && x.IsActive && !x.IsDeleted)
                    .Include(x => x.ProductUnit).Include(x => x.CurrencyUnit).Include(x => x.Category)
                    .FirstOrDefault();
                result.Data = _mapper.Map<ProductDto>(model);
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

        public Result<List<ProductDto>> GetAllProduct()
        {
            Result<List<ProductDto>> result = new Result<List<ProductDto>>();
            try
            {
                var model = _context.Products.Where(x => x.IsActive && !x.IsDeleted)
                    .Include(x => x.ProductUnit).Include(x => x.CurrencyUnit).Include(x => x.Category)
                    .ToList();
                result.Data = _mapper.Map<List<Product>, List<ProductDto>>(model);
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

        public Result<List<ProductDto>> GetRandomProduct(int count = 5)
        {
            Result<List<ProductDto>> result = new Result<List<ProductDto>>();
            try
            {
                Random rand = new Random();
                int toSkip = rand.Next(0, _context.Products.Where(x => x.IsActive && !x.IsDeleted).Count() - count);

                var model = _context.Products.Where(x => x.IsActive && !x.IsDeleted)
                    .Include(x => x.ProductUnit).Include(x => x.CurrencyUnit).Include(x => x.Category).Skip(toSkip).Take(count)
                    .ToList();
                result.Data = _mapper.Map<List<Product>, List<ProductDto>>(model);
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
                var products = GetAllProduct().Data.AsQueryable();
                //Sorting
                if (!string.IsNullOrEmpty(dataTable.SortColumn) && !string.IsNullOrEmpty(dataTable.SortColumnDirection))
                {
                    products = products.OrderBy(dataTable.SortColumn + " " + dataTable.SortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(dataTable.SearchValue))
                {
                    products = products.Where(m => m.Description.ToLower().Contains(dataTable.SearchValue.ToLower()));
                }
                var data = products.Skip(dataTable.Skip).Take(dataTable.PageSize);
                return new JsonResult(new { success = true, message = ResultMessages.Success, draw = dataTable.Draw, recordsFiltered = products.Count(), recordsTotal = products.Count(), data = data });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex });
            }
        }

        public Result<List<ProductDto>> GetAllProductByCategoryId(Guid categoryId)
        {
            Result<List<ProductDto>> result = new Result<List<ProductDto>>();
            try
            {
                var model = _context.Products.Where(x => x.CategoryId == categoryId && x.IsActive && !x.IsDeleted)
                    .Include(x => x.ProductUnit).Include(x => x.CurrencyUnit).Include(x => x.Category)
                    .ToList();
                result.Data = _mapper.Map<List<Product>, List<ProductDto>>(model);
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

        public Result UpdateProductStock(Guid Id, int newStock, int reducedStock)
        {
            Result result = new Result();
            try
            {
                if (Id != Guid.Empty)
                {
                    var oldModel = _context.Products.FirstOrDefault(x => x.Id == Id);
                    if (oldModel != null)
                    {
                        if (reducedStock != 0)
                            oldModel.Stock -= reducedStock;
                        else
                            oldModel.Stock = newStock;

                        _context.Entry(oldModel).State = EntityState.Detached;
                        _context.Update(oldModel);
                    }
                }
                _context.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        private void SaveImage(IFormFile formFile, Product product)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory()) + @"\";
            byte[] picture = null;
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                picture = ms.ToArray();
            }

            var fileExtension = "." + formFile.FileName.Split('.').Last();
            var filePath = @"\images\Product\" + product.Id + fileExtension;
            var fullpath = path + "wwwroot" + filePath;

            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
            File.WriteAllBytes(fullpath, picture);

            product.ImagePath = filePath;
            UpdateProduct(_mapper.Map<ProductDto>(product));
        }
    }
}