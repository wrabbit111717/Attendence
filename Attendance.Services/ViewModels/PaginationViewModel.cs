using System;
using System.Collections.Generic;

namespace Attendance.Services.ViewModels
{
    public class PaginationViewModel<T>
    {
        public int? PageNumber { get; set; } = null;
        public int? PageSize { get; set; } = null;
        public long? TotalPages { get; private set; } = null;
        private long? _totalRecords = null;
        public long? TotalRecords
        {
            get { return _totalRecords; }
            set
            {
                _totalRecords = value;
                if (PageNumber == null) return;
                TotalPages = Convert.ToInt64(Math.Ceiling((double)value / (int)PageSize));
                if (PageNumber > TotalPages)
                {
                    PageNumber = (int)TotalPages;
                }
            }
        }
        public List<T> Data { get; set; } = new List<T>();
        public PaginationViewModel(int? pageNumber = null, int? pageSize = 50)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
