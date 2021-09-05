using Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers.Identity
{
    public class UserPaginationParam : PaginationParam
    {
        public UserPaginationParam()
        {
        }
        public UserPaginationParam(int pageIndex, int pageSize) : base(pageIndex, pageSize)
        {
        }

        // to Select Users By Role
        public string Role { get; set; }
        public bool EmailConfirmed { get; set; } = true;
    }
}
