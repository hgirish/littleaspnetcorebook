using AspNetCoreTodo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AspNetCoreTodo.Controllers
{
    public class TodoController : Controller
    {
        public IActionResult Index(){
            var items = new List<TodoItem>();
            var viewModel = new TodoViewModel { Items = items };
            return View(viewModel);
        }
    }
}