using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class PaginationParam
    {
        public PaginationParam()
        {
            this.PageIndex = 1;
            this.PageSize = MaxPageSize;
        }
        public PaginationParam(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex < 1 ? 1 : pageIndex;
            this.PageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize;
        }

        private const int MaxPageSize = 50;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int PagesCount { get; set; }
        public string CurrentLanguage { get; set; }
        public string Sort { get; set; }
        private string _search;

        public string Search
        {
            get => _search;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _search = value.ToLower();
                };
            }
        }
    }
}
