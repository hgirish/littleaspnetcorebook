using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreTodo.UnitTests
{
   public class TodoItemServiceShould
    {
        private readonly ITestOutputHelper ouput;

        public TodoItemServiceShould(ITestOutputHelper ouput)
        {
            this.ouput = ouput;
        }
        [Fact]
        public async Task AddNewItemAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_AddNewItem")
                .Options;
            using (var inMemoryContext = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(inMemoryContext);

                var fakeUser = new ApplicationUser
                {
                    Id = "fake-000",
                    UserName = "fake@fake"
                };

                await service.AddItemAsync(new NewTodoItem { Title = "Testing?" }, fakeUser);
            }

            using (var inMemoryContext = new ApplicationDbContext(options))
            {
                Assert.Equal(1, await inMemoryContext.Items.CountAsync());

                var item = await inMemoryContext.Items.FirstAsync();
                Assert.Equal("Testing?", item.Title);
                Assert.Equal(false, item.IsDone);
                ouput.WriteLine($"Due at: {item.DueAt}");
                Assert.Null(item.DueAt);
               // Assert.True(DateTimeOffset.Now.AddDays(3) - item.DueAt < TimeSpan.FromSeconds(1));
            }
        }
        [Fact]
        public async Task MarkDoneAsyncReturnsFalseIfIdDoesNotExistAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "Test_MarkDone")
               .Options;
            using (var inMemoryContext = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(inMemoryContext);
                var fakeUser = new ApplicationUser
                {
                    Id = "fake-000",
                    UserName = "fake@fake"
                };

                await service.AddItemAsync(new NewTodoItem { Title = "Testing?" }, fakeUser);
                var success = await service.MarkDoneAsync(Guid.NewGuid(), fakeUser);
                Assert.False(success);

            }
        }
        [Fact]
        public async Task MarkDoneAsyncReturnsTrueForValidId()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "Test_MarkDone2")
               .Options;
            using (var inMemoryContext = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(inMemoryContext);
                var fakeUser = new ApplicationUser
                {
                    Id = "fake-000",
                    UserName = "fake@fake"
                };

                await service.AddItemAsync(new NewTodoItem { Title = "Testing?" }, fakeUser);
                var item = await inMemoryContext.Items.FirstAsync();
                var success = await service.MarkDoneAsync(item.Id, fakeUser);
                Console.WriteLine(item.Title);
                Assert.True(success);

            }

           
        }

        [Fact]
        public async Task  GetIncompleteItemsAsyncReturnsOnlyItemsOwnedByUser()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "Test_GetIncompleteItems")
               .Options;
            var fakeUser = new ApplicationUser
            {
                Id = "fake-000",
                UserName = "fake@fake"
            };
            var fakeUser2 = new ApplicationUser
            {
                Id = "fake2-001",
                UserName = "fake2@fake2"
            };
            using (var inMemoryContext = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(inMemoryContext);
               

                await service.AddItemAsync(new NewTodoItem { Title = "Item 1" }, fakeUser);
                await service.AddItemAsync(new NewTodoItem { Title = "Item 2" }, fakeUser);
                await service.AddItemAsync(new NewTodoItem { Title = "Item 3" }, fakeUser2);
                await service.AddItemAsync(new NewTodoItem { Title = "Item 4" }, fakeUser2);
                              

            }
            using (var inMemoryContext = new ApplicationDbContext(options))
            {
                Assert.Equal(4, await inMemoryContext.Items.CountAsync());
                var service = new TodoItemService(inMemoryContext);
                var items = await service.GetIncompleteItemsAsync(fakeUser);
                var itemsArray = items.ToArray();
                Assert.Equal(2, items.Count());
                Assert.Equal("Item 1", itemsArray[0].Title);
                Assert.Equal("Item 2", itemsArray[1].Title);

                var items2 = await service.GetIncompleteItemsAsync(fakeUser2);
                var itemsArray2 = items2.ToArray();
                Assert.Equal(2, items2.Count());
                Assert.Equal("Item 3", itemsArray2[0].Title);
                Assert.Equal("Item 4", itemsArray2[1].Title);

            }

        }
    }
}
