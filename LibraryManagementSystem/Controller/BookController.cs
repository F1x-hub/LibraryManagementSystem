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
    internal class BookController
    {
        private readonly LibraryContext _context;

        public BookController(LibraryContext context)
        {
            _context = context;
        }

        public void ViewProfile(User user)
        {
            try
            {
                user = _context.Users
                       .Include(u => u.BorrowRecords)
                       .ThenInclude(br => br.Book)
                       .ThenInclude(b => b.Author)
                       .Include(u => u.Fines)
                       .Include(u => u.Membership)
                       .Include(u => u.Reservations)
                       .ThenInclude(r => r.Book)
                       .FirstOrDefault(u => u.Id == user.Id);

                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return;
                }

                Console.WriteLine("Profile Information:");
                Console.WriteLine($"Name: {user.Name}");
                Console.WriteLine($"Email: {user.Email}");
                Console.WriteLine($"Membership: {user.Membership?.Type ?? "Unknown"}");
                Console.WriteLine($"Fine: {user.Fines.Sum(f => f.Amount):C}");

                Console.WriteLine("Borrowed Books:");
                foreach (var record in user.BorrowRecords)
                {
                    var dueDate = record.BorrowDate.AddDays(14);
                    Console.WriteLine($"Title: {record.Book.Title}, Author: {record.Book.Author.Name}, Due Date: {dueDate.ToShortDateString()}");
                }

                Console.WriteLine("Reserved Books:");
                foreach (var reservation in user.Reservations)
                {
                    Console.WriteLine($"Title: {reservation.Book.Title}, Reserved On: {reservation.ReservationDate.ToShortDateString()}");
                }

                var fines = _context.Fines.Where(f => f.UserId == user.Id).ToList();
                Console.WriteLine("Fines:");
                foreach (var fine in fines)
                {
                    Console.WriteLine($"Fine ID: {fine.FineId}, Amount: {fine.Amount}, Issued: {fine.IssuedDate}, Paid: {fine.PaidDate}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while viewing the profile: {ex.Message}");
            }
        }

        public void RentBook(User user, int bookId)
        {
            try
            {
                var book = _context.Books.Find(bookId);

                if (book != null)
                {
                    int borrowDays = user.Membership.Type == "Premium" ? 28 : 14;

                    var borrowRecord = new BorrowRecord
                    {
                        BookId = bookId,
                        UserId = user.Id,
                        BorrowDate = DateTime.Now,
                        ReturnDate = null
                    };

                    _context.BorrowRecords.Add(borrowRecord);
                    _context.SaveChanges();

                    Console.WriteLine($"Book '{book.Title}' rented successfully for {borrowDays} days.");
                }
                else
                {
                    Console.WriteLine("Book is not available for rent.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while renting the book: {ex.Message}");
            }
        }

        public void ReturnBook(int bookId, int userId)
        {
            try
            {
                var borrowRecord = _context.BorrowRecords
                    .Include(br => br.Book)
                    .Include(br => br.User)
                    .FirstOrDefault(br => br.BookId == bookId && br.UserId == userId && br.ReturnDate == null);

                if (borrowRecord != null)
                {
                    borrowRecord.ReturnDate = DateTime.Now;

                    _context.BorrowRecords.Remove(borrowRecord);
                    _context.SaveChanges();

                    Console.WriteLine($"Book '{borrowRecord.Book.Title}' returned successfully.");
                }
                else
                {
                    Console.WriteLine("No active borrowing record found for this book and user.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while returning the book: {ex.Message}");
            }
        }

        public void CalculateFines(User user)
        {
            try
            {
                var borrowRecords = _context.BorrowRecords
                    .Where(br => br.UserId == user.Id && br.ReturnDate == null)
                    .ToList();

                foreach (var record in borrowRecords)
                {
                    int borrowDays = user.Membership.Type == "Premium" ? 28 : 14;
                    var dueDate = record.BorrowDate.AddDays(borrowDays);

                    if (DateTime.Now > dueDate)
                    {
                        decimal fineAmount = user.Membership.Type == "Premium" ? 0.5m : 1m;

                        var fine = new Fine
                        {
                            UserId = user.Id,
                            Amount = fineAmount,
                            IssuedDate = DateTime.Now
                        };

                        _context.Fines.Add(fine);
                    }
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while calculating fines: {ex.Message}");
            }
        }

        public void ReserveBook(User user, int bookId)
        {
            try
            {
                var book = _context.Books.Find(bookId);
                if (book == null)
                {
                    Console.WriteLine("Book not found.");
                    return;
                }

                if (user.Reservations == null)
                {
                    user.Reservations = new List<Reservation>();
                }

                var reservation = new Reservation
                {
                    BookId = bookId,
                    UserId = user.Id,
                    ReservationDate = DateTime.Now,
                    CancellationDate = null,
                    IsFulfilled = false
                };

                user.Reservations.Add(reservation);
                _context.Reservations.Add(reservation);
                _context.SaveChanges();

                Console.WriteLine("Book reserved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reserving the book: {ex.Message}");
            }
        }

        public void CancelReservation(User user, string bookTitle)
        {
            try
            {
                var reservation = _context.Reservations
                                  .Include(r => r.Book)
                                  .Include(r => r.User)
                                  .FirstOrDefault(r => r.Book.Title == bookTitle && r.UserId == user.Id && r.CancellationDate == null);

                if (reservation == null)
                {
                    Console.WriteLine("Reservation not found.");
                    return;
                }

                reservation.CancellationDate = DateTime.Now;
                _context.Reservations.Remove(reservation);
                _context.SaveChanges();

                Console.WriteLine("Reservation canceled successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while canceling the reservation: {ex.Message}");
            }
        }

        public void CheckAndApplyFines()
        {
            try
            {
                var overdueReservations = _context.Reservations
                    .Include(r => r.User)
                    .Where(r => r.CancellationDate == null && r.ReservationDate.AddDays(3) < DateTime.Now)
                    .ToList();

                foreach (var reservation in overdueReservations)
                {
                    reservation.CancellationDate = DateTime.Now;

                    var fine = new Fine
                    {
                        UserId = reservation.UserId,
                        Amount = 1,
                        IssuedDate = DateTime.Now,
                        PaidDate = null
                    };

                    _context.Fines.Add(fine);
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while checking and applying fines: {ex.Message}");
            }
        }
    }
}
