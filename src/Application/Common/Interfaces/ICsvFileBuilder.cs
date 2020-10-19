using Electricity.Application.TodoLists.Queries.ExportTodos;
using System.Collections.Generic;

namespace Electricity.Application.Common.Interfaces
{
    public interface ICsvFileBuilder
    {
        byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
    }
}
