using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Model
{
    internal class Membership
    {
        public int MembershipId { get; set; }
        public string Type { get; set; }
        public decimal Fee { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
