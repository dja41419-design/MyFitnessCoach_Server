using MyFitnessCoach_Server.Models.DTOs;
using MyFitnessCoach_Server.Repositories;

namespace MyFitnessCoach_Server.Models.Services
{
	public class ProductService
	{
		private readonly IProductRepository _repository;

		public ProductService(IProductRepository repository)
		{
			_repository = repository;
		}

        /// <summary>
        /// 回傳 IQueryable，上層可再疊加 Where/OrderBy 後才 materialize。
        /// 回傳查詢物件，讓呼叫端可以繼續疊加篩選或排序條件後，才實際執行資料庫查詢。
        /// </summary>
        public IQueryable<ProductDto> GetAllProducts(
			string? name = null,
			int? categoryId = null,
			decimal? minPrice = null,
			decimal? maxPrice = null)
		{
			var query = _repository.GetAllQueryable();

			if (!string.IsNullOrEmpty(name))
			{
				query = query.Where(p => p.Name.Contains(name));
			}

			if (categoryId.HasValue && categoryId.Value > 0)
			{
				query = query.Where(p => p.CategoryId == categoryId.Value);
			}

			if (minPrice.HasValue)
			{
				query = query.Where(p => p.UnitPrice >= minPrice.Value);
			}

			if (maxPrice.HasValue)
			{
				query = query.Where(p => p.UnitPrice <= maxPrice.Value);
			}

			return query;
		}

		public Task<ProductDto?> GetProductAsync(int id)
		{
			return _repository.GetByIdAsync(id);
		}

		public Task CreateProductAsync(ProductDto dto)
		{
			return _repository.CreateAsync(dto);
		}

		public Task UpdateProductAsync(ProductDto dto)
		{
			return _repository.UpdateAsync(dto);
		}

		public Task DeactivateProductAsync(int id)
		{
			return _repository.DeactivateAsync(id);
		}
	}
}
