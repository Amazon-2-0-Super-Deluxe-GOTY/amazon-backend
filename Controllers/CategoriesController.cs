using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using amazon_backend.Data;
using FluentValidation;
using amazon_backend.CQRS.Queries.Request.CategoryRequests;
using amazon_backend.CQRS.Commands.CategoryRequests;
using amazon_backend.Services.Response;
using MediatR;
using amazon_backend.Profiles.ReviewProfiles;
using amazon_backend.Profiles.CategoryProfiles;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Data.Model;
using amazon_backend.CQRS.Commands.ReviewRequests;
using Microsoft.AspNetCore.Authorization;
using amazon_backend.CQRS.Commands.CategoryPropertyKeyRequests;
using amazon_backend.Services.JWTService;
using amazon_backend.CQRS.Handlers.QueryHandlers.CategoryPropertyQueryHandlers;
using Amazon.S3.Model;
using System.Xml.Linq;
using amazon_backend.DTO;
using amazon_backend.CQRS.Commands.CategoryImageRequst;
using amazon_backend.CQRS.Queries.Request.CategoryImageRequest;

namespace amazon_backend.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly CategoryDao _categoryDao;
        private readonly DataContext _dataContext;
        private readonly RestResponseService _restResponseService;
        private readonly IMediator _mediator;
        private readonly IValidator<GetCategoryQueryRequest> _getCategoryQueryValidator;
        private readonly RestResponseService _responseService;
        private readonly IValidator<CreateCategoryImageCommandRequst> _createImageValidator;
        private readonly IValidator<RemoveCategoryImageCommandRequst> _removeImageValidator;
        private readonly IValidator<GetCategoryImageByIdQueryRequst> _getImageValidator;
        private readonly IValidator<CreateCategoryPropertyKeyCommandRequst> _createCategoryPropertyKeyCommandValidator;
        private readonly TokenService _tokenService;

        public CategoriesController(ILogger<CategoriesController> logger, CategoryDao categoryDao, DataContext dataContext, RestResponseService restResponseService, IMediator mediator, IValidator<GetCategoryQueryRequest> getCategoryQueryValidator, RestResponseService responseService, IValidator<CreateCategoryImageCommandRequst> createImageValidator, IValidator<RemoveCategoryImageCommandRequst> removeImageValidator, IValidator<GetCategoryImageByIdQueryRequst> getImageValidator, IValidator<CreateCategoryPropertyKeyCommandRequst> createCategoryPropertyKeyCommandValidator, TokenService tokenService)
        {
            _logger = logger;
            _categoryDao = categoryDao;
            _dataContext = dataContext;
            _restResponseService = restResponseService;
            _mediator = mediator;
            _getCategoryQueryValidator = getCategoryQueryValidator;
            _responseService = responseService;
            _createImageValidator = createImageValidator;
            _removeImageValidator = removeImageValidator;
            _getImageValidator = getImageValidator;
            _createCategoryPropertyKeyCommandValidator = createCategoryPropertyKeyCommandValidator;
            _tokenService = tokenService;
        }

        [HttpGet("category")]
        [Authorize]
        public async Task<IActionResult> GetAllCategories()
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return BadRequest();
            }
            User user = decodeResult.data;
            var categories = await _dataContext.Categories
                                      .Include(c => c.CategoryPropertyKeys)
                                      .ToListAsync();
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Image = c.Image,
                IsDeleted = c.IsDeleted,
                IsVisible = c.IsVisible,
                Logo = c.Logo,
                CategoryPropertyKeys = c.CategoryPropertyKeys?.Select(cp => new CategoryPropertyKeyDto
                {
                    Id = cp.Id,
                    Name = cp.Name,
                    IsDeleted = cp.IsDeleted,
                    IsFilter = cp.IsFilter,
                    IsRequired = cp.IsRequired
                }).ToList()
            }).ToList();

            return Ok(categoryDtos);
        }
        [HttpGet("property_keys")]
        [Authorize]
        public async Task<IActionResult> GetAllCategoriesPropertyKey()
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return BadRequest();
            }
            User user = decodeResult.data;
            var categories = await _dataContext.Categories
                                    .Include(c => c.CategoryPropertyKeys)
                                    .ToListAsync();
            return Ok(categories);
        }

        [HttpGet("listPropertyKeysByNameCategory/{name}")]
        [Authorize]
        public async Task<IActionResult> GetListPropertyKeysByNameCategory(string name)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return BadRequest();
            }
            User user = decodeResult.data;
            var categories = await _dataContext.CategoryPropertyKeys.Where(c => c.NameCategory == name).ToListAsync();
            return Ok(categories);
        }

      
        [HttpPost("create_category")]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryModel categoryModel)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return BadRequest();
            }
            User user = decodeResult.data;
            if (categoryModel == null)
            {
                return BadRequest("Category model is null");
            }
            var existingCategory = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Name == categoryModel.Name);

            if (existingCategory != null)
            {
                return BadRequest("Category with the same name already exists");
            }
            CategoryImage? image = await _dataContext
                        .CategoryImages.FirstOrDefaultAsync(pi => pi.Id == Guid.Parse(categoryModel.ImageId));
            var category = new Category
            {
                Name = categoryModel.Name,
                Description = categoryModel.Description,
                IsDeleted = false,
                IsVisible = true,
                ParentCategoryName = categoryModel.ParentCategoryName,
                Logo = categoryModel.Logo
            };

            _dataContext.Categories.AddAsync(category);

            
            await _dataContext.SaveChangesAsync();
            if(image != null)
            {
                category.Image = image.ImageUrl;
            }

            if (categoryModel.PropertyKeys != null && categoryModel.PropertyKeys.Any())
            {
                foreach (var propertyKeyModel in categoryModel.PropertyKeys)
                {
                    
                    propertyKeyModel.CategoryId = category.Id;

                

                    var categoryPropertyKey = new CategoryPropertyKey
                    {
                        Id = Guid.NewGuid(),
                        Name = propertyKeyModel.Name,
                        IsDeleted = propertyKeyModel.IsDeleted,
                        IsFilter = propertyKeyModel.IsFilter,
                        IsRequired = propertyKeyModel.IsRequired,
                        CategoryId = propertyKeyModel.CategoryId 
                    };
                    categoryPropertyKey.NameCategory = category.Name; 
                    _dataContext.CategoryPropertyKeys.Add(categoryPropertyKey);
                }
            }
            await _dataContext.SaveChangesAsync();
            
           
            

           return Ok();
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<ActionResult> UpdateCategory([FromBody] СategoryModel categoryModel)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return BadRequest();
            }
            User user = decodeResult.data;
            if (categoryModel == null)
            {
                return BadRequest("Category is null");
            }

            var category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Name == categoryModel.Name);

            if (category == null)
            {
                return NotFound();
            }


            category.Description = categoryModel.Description;
            category.Image = categoryModel.Image;
            category.IsDeleted = categoryModel.IsDeleted;
            category.IsVisible = categoryModel.IsVisible;
            category.ParentCategoryName = categoryModel.ParentCategory;
            category.Logo = categoryModel.Logo;



            _dataContext.Categories.Update(category);
            await _dataContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("delete/{name}")]
        [Authorize]
        public async Task<ActionResult> DeleteCategory(string name)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return BadRequest();
            }
            User user = decodeResult.data;
            var category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Name == name);
            if (category == null)
            {
                return NotFound();
            }


            category.IsDeleted = true;
            await _dataContext.SaveChangesAsync();


            return Ok();
        }
        [HttpDelete("delete_id/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteCategoryById(int id)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return BadRequest();
            }
            User user = decodeResult.data;
            var category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }


            category.IsDeleted = true;
            await _dataContext.SaveChangesAsync();


            return Ok();
        }

    }
}