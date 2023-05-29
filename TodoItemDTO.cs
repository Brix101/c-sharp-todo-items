public class TodoDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }

    public TodoDTO() {
        IsComplete = false;
     }
    public TodoDTO(Todo todo) =>
    (Id, Name, IsComplete) = (todo.Id, todo.Name, todo.IsComplete);
}