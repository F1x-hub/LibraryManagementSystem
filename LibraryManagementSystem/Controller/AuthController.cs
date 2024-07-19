using LibraryManagementSystem.DataContext;
using LibraryManagementSystem.Model;
using LibraryManagementSystem.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controller
{
    internal class AuthController
    {
        
        private readonly LibraryContext _context;
        private readonly MembershipService _membershipService;

        public AuthController(LibraryContext context)
        {
            _context = context;
            _membershipService = new MembershipService(_context);
        }


        public bool RegisterUser()
        {
            try
            {
                _membershipService.EnsureDefaultMemberships();

                Console.Write("Enter Name: ");
                string name = Console.ReadLine()!;

                Console.Write("Enter email: ");
                string email = Console.ReadLine()!;

                Console.Write("Enter password: ");
                string password = Console.ReadLine()!;

                Console.Write("Enter membership type (Basic/Premium): ");
                string membershipType = Console.ReadLine()!.ToLower();

                var membership = _context.Memberships.FirstOrDefault(m => m.Type.ToLower() == membershipType);
                if (membership == null)
                {
                    Console.WriteLine("Invalid membership type. Please try again.");
                    return false;
                }

                User user = new User()
                {
                    Name = name,
                    Email = email,
                    Password = password,
                    MembershipId = membership.MembershipId
                };
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while registering the user: {ex.Message}");
                return false;
            }
        }

        public bool RegisterAdmin()
        {
            try
            {
                Console.Write("Enter Name: ");
                string name = Console.ReadLine()!;

                Console.Write("Enter email: ");
                string email = Console.ReadLine()!;

                Console.Write("Enter password: ");
                string password = Console.ReadLine()!;

                Admin admin = new Admin()
                {
                    Name = name,
                    Email = email,
                    Password = password
                };

                admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(admin.Password);

                _context.Admins.Add(admin);
                _context.SaveChanges();
                return admin != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while registering the admin: {ex.Message}");
                return false;
            }
        }

        private object Authorize(string email, string password, bool isAdmin)
        {
            try
            {
                if (isAdmin)
                {
                    var admin = _context.Admins.FirstOrDefault(a => a.Email == email);
                    if (admin == null) return "Admin not found";
                    bool isCorrectPassword = BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);
                    if (!isCorrectPassword) return "Incorrect password";
                    return admin;
                }
                else
                {
                    var user = _context.Users.FirstOrDefault(u => u.Email == email);
                    if (user == null) return "User not found";
                    bool isCorrectPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                    if (!isCorrectPassword) return "Incorrect password";
                    return user;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while authorizing: {ex.Message}");
                return "Authorization error";
            }
        }

        public object AuthorizeUser()
        {
            try
            {
                Console.Write("Enter Email: ");
                string email = Console.ReadLine()!;

                Console.Write("Enter password: ");
                string password = Console.ReadLine()!;

                return Authorize(email, password, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while authorizing the user: {ex.Message}");
                return "Authorization error";
            }
        }

        public object AuthorizeAdmin()
        {
            try
            {
                Console.Write("Enter Email: ");
                string email = Console.ReadLine()!;

                Console.Write("Enter password: ");
                string password = Console.ReadLine()!;

                return Authorize(email, password, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while authorizing the admin: {ex.Message}");
                return "Authorization error";
            }
        }


    }
}
