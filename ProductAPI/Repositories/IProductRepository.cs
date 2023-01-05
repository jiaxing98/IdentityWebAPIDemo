using ProductApi.Dtos;

namespace ProductApi.Repositories
{
    public interface IProductRepository
    {
        Task<ProductDto> CreateProduct(ProductDto productDto);
        Task<bool> DeleteProduct(int productId);
        Task<ProductDto> GetProductById(int productId);
        Task<IEnumerable<ProductDto>> GetProducts();
        Task<ProductDto> UpdateProduct(ProductDto productDto);
    }
}