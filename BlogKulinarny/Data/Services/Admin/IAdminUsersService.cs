using BlogKulinarny.Data.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BlogKulinarny.Data.Services.Admin
{
    public interface IAdminUsersService
    {
        public Task<ChangesResult> ConfirmUser(int UserId);

        public Task<ChangesResult> DeleteUser(int UserId);
    }
}
