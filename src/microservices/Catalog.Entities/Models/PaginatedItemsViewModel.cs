using System;
namespace Catalog.Entities.Models
{
	public class PaginatedItemsViewModel<T>(int pageIndex, int pageSize, long totalItems, List<T> itemsOnPage)
	{
		public List<T> ItemsOnPage { get; set; } = itemsOnPage;
		public int PageIndex { get; set; } = pageIndex;
		public int PageSize { get; set; } = pageSize;
		public long TotalItems { get; set; } = totalItems;		
	}
}

