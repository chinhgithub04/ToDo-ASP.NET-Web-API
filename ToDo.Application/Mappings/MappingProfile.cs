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
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            //PaginateResult
            CreateMap(typeof(PaginatedResultDto<>), typeof(PaginatedResultDto<>));
        }
    }
}