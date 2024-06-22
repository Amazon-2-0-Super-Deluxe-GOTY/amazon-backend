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
using amazon_backend.Services.Response;
using MediatR;
using amazon_backend.Profiles.ReviewProfiles;
using amazon_backend.Profiles.CategoryProfiles;
using amazon_backend.Services.FluentValidation;
using amazon_backend.Data.Model;
using amazon_backend.CQRS.Commands.ReviewRequests;
using Microsoft.AspNetCore.Authorization;
using amazon_backend.Services.JWTService;
using Amazon.S3.Model;
using System.Xml.Linq;
using amazon_backend.DTO;
using amazon_backend.CQRS.Commands.CategoryImageRequst;
using amazon_backend.CQRS.Queries.Request.CategoryImageRequest;
using Google.Protobuf.WellKnownTypes;
using amazon_backend.Migrations;

namespace amazon_backend.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly CategoryDao _categoryDao;
        private readonly DataContext _dataContext;
        private readonly IMediator _mediator;
        private readonly RestResponseService _responseService;
        private readonly IValidator<CreateCategoryImageCommandRequst> _createImageValidator;
        private readonly IValidator<RemoveCategoryImageCommandRequst> _removeImageValidator;
        private readonly IValidator<GetCategoryImageByIdQueryRequst> _getImageValidator;
        private readonly TokenService _tokenService;

        public CategoriesController(ILogger<CategoriesController> logger, CategoryDao categoryDao, DataContext dataContext, RestResponseService restResponseService, IMediator mediator, RestResponseService responseService, IValidator<CreateCategoryImageCommandRequst> createImageValidator, IValidator<RemoveCategoryImageCommandRequst> removeImageValidator, IValidator<GetCategoryImageByIdQueryRequst> getImageValidator, TokenService tokenService)
        {
            _logger = logger;
            _categoryDao = categoryDao;
            _dataContext = dataContext;
            _mediator = mediator;
            _responseService = responseService;
            _createImageValidator = createImageValidator;
            _removeImageValidator = removeImageValidator;
            _getImageValidator = getImageValidator;
            _tokenService = tokenService;
        }


        [HttpGet("category")]
        public async Task<IActionResult> GetAllCategories([FromQuery] PaginationDto paginationDto)
        {
            if (paginationDto.PageNumber <= 0 || paginationDto.PageSize <= 0)
            {
                return BadRequest("Invalid pagination parameters.");
            }

            var totalCategories = await _dataContext.Categories.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCategories / (double)paginationDto.PageSize);



            var categories = await _dataContext.Categories
                               .Include(c => c.CategoryPropertyKeys)
                               .Include(c => c.Image)
                               .Where(c => c.IsActive == true)
                               .OrderBy(c => c.CreatedAt)
                               .Skip((paginationDto.PageNumber - 1) * paginationDto.PageSize)
                               .Take(paginationDto.PageSize)
                               .ToListAsync();

            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                ParentId = c.ParentCategoryId,
                Name = c.Name,
                Description = c.Description,
                Image = new CategoryImageDTO
                {
                    Id = c.Image.Id,
                    Url = "https://perry11.s3.eu-north-1.amazonaws.com/" + c.Image.ImageUrl
                },
                IsActive = c.IsActive,
                Logo = c.Logo,
                CategoryPropertyKeys = c.CategoryPropertyKeys?.Select(cp => new CategoryPropertyKeyDto
                {
                    Id = cp.Id,
                    Name = cp.Name,
                }).ToList()
            }).ToList();

            return Ok(categoryDtos);
        }
        [HttpGet("category_admin")]
        [Authorize]
        public async Task<IActionResult> GetAllCategoriesAdmin([FromQuery] PaginationDto paginationDto)
        {
            if (paginationDto.PageNumber <= 0 || paginationDto.PageSize <= 0)
            {
                return BadRequest("Invalid pagination parameters.");
            }
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return BadRequest();
            }
            User user = decodeResult.data;


            var totalCategories = await _dataContext.Categories.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCategories / (double)paginationDto.PageSize);


            var categories = await _dataContext.Categories
                              .Include(c => c.CategoryPropertyKeys)
                              .Include(c => c.Image)
                              .OrderBy(c => c.CreatedAt)
                              .Skip((paginationDto.PageNumber - 1) * paginationDto.PageSize)
                              .Take(paginationDto.PageSize)
                              .ToListAsync();
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                ParentId = c.ParentCategoryId,
                Name = c.Name,
                Description = c.Description,
                Image = new CategoryImageDTO
                {
                    Id = c.Image.Id,
                    Url = "https://perry11.s3.eu-north-1.amazonaws.com/" + c.Image.ImageUrl
                },
                IsActive = c.IsActive,
                Logo = c.Logo,
                CategoryPropertyKeys = c.CategoryPropertyKeys?.Select(cp => new CategoryPropertyKeyDto
                {
                    Id = cp.Id,
                    Name = cp.Name,
                }).ToList()
            }).ToList();



            return Ok(categoryDtos);
        }


        [HttpGet("category/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _dataContext.Categories
                                      .Include(c => c.CategoryPropertyKeys)
                                      .Include(c => c.Image)
                                      .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                ParentId = category.ParentCategoryId,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                Logo = category.Logo,
                Image = category.Image != null ? new CategoryImageDTO
                {
                    Id = category.Image.Id,
                    Url = "https://perry11.s3.eu-north-1.amazonaws.com/" + category.Image.ImageUrl
                } : null,
                CategoryPropertyKeys = category.CategoryPropertyKeys?.Select(cp => new CategoryPropertyKeyDto
                {
                    Id = cp.Id,
                    Name = cp.Name,
                }).ToList()
            };

            return Ok(categoryDto);

        }

        [HttpGet("property_keys")]
        public async Task<IActionResult> GetAllCategoriesPropertyKey()
        {


            var categories = await _dataContext.Categories
                                    .Include(c => c.CategoryPropertyKeys)
                                    .ToListAsync();
            return Ok(categories);
        }

        [HttpGet("listPropertyKeysByNameCategory/{name}")]
        public async Task<IActionResult> GetListPropertyKeysByNameCategory(string name)
        {

            var categories = await _dataContext.CategoryPropertyKeys.Where(c => c.NameCategory == name).ToListAsync();
            return Ok(categories);
        }


        [HttpPost("create_category")]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryModel categoryModel)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return Unauthorized();
            }

            User user = decodeResult.data;

            if (categoryModel == null)
            {
                return BadRequest("Category model is null");
            }

            var imageId = Guid.Parse(categoryModel.ImageId);
            var image = await _dataContext.CategoryImages.FirstOrDefaultAsync(pi => pi.Id == imageId);
            if (image == null)
            {
                return BadRequest("Image not found.");
            }

            var category = new Data.Entity.Category
            {
                Name = categoryModel.Name,
                Description = categoryModel.Description,
                ParentCategoryId = categoryModel.ParentCategoryId,
                IsActive = categoryModel.IsActive,
                Logo = categoryModel.Logo,
                Image = image,
                CreatedAt = DateTime.UtcNow
            };

            await _dataContext.Categories.AddAsync(category);
            await _dataContext.SaveChangesAsync();

            if (categoryModel.PropertyKeys != null && categoryModel.PropertyKeys.Any())
            {
                foreach (var propertyKeyModel in categoryModel.PropertyKeys)
                {
                    var categoryPropertyKey = new CategoryPropertyKey
                    {
                        Id = Guid.NewGuid(),
                        Name = propertyKeyModel.Name,
                        CategoryId = category.Id,
                        NameCategory = category.Name
                    };

                    _dataContext.CategoryPropertyKeys.Add(categoryPropertyKey);
                }
            }

            await _dataContext.SaveChangesAsync();

            return Ok();
        }



        [HttpPut("update")]
        [Authorize]
        public async Task<ActionResult> UpdateCategory([FromBody] UpdateCategoryModel categoryModel)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return Unauthorized();
            }

            User user = decodeResult.data;

            if (categoryModel == null)
            {
                return BadRequest("Category model is null");
            }

            var category = await _dataContext.Categories
                .Include(c => c.CategoryPropertyKeys)
                .FirstOrDefaultAsync(c => c.Id == categoryModel.Id);

            if (category == null)
            {
                return NotFound("Category not found");
            }

            category.Name = categoryModel.Name;
            category.ParentCategoryId = (uint?)categoryModel.ParentCategoryId;
            category.Description = categoryModel.Description;
            category.IsActive = categoryModel.IsActive;
            category.Logo = categoryModel.Logo;

            if (string.IsNullOrEmpty(categoryModel.ImageId) || !Guid.TryParse(categoryModel.ImageId, out var imageId))
            {
                return BadRequest("Invalid or missing image ID.");
            }

            var image = await _dataContext.CategoryImages.FirstOrDefaultAsync(pi => pi.Id == imageId);
            if (image == null)
            {
                return BadRequest("Image not found.");
            }
            category.Image = image;

            var existingPropertyKeys = await _dataContext.CategoryPropertyKeys
                .Where(cpk => cpk.CategoryId == category.Id)
                .ToListAsync();
            _dataContext.CategoryPropertyKeys.RemoveRange(existingPropertyKeys);

            if (categoryModel.PropertyKeys != null && categoryModel.PropertyKeys.Any())
            {
                foreach (var propertyKeyModel in categoryModel.PropertyKeys)
                {
                    var categoryPropertyKey = new CategoryPropertyKey
                    {
                        Id = Guid.NewGuid(),
                        Name = propertyKeyModel.Name,
                        CategoryId = category.Id,
                        NameCategory = category.Name
                    };

                    _dataContext.CategoryPropertyKeys.Add(categoryPropertyKey);
                }
            }

            _dataContext.Categories.Update(category);
            await _dataContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete/{name}")]
        [Authorize]
        public async Task<ActionResult> DeleteCategory(string name)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
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


            category.IsActive = false;
            await _dataContext.SaveChangesAsync();


            return Ok();
        }
        [HttpDelete("delete_id/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteCategoryById(int id)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
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


            category.IsActive = false;
            await _dataContext.SaveChangesAsync();


            return Ok();
        }

    }
}