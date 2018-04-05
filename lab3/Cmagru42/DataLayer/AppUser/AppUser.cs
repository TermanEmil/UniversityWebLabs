using Microsoft.AspNet.Identity;

namespace DataLayer.AppUser
{
    public class ApplicationUser
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        IPasswordHasher PasswordHasher { get; set; }
        public string Password { get; private set; }

        public void SetPassword(
            IPasswordHasher passwordHasher,
            string newPassword)
        {
            Password = passwordHasher.HashPassword(newPassword);
        }
    }
}
