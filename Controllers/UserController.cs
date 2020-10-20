using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

[Route("v1/users")]
public class UserController : ControllerBase
{
    [HttpGet]
    [Route("")]
    [Authorize(Roles="manager")]
    public async Task<ActionResult<List<User>>> Get(
        [FromServices] DataContext context)
    {
        var users = await context
            .Users
            .AsNoTracking()
            .ToListAsync();
        return Ok(users);
    }

    [HttpPost]
    [Route("")]
    [AllowAnonymous]
    public async Task<ActionResult<User>> Post(
        [FromBody]User model,
        [FromServices] DataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            model.Role = "employee";

            context.Users.Add(model);
            await context.SaveChangesAsync();

            model.Password = "";
            return Ok(model);  
        }
        catch 
        {
            return BadRequest(new { message = "Não foi possível criar o Usuário" });
        }  
    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles="manager")]
    public async Task<ActionResult<Product>> Put(
        int id, 
        [FromBody]Product model,
        [FromServices] DataContext context)
    {
        if (id != model.Id)
            return NotFound(new { message = "Usuário não encontrado" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Entry(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new { message = "Este registro já foi atualizado" });
        }
        catch(Exception)
        {
            return BadRequest(new { message = "Não foi possível atualizar o usuário" });
        }
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<dynamic>> Authenticate(
        [FromBody]User model,
        [FromServices] DataContext context)
    {
        try
        {
            var user = await context.Users
                .AsNoTracking()
                .Where(x => x.UserName == model.UserName && x.Password == model.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);

            user.Password = "";
            return Ok(new {
                user = user,
                token = token
            });
        }
        catch 
        {
            return BadRequest(new { message = "Não foi autenticar o Usuário" });
        }  
    }
}