using System.Data;

namespace BLAInterview.Domain.Tasks;

public static class TaskRepository
{
    private static readonly object Gate = new();
    private static readonly DataTable Tasks = CreateTasksTable();

    public static Task Create(string title, string ownerId)
    {
        var task = Task.Create(title, ownerId);

        lock (Gate)
        {
            var row = Tasks.NewRow();
            row["Id"] = task.Id;
            row["Title"] = task.Title;
            row["OwnerId"] = task.OwnerId;
            row["Created"] = task.Created;
            Tasks.Rows.Add(row);
        }

        return task;
    }

    private static DataTable CreateTasksTable()
    {
        var table = new DataTable("Tasks");
        table.Columns.Add("Id", typeof(Guid));
        table.Columns.Add("Title", typeof(string));
        table.Columns.Add("OwnerId", typeof(string));
        table.Columns.Add("Created", typeof(DateTimeOffset));

        return table;
    }
}
