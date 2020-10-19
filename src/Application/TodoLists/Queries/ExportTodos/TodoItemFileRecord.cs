using Electricity.Application.Common.Mappings;
using Electricity.Domain.Entities;

namespace Electricity.Application.TodoLists.Queries.ExportTodos
{
    public class TodoItemRecord : IMapFrom<TodoItem>
    {
        public string Title { get; set; }

        public bool Done { get; set; }
    }
}
