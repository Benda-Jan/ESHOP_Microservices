using System;
namespace Catalog.Entities.Models
{
	public class PaginatedItemsViewModel<T>
	{
		public List<T> ItemsOnPage { get; set; } = new List<T>();
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
		public long TotalItems { get; set; }

        public PaginatedItemsViewModel(int pageIndex, int pageSize, long totalItems, List<T> itemsOnPage)
		{
			PageIndex = pageIndex;
			PageSize = pageSize;
			TotalItems = totalItems;
			ItemsOnPage = itemsOnPage;
		}
		
	}
}

