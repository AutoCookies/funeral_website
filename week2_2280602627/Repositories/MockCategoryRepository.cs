using week2_2280602627.Models;

namespace week2_2280602627.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private List<Category> _categoryList;

        public MockCategoryRepository()
        {
            _categoryList = new List<Category>
            {
                new Category { Id = 1, Name = "Quan Tài" },
                new Category { Id = 2, Name = "Khắc Hình Thờ" }
            };
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryList;
        }

        public Category GetById(int id) // Thêm phương thức này để lấy danh mục theo Id
        {
            return _categoryList.FirstOrDefault(c => c.Id == id);
        }
    }
}
