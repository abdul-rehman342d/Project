using System.Collections.Generic;

namespace Common.Models
{
    public class Pagination<T> where T : class
    {
        public List<T> Items { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }
        public int TotalPages { get; set; }
        public int ItemCount { get
            {
                if(Items == null)
                {
                    return 0;
                }

                return Items.Count;
            }
        }
    }
}
