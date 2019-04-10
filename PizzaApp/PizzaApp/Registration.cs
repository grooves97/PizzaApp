using System;
using PizzaApp.DataAcces;
using PizzaApp.Models;
using PizzaApp.Services.Abstract;

namespace PizzaApp
{
    public class Registration
    {
        public ISender Sender { get; set; }
        private UsersTableDataService _userTable;

        public User RegisterUser(ISender sender)
        {
            User user = new User();
            Sender = sender;
            _userTable = new UsersTableDataService();

            var users = _userTable.GetAll();
            bool isValid = false;

            do
            {
                user.Id = users.Count + 1;
                user.Name = InitName();
                user.Phone = $"+{InitPhoneNumber(sender)}";
                user.Password = InitPassword();

                foreach (var tempUser in users)
                {
                    if (user.Phone == tempUser.Phone)
                    {
                        Console.WriteLine("\n\nЭтот номер уже зарегистрирован. Используйте другой номер\n");

                        isValid = false;
                        break;
                    }
                    else
                    {
                        isValid = true;
                    }
                }

                if (isValid)
                {
                    _userTable.Add(user);

                    Console.WriteLine("\nРегистрация прошла успешна.");
                }

            } while (!isValid);

            return user;
        }

        private string InitName()
        {
            string name;
            bool isRightName = true;

            do
            {
                Console.Write("Введите ваше имя: ");
                name = Console.ReadLine();

                char[] charName = name.ToCharArray();

                for (int i = 0; i < charName.Length; i++)
                {
                    if (!char.IsLetter(charName[i]))
                    {
                        isRightName = false;
                        Console.WriteLine("Ошибка: имя должен состоять из букв.");
                        break;
                    }
                    else
                        isRightName = true;
                }
            }
            while (!isRightName);

            return name;
        }

        private string InitPhoneNumber(ISender sender)
        {
            bool isRightNumber = true;
            string phoneNumber = "";

            do
            {
                Console.Write("Введите ваш номер телефона: +");
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

            CheckPhoneNumber(phoneNumber, sender);

            return phoneNumber;
        }

        private void CheckPhoneNumber(string phoneNumber, ISender sender)
        {
            string rightSmsCode = sender.SendMessage(phoneNumber);
            string writtenSmsCode;

            Console.Write("Вам отправлен код подтверждения.\nВведите код: ");

            do
            {
                writtenSmsCode = Console.ReadLine();

                if (rightSmsCode != writtenSmsCode)
                {
                    Console.Write("Ошибка: неправильный код.\nВведите снова: ");
                }
            }
            while (rightSmsCode != writtenSmsCode);
        }

        private string InitPassword()
        {
            string password = "";
            Console.Write("Введите пароль: ");

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

            string tempPassword = "";

            Console.Write("\nПодтвердите ваш пароль: ");

            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    tempPassword += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && tempPassword.Length > 0)
                    {
                        tempPassword = tempPassword.Substring(0, (tempPassword.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        if (password != tempPassword)
                        {
                            Console.Write("\nОшибка: ваши пароли не соответствуют. Повторите снова.\nПодтвердите ваш пароль:");
                            tempPassword = string.Empty;
                        }
                        else
                            break;
                    }
                }
            }
            while (true);

            return password;
        }
    }
}
