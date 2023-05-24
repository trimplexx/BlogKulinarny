using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;
using static BlogKulinarny.Data.Services.AuthService;

namespace BlogKulinarny.Data.Services.Admin
{
    public class AdminUsersService : IAdminUsersService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminUsersService(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ChangesResult> ConfirmUser(int userId)
        {
            try
            {
                User user = await _dbContext.users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    user.isAccepted = true;
                    _dbContext.users.Update(user);
                    await _dbContext.SaveChangesAsync(); // Zapisz zmiany w bazie danych
                    return new ChangesResult(true, "Pomyslnie zaakcetpowano uzytkownika");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ChangesResult(false, "wykryto wyjatek");
            }
            return new ChangesResult(false, "wystapil blad");
        }

        public async Task<ChangesResult> DeleteUser(int userId)
        {
            try
            {
                User user = await _dbContext.users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    _dbContext.users.Remove(user);
                    await _dbContext.SaveChangesAsync(); // Zapisz zmiany w bazie danych
                    return new ChangesResult(true, "Pomyslnie usunieto uzytkownika");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ChangesResult(false, "wykryto wyjatek");
            }
            return new ChangesResult(false, "wystapil blad");
            
        }
    }
}
