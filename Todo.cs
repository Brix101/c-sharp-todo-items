public class Todo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }

    public Todo()
    {
        IsComplete = false; // Set IsComplete to false by default
    }
}