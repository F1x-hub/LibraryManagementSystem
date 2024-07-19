using LibraryManagementSystem.DataContext;
using LibraryManagementSystem.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controller
{
    internal class AdminController
    {
        private readonly LibraryContext _context;

        public AdminController(LibraryContext context)
        {
            _context = context;
        }

        public void AddBook(Book book)
        {
            try
            {
                _context.Books.Add(book);
                _context.SaveChanges();
                Console.WriteLine("Book added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the book: {ex.Message}");
            }
        }

        public Book GetBookById(int bookId)
        {
            try
            {
                return _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.LibraryBranch)
                    .Include(b => b.BorrowRecords)
                    .Include(b => b.Genres)
                    .FirstOrDefault(b => b.BookId == bookId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the book: {ex.Message}");
                return null;
            }
        }

        public List<Book> GetAllBooks()
        {
            try
            {
                return _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.LibraryBranch)
                    .Include(b => b.BorrowRecords)
                    .Include(b => b.Genres)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving books: {ex.Message}");
                return new List<Book>();
            }
        }

        public void UpdateBook(Book book, List<string> newGenres)
        {
            try
            {
                
                var existingBook = _context.Books
                    .Include(b => b.Genres)
                    .FirstOrDefault(b => b.BookId == book.BookId);

                if (existingBook == null)
                {
                    Console.WriteLine("Book not found.");
                    return;
                }

                
                existingBook.Title = book.Title;
                existingBook.ISBN = book.ISBN;
                existingBook.PublishedDate = book.PublishedDate;

                
                if (book.Author != null)
                {
                    var existingAuthor = _context.Authors.Find(book.Author.AuthorId);
                    if (existingAuthor != null)
                    {
                        existingBook.Author = existingAuthor;
                    }
                    else
                    {
                        existingBook.Author = new Author { Name = book.Author.Name };
                        _context.Authors.Add(existingBook.Author);
                    }
                }

                
                if (book.LibraryBranch != null)
                {
                    var existingBranch = _context.LibraryBranches.Find(book.LibraryBranch.LibraryBranchId);
                    if (existingBranch != null)
                    {
                        existingBook.LibraryBranch = existingBranch;
                    }
                    else
                    {
                        existingBook.LibraryBranch = new LibraryBranch { Name = book.LibraryBranch.Name, Address = book.LibraryBranch.Address };
                        _context.LibraryBranches.Add(existingBook.LibraryBranch);
                    }
                }

                
                existingBook.Genres.Clear();
                _context.SaveChanges();

                
                foreach (var genreName in newGenres)
                {
                    var genre = _context.Genres.FirstOrDefault(g => g.Name == genreName);
                    if (genre == null)
                    {
                        genre = new Genre { Name = genreName };
                        _context.Genres.Add(genre);
                        _context.SaveChanges();
                    }
                    existingBook.Genres.Add(genre);
                }

                
                _context.SaveChanges();
                Console.WriteLine("Book updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating the book: {ex.Message}");
            }
        }

        public void DeleteBook(int bookId)
        {
            try
            {
                var book = GetBookById(bookId);
                if (book != null)
                {
                    _context.Books.Remove(book);
                    _context.SaveChanges();
                    Console.WriteLine("Book deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Book not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the book: {ex.Message}");
            }
        }

        

        public void AddBook()
        {
            try
            {
                var book = new Book();

                Console.Write("Enter book title: ");
                book.Title = Console.ReadLine()!;

                Console.Write("Enter ISBN: ");
                book.ISBN = Console.ReadLine()!;

                Console.Write("Enter published date (yyyy-mm-dd): ");
                book.PublishedDate = DateTime.Parse(Console.ReadLine()!);

                Console.Write("Enter author name: ");
                var authorName = Console.ReadLine();
                book.Author = new Author { AuthorId = book.AuthorId, Name = authorName };

                Console.Write("Enter library branch name: ");
                var branchName = Console.ReadLine();

                Console.Write("Enter library branch address: ");
                var branchAddress = Console.ReadLine();
                book.LibraryBranch = new LibraryBranch { LibraryBranchId = book.LibraryBranchId, Name = branchName, Address = branchAddress };

                book.BorrowRecords = new List<BorrowRecord>();
                book.Genres = new List<Genre>();

                Console.WriteLine("Enter the number of genres:");
                int genreCount = int.Parse(Console.ReadLine()!);

                for (int i = 0; i < genreCount; i++)
                {
                    Console.WriteLine($"Enter genre name #{i + 1}:");
                    var genreName = Console.ReadLine();
                    book.Genres.Add(new Genre { Name = genreName });
                }

                AddBook(book);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the book: {ex.Message}");
            }
        }

        public void GetBookById()
        {
            try
            {
                GetAllBookss();

                Console.WriteLine("Enter book ID:");
                int bookId = int.Parse(Console.ReadLine()!);

                var book = GetBookById(bookId);
                if (book != null)
                {
                    Console.WriteLine($"Book ID: {book.BookId}");
                    Console.WriteLine($"Title: {book.Title}");
                    Console.WriteLine($"ISBN: {book.ISBN}");
                    Console.WriteLine($"Published Date: {book.PublishedDate.ToShortDateString()}");
                    Console.WriteLine($"Author: {book.Author.Name}");
                    Console.WriteLine($"Library Branch: {book.LibraryBranch.Name}, {book.LibraryBranch.Address}");

                    Console.Write("Genres: ");
                    var genres = book.Genres.ToList();
                    for (int i = 0; i < genres.Count; i++)
                    {
                        if (i < genres.Count - 1)
                        {
                            Console.Write($"{genres[i].Name}, ");
                        }
                        else
                        {
                            Console.Write($"{genres[i].Name}. ");
                        }
                    }

                    Console.WriteLine("\n");
                }
                else
                {
                    Console.WriteLine("Book not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the book: {ex.Message}");
            }
        }

        public void UpdateBook()
        {
            try
            {
                GetAllBookss();

                Console.WriteLine("Enter book ID to update:");
                int bookId = int.Parse(Console.ReadLine()!);

                var existingBook = GetBookById(bookId);
                if (existingBook != null)
                {
                    var book = new Book();

                    Console.Write("Enter new title: ");
                    book.Title = Console.ReadLine()!;

                    Console.Write("Enter new ISBN: ");
                    book.ISBN = Console.ReadLine()!;

                    Console.Write("Enter new published date (yyyy-mm-dd): ");
                    book.PublishedDate = DateTime.Parse(Console.ReadLine()!);

                    Console.Write("Enter new author name: ");
                    book.Author = new Author { AuthorId = existingBook.AuthorId, Name = Console.ReadLine()! };

                    Console.Write("Enter new library branch name: ");
                    var branchName = Console.ReadLine();
                    Console.Write("Enter new library branch address: ");
                    var branchAddress = Console.ReadLine();
                    book.LibraryBranch = new LibraryBranch { LibraryBranchId = existingBook.LibraryBranchId, Name = branchName, Address = branchAddress };

                    Console.WriteLine("Enter the number of new genres:");
                    int genreCount = int.Parse(Console.ReadLine()!);
                    var newGenres = new List<string>();

                    for (int i = 0; i < genreCount; i++)
                    {
                        Console.WriteLine($"Enter genre name #{i + 1}:");
                        newGenres.Add(Console.ReadLine()!);
                    }

                    book.BookId = bookId; 
                    UpdateBook(book, newGenres);
                }
                else
                {
                    Console.WriteLine("Book not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating the book: {ex.Message}");
            }
        }

        public void DeleteBook()
        {
            try
            {
                GetAllBookss();

                Console.WriteLine("Enter book ID to delete:");
                int bookId = int.Parse(Console.ReadLine()!);

                DeleteBook(bookId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the book: {ex.Message}");
            }
        }

        public void GetAllBookss()
        {
            try
            {
                var books = GetAllBooks();
                foreach (var book in books)
                {
                    Console.WriteLine($"Book ID :{book.BookId}, Title: {book.Title}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving all books: {ex.Message}");
            }
        }

        public void EditUserBooks()
        {
            try
            {
                Console.WriteLine("Enter User ID:");
                int userId = int.Parse(Console.ReadLine()!);

                var user = _context.Users
                    .Include(u => u.BorrowRecords)
                    .ThenInclude(br => br.Book)
                    .Include(u => u.Reservations)
                    .ThenInclude(r => r.Book)
                    .FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return;
                }

                Console.WriteLine("User's Borrowed Books:");
                foreach (var record in user.BorrowRecords)
                {
                    Console.WriteLine($"Book ID: {record.Book.BookId}, Title: {record.Book.Title}");
                }

                Console.WriteLine("User's Reserved Books:");
                foreach (var reservation in user.Reservations)
                {
                    Console.WriteLine($"Book ID: {reservation.Book.BookId}, Title: {reservation.Book.Title}");
                }

                Console.WriteLine("Enter Book ID to delete (borrowed or reserved):");
                int bookId = int.Parse(Console.ReadLine()!);

                var borrowRecord = user.BorrowRecords.FirstOrDefault(br => br.BookId == bookId);
                var reservationRecord = user.Reservations.FirstOrDefault(r => r.BookId == bookId);

                if (borrowRecord != null)
                {
                    _context.BorrowRecords.Remove(borrowRecord);
                    Console.WriteLine($"Borrowed book '{borrowRecord.Book.Title}' deleted successfully.");
                }
                else if (reservationRecord != null)
                {
                    _context.Reservations.Remove(reservationRecord);
                    Console.WriteLine($"Reserved book '{reservationRecord.Book.Title}' deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Book not found in user's borrowed or reserved books.");
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while editing user's books: {ex.Message}");
            }
        }

        public void ChangeUserMembershipType()
        {
            try
            {
                Console.WriteLine("Enter User ID:");
                int userId = int.Parse(Console.ReadLine()!);

                var user = _context.Users
                    .Include(u => u.Membership)
                    .FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return;
                }

                Console.WriteLine($"Current Membership Type: {user.Membership?.Type ?? "Unknown"}");
                Console.WriteLine("Enter new Membership Type (Basic/Premium):");
                string newType = Console.ReadLine()!;

                if (user.Membership == null)
                {
                    user.Membership = new Membership();
                    _context.Memberships.Add(user.Membership);
                }

                user.Membership.Type = newType;
                _context.SaveChanges();

                Console.WriteLine("Membership type updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while changing the user's membership type: {ex.Message}");
            }
        }

        
    }
}
