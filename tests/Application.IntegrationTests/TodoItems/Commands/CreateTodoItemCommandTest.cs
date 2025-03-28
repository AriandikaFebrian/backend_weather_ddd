// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using NetCa.Application.Common.Exceptions;
using NetCa.Application.TodoItems.Commands.CreateTodoItem;
using NetCa.Domain.Constants;
using NetCa.Domain.Entities;
using NetCa.Infrastructure.Data;
using static NetCa.Application.IntegrationTests.Testing;

namespace NetCa.Application.IntegrationTests.TodoItems.Commands;

/// <summary>
/// CreateTodoItemCommandTest
/// </summary>
public class CreateTodoItemCommandTest : BaseTestFixture
{
    private readonly ApplicationDbContext _context = Context;

    /// <summary>
    /// ShouldCreateTodoItem
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldCreateTodoItem()
    {
        var id = Guid.NewGuid();

        var todoItem = new TodoItem
        {
            Id = id,
            Title = "Pencil",
            Description = "Pencil is writing purpose",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            CreatedBy = SystemConstants.Name,
            CreatedDate = DateTimeOffset.Now,
            UpdatedBy = null,
            UpdatedDate = null,
            IsActive = true
        };

        _context.TodoItems.Add(todoItem);

        await _context.SaveChangesAsync().ConfigureAwait(false);

        var test = await _context.TodoItems
            .FirstOrDefaultAsync(x => x.Id.Equals(id)).ConfigureAwait(false);

        test.Should().NotBeNull();
    }

    /// <summary>
    /// ShouldRequiredField
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldRequiredField()
    {
        var command = new CreateTodoItemCommand
        {
            Title = "Pencil",
            Description = "Pencil is writing purpose",
            StartDate = null
        };

        await FluentActions
            .Invoking(() => SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>()
            .ConfigureAwait(false);
    }
}
