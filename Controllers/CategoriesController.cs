using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace amazon_backend.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger<CategoriesController> logger;
        private readonly CategoryDao categoryDao;

        public CategoriesController(ILogger<CategoriesController> logger, CategoryDao categoryDao)
        {
            this.logger = logger;
            this.categoryDao = categoryDao;
        }

        [HttpGet]
        public Category[] GetCategories()
        {
            return categoryDao.GetAll();
        }

        [HttpGet]
        [Route("{id}")]
        public Category? GetCategoryById(uint id)
        {
            return categoryDao.GetById(id);
        }


        [HttpPost]
        public ActionResult<Category> CreateCategory([FromBody] Category category)
        {
            if (category != null)
            {
                categoryDao.Add(category);
            }

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCategory(uint id, [FromBody] Category category)
        {
            if (category == null || category.Id != id)
            {
                return BadRequest("Category ID mismatch");
            }

            var existingCategory = categoryDao.GetById(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            categoryDao.Update(category);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCategory(uint id)
        {
            var category = categoryDao.GetById(id);
            if (category == null)
            {
                return NotFound();
            }

            categoryDao.Delete(id);
            return NoContent();
        }

        [HttpPut("restore/{id}")]
        public ActionResult RestoreCategory(uint id)
        {
            var category = categoryDao.GetById(id);
            if (category == null)
            {
                return NotFound();
            }

            categoryDao.Restore(id);
            return NoContent();
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<Category>> GetCategoryByName(string name)
        {
            var category = await categoryDao.GetByName(name);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
    }
}
