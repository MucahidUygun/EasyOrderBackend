using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistence.Paging;

public interface IPaginate<T>
{
    public IList<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public int TotalItems { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }
}
