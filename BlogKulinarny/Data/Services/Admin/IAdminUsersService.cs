using BlogKulinarny.Data.Helpers;

namespace BlogKulinarny.Data.Services.Admin
{
    public interface IAdminUsersService
    {
        public Task<ChangesResult> ConfirmUser(int UserId);

        public Task<ChangesResult> DeleteUser(int UserId);
    }
}
