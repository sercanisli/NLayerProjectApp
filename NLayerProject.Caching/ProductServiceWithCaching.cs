using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NLayerProject.Core.DTOs;
using NLayerProject.Core.Models;
using NLayerProject.Core.Repositories;
using NLayerProject.Core.Services;
using NLayerProject.Core.UnitOfWorks;
using NLayerProject.Service.Excepitons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NLayerProject.Caching
{
    public class ProductServiceWithCaching : IProductService
    {
        private const string CacheProductKey = "productsKey";
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductServiceWithCaching(IUnitOfWork unitOfWork, IProductRepository productRepository, IMemoryCache memoryCache, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _memoryCache = memoryCache;
            _mapper = mapper;


            //Is there a value for this cache? We don't want to get that value _
            if (!_memoryCache.TryGetValue(CacheProductKey, out _))
            {   //create cache if it doesn't exist
                _memoryCache.Set(CacheProductKey, _productRepository.GetProductsWithCategory().Result);
            }
        }

        public async Task<Product> AddAsync(Product entity)
        {
            await _productRepository.AddAsync(entity);
            await _unitOfWork.ComitAsync();
            await CacheAllProductsAsync();
            return entity;
        }

        public async Task<IEnumerable<Product>> AddRangeAsync(IEnumerable<Product> entities)
        {
            await _productRepository.AddRangeAsync(entities);
            await _unitOfWork.ComitAsync();
            await CacheAllProductsAsync();
            return entities;
        }

        public Task<bool> AnyAsync(Expression<Func<Product, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult(_memoryCache.Get<IEnumerable<Product>>(CacheProductKey));
        }

        public Task<Product> GetByIdAsync(int id)
        {
            var product = _memoryCache.Get<List<Product>>(CacheProductKey).FirstOrDefault(x => x.Id == id);
            if(product == null)
            {
                throw new NotFoundException($"{typeof(Product).Name}({id}) not found");
            }
            return Task.FromResult(product);
        }

        public Task<List<ProductWithCategoryDto>> GetProductsWithCategory()
        {
            var products = _memoryCache.Get<IEnumerable<Product>>(CacheProductKey);
            var productsWithCategoryDto = _mapper.Map<List<ProductWithCategoryDto>>(products);
            return Task.FromResult(productsWithCategoryDto);
        }

        public async Task RemoveAsync(Product entity)
        {
            _productRepository.Remove(entity);
            await _unitOfWork.ComitAsync();
            await CacheAllProductsAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<Product> entities)
        {
            _productRepository.RemoveRange(entities);
            await _unitOfWork.ComitAsync();
            await CacheAllProductsAsync(); 
        }

        public async Task UpdateAsync(Product entity)
        {
            _productRepository.Update(entity);
            await _unitOfWork.ComitAsync();
            await CacheAllProductsAsync();
        }

        public IQueryable<Product> Where(Expression<Func<Product, bool>> expression)
        {
            return _memoryCache.Get<List<Product>>(CacheProductKey).Where(expression.Compile()).AsQueryable();
        }

        //Caching Method
        public async Task CacheAllProductsAsync()
        {
            _memoryCache.Set(CacheProductKey, await _productRepository.GetAll().ToListAsync());
        }
    }
}
