using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Controllers;

public class HomeController : Controller
{
    private ToDoContext context;

    public HomeController(ToDoContext ctx) => context = ctx;

    public IActionResult Index(string id)
    {
        var filters = new Filters(id);
        ViewBag.Filters = filters;

        ViewBag.Categories = context.Categories.ToList();
        ViewBag.Statuses = context.Statuses.ToList();
        ViewBag.DueFilters = Filters.DueFilterValues;

        IQueryable<ToDo> query = context.Todos
            .Include(t => t.Category)
            .Include(t => t.Status);

        if (filters.HasCategory)
        {
            query = query.Where(t => t.CategoryId == filters.CategoryId);
        }

        if (filters.HasStatus)
        {
            query = query.Where(t => t.StatusId == filters.StatusId);
        }

        if (filters.HasDue)
        {
            var today = DateTime.Today;
            if (filters.IsPast)
            {
                query = query.Where(t => t.DueDate < today);
            }
            else if(filters.IsFuture)
            {
                query = query.Where(t => t.DueDate > today);
            }
            else if(filters.IsToday)
            {
                query = query.Where(t => t.DueDate == today);
            }
        }
        var task = query.OrderBy(t => t.DueDate).ToList();

        return View(task);
    }

    [HttpGet]
    public IActionResult Add()
    {
        ViewBag.Categories = context.Categories.ToList();
        ViewBag.Statuses = context.Statuses.ToList();
        var task = new ToDo { StatusId = "open" };

        return View(task);
    }

    [HttpPost]
    public IActionResult Add(ToDo task)
    {
        if (ModelState.IsValid)
        {
            context.Todos.Add(task);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        else
        {
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            return View(task);
        }
    }

    [HttpPost]
    public IActionResult Filter(string[] filter)
    {
        string id = string.Join("-", filter);
        return RedirectToAction("Index", new { ID = id });
    }

    [HttpPost]
    public IActionResult MarkComplete([FromRoute]string id, ToDo selected)
    {
        selected = context.Todos.Find(selected.Id)!;

        if(selected != null)
        {
            selected.StatusId = "closed";
            context.SaveChanges();
        }
        return RedirectToAction("Index", new { ID = id });
    }

    [HttpPost]
    public IActionResult DeleteComplete(string id)
    {
        var toDelete = context.Todos.Where(t => t.StatusId == "closed").ToList();

        foreach (var task in toDelete)
        {
            context.Todos.Remove(task);
        }
        context.SaveChanges();

        return RedirectToAction("Index", new { ID = id });
    }
}
