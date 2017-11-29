using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AspNetCoreTodo.Controllers
{
    public class TodoController : Controller
    {
        private readonly ITodoItemService _todoItemService;

        public TodoController(ITodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }
        public async System.Threading.Tasks.Task<IActionResult> Index(){
            var items =  await _todoItemService.GetIncompleteItemsAsync();
            var viewModel = new TodoViewModel { Items = items };
            return View(viewModel);
        }
    }
}