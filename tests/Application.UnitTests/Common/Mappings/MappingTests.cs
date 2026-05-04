using System.Reflection;
using System.Runtime.CompilerServices;
using AutoMapper;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Application.Common.Models;
using Heven.Api.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Heven.Api.Application.TodoLists.Queries.GetTodos;
using Heven.Api.Domain.Entities;
using NUnit.Framework;

namespace Heven.Api.Application.UnitTests.Common.Mappings;

public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        // Cách này chia nhỏ ra để trình biên dịch không bị nhầm lẫn tham số
        var configurationConfigurationExpression = new MapperConfigurationExpression();

        configurationConfigurationExpression.AddMaps(typeof(IApplicationDbContext).Assembly);

        _configuration = new MapperConfiguration(configurationConfigurationExpression);
        _mapper = _configuration.CreateMapper();
    }

    [Test]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }

    [Test]
    [TestCase(typeof(TodoList), typeof(TodoListDto))]
    [TestCase(typeof(TodoItem), typeof(TodoItemDto))]
    [TestCase(typeof(TodoList), typeof(LookupDto))]
    [TestCase(typeof(TodoItem), typeof(LookupDto))]
    [TestCase(typeof(TodoItem), typeof(TodoItemBriefDto))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = GetInstanceOf(source);

        _mapper.Map(instance, source, destination);
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        // Type without parameterless constructor
        return RuntimeHelpers.GetUninitializedObject(type);
    }
}
