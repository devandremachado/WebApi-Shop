using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

[Route("v1")]
public class HomeController : ControllerBase
{
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<List<dynamic>>> Get(
        [FromServices] DataContext context)
    {
        var employee = new User {Id = 1, UserName = "robin", Role = "employee"};
        var manager = new User {Id = 2, UserName = "batman", Role = "manager"};
        var category = new Category {Id = 1, Title = "Informática"};
        var product = new Product {Id = 1, Category = category, Title = "Mouse", Price = 299, Description = "Mouse Gamer"};

        context.Users.Add(employee);
        context.Users.Add(manager);
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        return Ok(new {message = "Dados iniciais configurados"});
    }
}