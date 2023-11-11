using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vysion.Helpers
{
    public class PaginationParams
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
