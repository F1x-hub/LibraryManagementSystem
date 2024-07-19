// See https://aka.ms/new-console-template for more information
using LibraryManagementSystem.Controller;
using LibraryManagementSystem.DataContext;
using LibraryManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

var context = new LibraryContext();
var authController = new AuthController(context);

var bookController = new BookController(context);
var adminController = new AdminController(context);

var menuController = new MenuController();

menuController.LoadingAnimation();
while (true)
{
    Console.WriteLine("Select an operation:");

    Console.WriteLine("\n-------------------\n");

    Console.WriteLine("1. Register a new user");
    Console.WriteLine("2. Register a new admin");
    Console.WriteLine("3. Login as user");
    Console.WriteLine("4. Login as admin");
    Console.WriteLine("5. Exit");

    var choice = Console.ReadLine();

    if (choice == "1")
    {
        menuController.LoadingAnimation();
        if (authController.RegisterUser())
            Console.WriteLine("User registered successfully.");
        else
            Console.WriteLine("User registration failed.");
    }
    if (choice == "2")
    {
        menuController.LoadingAnimation();
        if (authController.RegisterAdmin())
            Console.WriteLine("Admin registered successfully.");
        else
            Console.WriteLine("Admin registration failed.");
    }
    if (choice == "3")
    {
        var result = authController.AuthorizeUser();
        if (result is User user)
        {
            if (user != null)
            {
                menuController.LoadingAnimation();
                Console.WriteLine("Welcome, " + user.Name);
                UserMenu(bookController, user, adminController);
            }
        }
        else
            Console.WriteLine(result);
    }
    if (choice == "4")
    {
        var result = authController.AuthorizeAdmin();
        if (result is Admin admin)
        {
            menuController.LoadingAnimation();

            Console.WriteLine($"Admin {admin.Name} logged in successfully.");

            AdminMenu();
        }   
        else
            Console.WriteLine(result);
    }
    if (choice == "5")
    {
        break;
    }

    bookController.CheckAndApplyFines();
}


void AdminMenu()
{
    using (var context = new LibraryContext())
    {
        

        while (true)
        {
            Console.WriteLine("Select an operation:");

            Console.WriteLine("\n-------------------\n");

            Console.WriteLine("1. Add a new book");
            Console.WriteLine("2. Get a book by ID");
            Console.WriteLine("3. Get all books");
            Console.WriteLine("4. Update a book");
            Console.WriteLine("5. Delete a book");
            Console.WriteLine("6. Edit User's Books");
            Console.WriteLine("7. Change User's Membership Type");
            Console.WriteLine("8. Exit");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                menuController.LoadingAnimation();
                adminController.AddBook();
            }
            if (choice == "2")
            {
                menuController.LoadingAnimation();
                adminController.GetBookById();
            }
            if (choice == "3")
            {
                menuController.LoadingAnimation();
                adminController.GetAllBookss();
            }
            if (choice == "4")
            {
                menuController.LoadingAnimation();
                adminController.UpdateBook();
            }
            if (choice == "5")
            {
                menuController.LoadingAnimation();
                adminController.DeleteBook();
            }
            if (choice == "6")
            {
                menuController.LoadingAnimation();
                adminController.EditUserBooks();
            }
            else if (choice == "7")
            {
                menuController.LoadingAnimation();
                adminController.ChangeUserMembershipType();
            }
            if (choice == "8")
            {
                return;
            }
        }
    }
}

void UserMenu(BookController bookController, User user, AdminController adminController)
{
    while (true)
    {
        Console.WriteLine("Select an operation:");

        Console.WriteLine("\n-------------------\n");

        Console.WriteLine("1. Rent a book");
        Console.WriteLine("2. Reserve a book");
        Console.WriteLine("3. Cancel a reservation");
        Console.WriteLine("4. View profile");
        Console.WriteLine("5. Return a book");
        Console.WriteLine("6. Exit");

        var choice = Console.ReadLine();

        if (choice == "1")
        {
            menuController.LoadingAnimation();
            adminController.GetAllBookss();

            Console.WriteLine("\n-------------------\n");

            Console.Write("Enter book ID to rent: ");
            int bookId = int.Parse(Console.ReadLine()!);
            bookController.RentBook(user, bookId);
        }
        if (choice == "2")
        {
            menuController.LoadingAnimation();
            adminController.GetAllBookss();

            Console.WriteLine("Enter book ID to reserve:");
            int bookId = int.Parse(Console.ReadLine()!);
            bookController.ReserveBook(user, bookId);
        }
        if (choice == "3")
        {
            menuController.LoadingAnimation();
            Console.WriteLine("Enter the title of the book to cancel reservation:");
            string bookTitle = Console.ReadLine()!;
            bookController.CancelReservation(user, bookTitle);
        }
        if (choice == "4")
        {
            menuController.LoadingAnimation();
            bookController.ViewProfile(user);
        }
        if (choice == "5") 
        {
            menuController.LoadingAnimation();
            Console.WriteLine("Enter book ID to return:");
            int bookId = int.Parse(Console.ReadLine()!);
            bookController.ReturnBook(bookId, user.Id);
        }
        if (choice == "6")
        {
            return;
        }


        bookController.CalculateFines(user);
    }
}


