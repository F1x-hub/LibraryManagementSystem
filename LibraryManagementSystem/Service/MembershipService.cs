using LibraryManagementSystem.DataContext;
using LibraryManagementSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Service
{
    internal class MembershipService
    {
        private readonly LibraryContext _context;

        public MembershipService(LibraryContext context)
        {
            _context = context;
        }

        public void EnsureDefaultMemberships()
        {
            if (!_context.Memberships.Any(m => m.Type == "Basic"))
            {
                _context.Memberships.Add(new Membership
                {
                    Type = "Basic",
                    Fee = 0.0m 
                });
            }

            if (!_context.Memberships.Any(m => m.Type == "Premium"))
            {
                _context.Memberships.Add(new Membership
                {
                    Type = "Premium",
                    Fee = 50.0m 
                });
            }

            _context.SaveChanges();
        }
    }
}
