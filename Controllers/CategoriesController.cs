using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoriesController
    {
        private readonly ILogger<CategoriesController> logger;
        private readonly CategoryDao categoryDao;

        public CategoriesController(ILogger<CategoriesController> logger, CategoryDao categoryDao)
        {
            this.logger = logger;
            this.categoryDao = categoryDao;
        }

        [HttpGet]
        public Category[] GetCategories([FromQuery] string? name)
        {
            if(name is not null)
            {
                return categoryDao.GetByName(name);
            }

            return categoryDao.GetAll();
        }

        [HttpGet]
        [Route("{id}")]
        public Category? GetCategoryById(uint id)
        {
            return categoryDao.GetById(id);
        }
    }
}
