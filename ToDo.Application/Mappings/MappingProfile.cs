using AutoMapper;
using ToDo.Application.DTOs.Category;
using ToDo.Application.DTOs.Common;
using ToDo.Application.DTOs.TodoItem;
using ToDo.Domain.Entities;

namespace ToDo.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category
            CreateMap<Category, CategoryDto>();

            CreateMap<Category, DetailCategoryDto>();

            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.TodoItems, opt => opt.Ignore());

            CreateMap<UpdateCategoryDto, Category>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // TodoItem
            CreateMap<TodoItem, TodoItemDto>();

            CreateMap<TodoItem, DetailTodoItemDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

            CreateMap<CreateTodoItemDto, TodoItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<UpdateTodoItemDto, TodoItem>()
                .ForMember(dest => dest.Title, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Title)))
                .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
                .ForMember(dest => dest.IsCompleted, opt => opt.Condition(src => src.IsCompleted.HasValue))
                .ForMember(dest => dest.CategoryId, opt => opt.Condition(src => src.CategoryId.HasValue))
                .ForMember(dest => dest.DueDate, opt => opt.Condition(src => src.DueDate.HasValue))
                .ForMember(dest => dest.Priority, opt => opt.Condition(src => src.Priority.HasValue))
                // Ignore all properties not in UpdateTodoItemDto
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<TodoItem, TodoItemListDto>()
                .ForMember(dest => dest.DescriptionPreview, opt => opt.MapFrom(src =>
                    src.Description != null
                    ? (src.Description.Length <= 100
                        ? src.Description
                        : src.Description.Substring(0, 100) + "...")
                    : null))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.CategoryColor, opt => opt.MapFrom(src => src.Category != null ? src.Category.Color : null));

            //PaginateResult
            CreateMap(typeof(PaginatedResultDto<>), typeof(PaginatedResultDto<>));
        }
    }
}