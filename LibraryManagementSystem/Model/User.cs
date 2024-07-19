using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Model
{
    internal class User
    {
        public User()
        {
            BorrowRecords = new List<BorrowRecord>();
            Fines = new List<Fine>();
        }

        public int Id { get; set; } 
        public string Name { get; set; }
        public string Email { get; set; }

        [NotMapped]
        public string Password { get; set; }

        public string PasswordHash { get; set; }

        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
        public int MembershipId { get; set; }
        public Membership Membership { get; set; }
        public ICollection<Fine> Fines { get; set; } = new List<Fine>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
