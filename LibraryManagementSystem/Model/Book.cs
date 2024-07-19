using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Model
{
    internal class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishedDate { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public int LibraryBranchId { get; set; } 
        public LibraryBranch LibraryBranch { get; set; }
        public ICollection<BorrowRecord> BorrowRecords { get; set; }
        public ICollection<Genre> Genres { get; set; }
    }
}
