﻿using amazon_backend.CQRS.Queries.Request.ReviewsRequests;
using amazon_backend.Models;
using amazon_backend.Profiles.CategoryProfiles;
using FluentValidation;
using MediatR;

namespace amazon_backend.CQRS.Queries.Request.CategoryRequests
{
    public class GetCategoryQueryRequest : IRequest<Result<List<CategoryViewProfile>>>
    {
        public int pageSize { get; set; }
        public string categoryName { get; set; }
        public int pageIndex { get; set; }
        public int? categorytId { get; set; }
        public bool? byAsc { get; set; }
       
    }
    public class GetCategoryQueryValidator : AbstractValidator<GetCategoryQueryRequest>
    {
        public GetCategoryQueryValidator()
        {
            RuleFor(x => x.pageSize).NotEmpty().GreaterThan(0);
            RuleFor(x => x.pageIndex).NotEmpty().GreaterThan(0);
            //RuleFor(x => x.categorytId)
            //    .Must(x => Guid.TryParse(x, out var result) == true)
            //    .When(x => string.IsNullOrEmpty(x.categorytId) == false)
            //    .WithMessage("Incorrect {PropertyName} format");
           
        }
    }
}