using Core.Entities.Identity;
using Core.Helpers.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SpecificationsQueries.Identity
{
    public class Speci_UserPagination : BaseSpecification<AppUser>
    {
        public Speci_UserPagination(UserPaginationParam param, bool emptyCtor = false, string superAdminId = null) :
            base(x =>
            ((string.IsNullOrEmpty(param.Search) || param.Search.Length < 3) || 
                (x.UserName.ToLower().Contains(param.Search) || x.Email.ToLower().Contains(param.Search) || x.PhoneNumber.Trim().Contains(param.Search))) &&
             x.EmailConfirmed == param.EmailConfirmed &&
            /* if superAdmin null: curent user is Admin, for that hidden the SuperAdmin data, else displey all users with SuperAdmin*/
            (string.IsNullOrEmpty(superAdminId) || x.Id != superAdminId))
        {
            // if was emptyCtor == true then we need empty constructor
            if (!emptyCtor)
            {
                // At the beginning of the sorting, the blogs are placed to remain at the top and then sorted by date of issue
                switch (param.Sort)
                {
                    case "UserNameDesc":
                        AddOrderByDescending(x => x.UserName);
                        break;
                    case "UserNameAsc":
                        AddOrderBy(x => x.UserName);
                        break;
                    case "EmailDesc":
                        AddOrderByDescending(x => x.Email);
                        break;
                    case "EmailAsc":
                        AddOrderBy(x => x.Email);
                        break;
                    case "CreatedDateDesc":
                        AddOrderByDescending(x => x.CreatedDate);
                        break;
                    default: // CreatedDateAsc
                        AddOrderBy(x => x.CreatedDate);
                        break;
                }
                /*  
                .Skip() and .Take() It must be at the end in order for pages to be created after filtering and searching
                how many do we want to Skip:
                minus one here because we want start from 0, PageSize=5 (PageIndex=1 - 1)=0
                5x0=0 this is start page    */
                ApplyPaging(param.PageSize * (param.PageIndex - 1), param.PageSize);
            }

        }
    }
}
