namespace BlogKulinarny.Data.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;

        public AuthService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Login(string emailOrLogin, string password)
        {
            var user = _dbContext.users.FirstOrDefault(u =>
                (u.mail == emailOrLogin || u.login == emailOrLogin) && u.password == password);

            if (user != null)
            {
                // Utwórz sesję użytkownika lub zapisz informacje o zalogowanym użytkowniku w sesji
                // Na przykład:
                // HttpContext.Session.SetString("UserId", user.Id.ToString());
                return true;
            }

            return false;
        }
    }
}
