using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();


app.MapGet("/", () => "Hello World!");

var todos = app.MapGroup("/todos");

todos.MapGet("/", GetAllTodos);
todos.MapGet("/complete", GetCompleteTodos);
todos.MapGet("/{id}", GetTodo);
todos.MapPost("/", CreateTodo);
todos.MapPut("/{id}", UpdateTodo);
todos.MapDelete("/{id}", DeleteTodo);

app.Run();

static async Task<IResult> GetAllTodos(TodoDb db){
    return TypedResults.Ok(await db.Todos.Select(todo => new TodoDTO(todo)).ToArrayAsync());
}


static async Task<IResult> GetCompleteTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(todo => new TodoDTO(todo)).ToListAsync());
}

static async Task<IResult> GetTodo(int id, TodoDb db)
{
    return await db.Todos.FindAsync(id)
        is Todo todo
            ? TypedResults.Ok(new TodoDTO(todo))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo(TodoDTO todoDTO, TodoDb db)
{
    var todo = new Todo {
        Name = todoDTO.Name,
        IsComplete = todoDTO.IsComplete
    };

    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/todos/{todo.Id}", todo);
}

static async Task<IResult> UpdateTodo(int id, Todo inputTodo, TodoDb db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return TypedResults.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, TodoDb db)
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();

        TodoDTO todoDTO = new TodoDTO(todo);
        
        return TypedResults.Ok(todoDTO);
    }

    return TypedResults.NotFound();
}