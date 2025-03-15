using week2_2280602627.Models;

namespace week2_2280602627.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private List<Category> _categoryList;

        public MockCategoryRepository() {
            _categoryList = new List<Category>
            {
                new Category { Id = 1, Name = "Quan tai"},
                new Category { Id = 2, Name = "Khac Hinh Tho"},
            };
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryList;
        }
    }
}
