using Reddit.Models;
using System.Linq.Expressions;

namespace Reddit.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _context;


        public UsersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<User>> GetUsers(int pageNumber, int pageSize, string? searchKey, string? sortKey = null, bool? isAscending = null)
        {
            var users = _context.Users.AsQueryable();

            if (searchKey != null)
            {
                users = users.Where(u => u.Name.Contains(searchKey) || u.Email.Contains(searchKey) );
            }

            if (isAscending != null) { 
                if (isAscending == true)
                {
                    users = users.OrderBy(GetSortExpression(sortKey));
                }
                else
                {
                    users = users.OrderByDescending(GetSortExpression(sortKey));
                }

            }
            return await PagedList<User>.CreateAsync(users, pageNumber, pageSize);
        }

        private Expression<Func<User, object>> GetSortExpression(string? sortKey)
        {
            sortKey = sortKey?.ToLower();
            return sortKey switch
            {
               "numberofpages"=> user => user.Posts.Count,
                _ => user => user.Id
            };
        }
    }
}
