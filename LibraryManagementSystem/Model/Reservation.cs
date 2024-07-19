using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Model
{
    internal class Reservation
    {
        public int ReservationId { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public bool IsFulfilled { get; set; }
    }
}
