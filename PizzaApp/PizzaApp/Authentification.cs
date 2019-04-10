using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaApp.Models;
using PizzaApp.DataAcces;

namespace PizzaApp
{
    public class Authentification
    {
        private UsersTableDataService _userTable;

        public User AuthUser()
        {
            bool isCorrect = false;
            User user;

            do
            {
                string phone = AuthPhone();
                string password = AuthPassword();

                _userTable = new UsersTableDataService();
                var users = _userTable.GetAll();

                phone = phone.Insert(0, "+");

                user = new User()
                {
                    Phone = phone,
                    Password = password
                };

                if (IsUser(user, users))
                {
                    user = users.Find(findUser => findUser.Phone == phone);
                    isCorrect = true;

                    Console.WriteLine("\n\nАвторизация прошла успешно!");

                    return user;
                }
                else
                {
                    isCorrect = false;

                    Console.WriteLine("\nНеверный логин или пароль.");
                    Console.ReadLine();
                    Console.Clear();
                }
            } while (!isCorrect);

            return user;
        }

        private string AuthPhone()
        {
            bool isRightNumber = false;
            string phoneNumber = "";

            do
            {
                Console.Write("Номер телефона: +");
                phoneNumber = Console.ReadLine();

                char[] charTelephoneNumber = phoneNumber.ToCharArray();

                for (int i = 0; i < charTelephoneNumber.Length; i++)
                {
                    if (!char.IsDigit(charTelephoneNumber[i]) || phoneNumber.Length != 11) //правильный номер телефона состоит из 11 цифр
                    {
                        isRightNumber = false;
                        Console.WriteLine("Ошибка: неправильный формат номера телефона.");
                        break;
                    }
                    else
                        isRightNumber = true;
                }
            }
            while (!isRightNumber);

            return phoneNumber;
        }

        private string AuthPassword()
        {
            string password = "";
            Console.Write("Пароль: ");

            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }

            } while (true);

            return password;
        }

        private bool IsUser(User user, List<User> users)
        {
            foreach (User tempUser in users)
                if ((user.Phone == tempUser.Phone) && (user.Password == tempUser.Password))
                    return true;

            return false;
        }
    }
}
