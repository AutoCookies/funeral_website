using week2_2280602627.Models;

public interface ICategoryRepository
{
    IEnumerable<Category> GetAllCategories();
    Category GetById(int id);
}
