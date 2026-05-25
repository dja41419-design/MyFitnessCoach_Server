using Microsoft.EntityFrameworkCore;
using MyFitnessCoach_Server.Models.DTOs;
using MyFitnessCoach_Server.Models.EfModels;

namespace MyFitnessCoach_Server.Repositories
{
	public interface IProductRepository
	{
		IQueryable<ProductDto> GetAllQueryable();
		Task<ProductDto?> GetByIdAsync(int id);
		Task CreateAsync(ProductDto dto);
		Task UpdateAsync(ProductDto dto);
		Task DeactivateAsync(int id);
	}

	public class ProductRepository : IProductRepository
	{
		private readonly MyFitnessCoachDbContext _context;

		public ProductRepository(MyFitnessCoachDbContext context)
		{
			_context = context;
		}

        /// <summary>
        /// 回傳 IQueryable，保留延遲執行，讓上層可繼續組合 Where/OrderBy 後才 materialize。
        /// 使用 inline projection 讓 EF 可翻譯成 SQL，只 SELECT 需要的欄位。
        /// 回傳查詢物件以保留「延遲執行」特性，讓呼叫端可以繼續加上篩選或排序後，才真正對資料庫執行查詢。
        /// 透過直接轉換格式，讓系統能自動產生最佳化的資料庫語法，只抓取需要的欄位以節省效能。
        /// </summary>
        public IQueryable<ProductDto> GetAllQueryable()
		{
			return _context.Products
				.AsNoTracking()
				.Select(p => new ProductDto
				{
					Id = p.Id,
					CategoryId = p.CategoryId,
					Name = p.Name,
					ImageUrl = p.ImageUrl,
					OriginalPrice = p.OriginalPrice,
					UnitPrice = p.UnitPrice,
					Description = p.Description,
					SortOrder = p.SortOrder,
					IsActive = p.IsActive,
					CategoryName = p.Category.CategoryName
				});
		}

		public async Task<ProductDto?> GetByIdAsync(int id)
		{
			return await _context.Products
				.AsNoTracking()
				.Where(p => p.Id == id)
				.Select(p => new ProductDto
				{
					Id = p.Id,
					CategoryId = p.CategoryId,
					Name = p.Name,
					ImageUrl = p.ImageUrl,
					OriginalPrice = p.OriginalPrice,
					UnitPrice = p.UnitPrice,
					Description = p.Description,
					SortOrder = p.SortOrder,
					IsActive = p.IsActive,
					CategoryName = p.Category.CategoryName
				})
				.FirstOrDefaultAsync();
		}

		public async Task CreateAsync(ProductDto dto)
		{
			var product = dto.ToEntity();
			_context.Products.Add(product);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(ProductDto dto)
		{
			var product = await _context.Products.FindAsync(dto.Id);
			if (product == null) return;

			product.CategoryId = dto.CategoryId;
			product.Name = dto.Name;
			product.ImageUrl = dto.ImageUrl ?? string.Empty;
			product.OriginalPrice = dto.OriginalPrice;
			product.UnitPrice = dto.UnitPrice;
			product.Description = dto.Description ?? string.Empty;
			product.SortOrder = dto.SortOrder;
			product.IsActive = dto.IsActive;

			await _context.SaveChangesAsync();
		}

		public async Task DeactivateAsync(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null) return;

			product.IsActive = false;
			await _context.SaveChangesAsync();
		}
	}
}
