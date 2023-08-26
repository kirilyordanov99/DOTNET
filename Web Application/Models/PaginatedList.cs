using System;
using System.Collections.Generic;
using System.Linq;

namespace Web_Application.Models
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int TotalItems { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; }

        public PaginatedList(List<T> items, int totalItems, int pageNumber, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        }
    }
}
