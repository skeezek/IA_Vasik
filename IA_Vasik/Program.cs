using System;
using System.Collections.Generic;
using System.Linq;

namespace IA_Vasik
{
    class Program
    {
        const int MaxWords = 1000; // Максимальна кількість слів

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Встановлюємо кодування консолі на UTF-8 для підтримки кирилиці
            Console.WriteLine("Введiть кількість слів:"); // Просимо користувача ввести кількість слів

            int n = int.Parse(Console.ReadLine()); // Зчитуємо кількість слів з консолі

            string[] words = new string[n]; // Масив для зберігання слів
            int[] categories = new int[n]; // Масив для зберігання категорій слів

            // Введення слів та категорій
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine("Введiть слово:"); // Просимо користувача ввести слово
                words[i] = Console.ReadLine().ToLower(); // Зчитуємо слово з консолі та переводимо його до нижнього регістру
                Console.WriteLine("Що це?\n 1 - Нерухомість,\n 2 - фінанси,\n 3 або iншi символи - енергетика:"); // Просимо користувача ввести категорію слова
                int.TryParse(Console.ReadLine(), out categories[i]); // Зчитуємо категорію з консолі
                if (categories[i] != 1 && categories[i] != 2)
                {
                    categories[i] = 3; // Якщо категорія не відповідає 1 або 2, встановлюємо 3 - енергетика
                }
            }

            // Додавання унікальних слів для перевірки до словника слів та категорій
            List<string> uniqueWords = words.Distinct().ToList();
            foreach (var word in uniqueWords)
            {
                if (!words.Contains(word))
                {
                    words = words.Append(word).ToArray(); // Додаємо унікальне слово до масиву слів
                    categories = categories.Append(0).ToArray(); // 0 означає, що слово ще не класифіковане
                }
            }

            // Розподілення слів за категоріями
            ClusterWords(words, categories, n);

            string input;
            do
            {
                Console.WriteLine("\nВведiть слово для перевiрки:"); // Просимо користувача ввести слово для перевірки
                input = Console.ReadLine().ToLower(); // Зчитуємо слово з консолі та переводимо його до нижнього регістру

                double real_estate_score = 0, finance_score = 0, energy_score = 0; // Змінні для збереження ваг кожної категорії

                // Розрахунок ваги для кожної категорії
                CalculateCategoryScores(input, words, categories, ref real_estate_score, ref finance_score, ref energy_score);

                Console.WriteLine("\nРезультат кластеризацii:"); // Виведення результатів кластеризації
                if (real_estate_score > finance_score && real_estate_score > energy_score)
                {
                    Console.WriteLine("Це Нерухомість."); // Виведення категорії "Нерухомість"
                }
                else if (finance_score > real_estate_score && finance_score > energy_score)
                {
                    Console.WriteLine("Це фінанси."); // Виведення категорії "фінанси"
                }
                else if (energy_score > real_estate_score && energy_score > finance_score)
                {
                    Console.WriteLine("Це енергетика."); // Виведення категорії "енергетика"
                }
                else
                {
                    Console.WriteLine("Вiдповiднiсть не знайдено."); // Виведення повідомлення, якщо категорія не знайдена
                }

                Console.WriteLine("\nНатиснiть будь-яку клавiшу для продовження або '1' для виходу."); // Просимо користувача ввести 1 для виходу
            } while (Console.ReadKey().Key != ConsoleKey.D1); // Повторюємо цикл, поки користувач не натисне клавішу 1
        }

        // Розподілення слів за категоріями
        static void ClusterWords(string[] words, int[] categories, int n)
        {
            Dictionary<string, int> real_estate = new Dictionary<string, int>(); // Словник для зберігання слів категорії "Нерухомість"
            Dictionary<string, int> finance = new Dictionary<string, int>(); // Словник для зберігання слів категорії "фінанси"
            Dictionary<string, int> energy = new Dictionary<string, int>(); // Словник для зберігання слів категорії "енергетика"

            for (int i = 0; i < n; i++)
            {
                string[] wordSplit = words[i].Split(new char[] { ' ', ',', '.', ':', '\t', '-' }, StringSplitOptions.RemoveEmptyEntries); // Розділяємо слова на окремі слова
                foreach (var word in wordSplit)
                {
                    string lowerWord = word.ToLower(); // Переводимо слово до нижнього регістру
                    int category = categories[i]; // Категорія слова

                    if (category == 1)
                    {
                        if (real_estate.ContainsKey(lowerWord))
                            real_estate[lowerWord]++; // Якщо слово вже існує, збільшуємо лічильник
                        else
                            real_estate[lowerWord] = 1; // Додаємо слово до словника
                    }
                    else if (category == 2)
                    {
                        if (finance.ContainsKey(lowerWord))
                            finance[lowerWord]++; // Якщо слово вже існує, збільшуємо лічильник
                        else
                            finance[lowerWord] = 1; // Додаємо слово до словника
                    }
                    else
                    {
                        if (energy.ContainsKey(lowerWord))
                            energy[lowerWord]++; // Якщо слово вже існує, збільшуємо лічильник
                        else
                            energy[lowerWord] = 1; // Додаємо слово до словника
                    }
                }
            }

            Console.WriteLine("\nКiлькість унiкальних слiв: " + (real_estate.Count + finance.Count + energy.Count)); // Виведення загальної кількості унікальних слів
            foreach (var kvp in real_estate.Concat(finance).Concat(energy)) // Перебір усіх слів та їх категорій
            {
                Console.WriteLine($"{kvp.Key} - Нерухомість: {real_estate.GetValueOrDefault(kvp.Key, 0)}, фінанси: {finance.GetValueOrDefault(kvp.Key, 0)}, енергетика: {energy.GetValueOrDefault(kvp.Key, 0)}"); // Виведення слова та його кількості в кожній категорії
            }
        }

        // Розрахунок ваги для кожної категорії
        static void CalculateCategoryScores(string input, string[] words, int[] categories, ref double real_estate_score, ref double finance_score, ref double energy_score)
        {
            int total_real_estate_score = words.Count(w => categories[Array.IndexOf(words, w)] == 1); // Загальна кількість слів категорії "Нерухомість"
            int total_finance_score = words.Count(w => categories[Array.IndexOf(words, w)] == 2); // Загальна кількість слів категорії "фінанси"
            int total_energy_score = words.Count(w => categories[Array.IndexOf(words, w)] == 3); // Загальна кількість слів категорії "енергетика"

            foreach (var word in input.Split(new char[] { ' ', ',', '.', ':', '\t', '-' }, StringSplitOptions.RemoveEmptyEntries)) // Перебір слова
            {
                string lowerWord = word.ToLower(); // Переводимо слово до нижнього регістру
                int index = Array.IndexOf(words, lowerWord); // Індекс слова у масиві слів
                if (index != -1)
                {
                    int category = categories[index]; // Категорія слова
                    if (category == 1)
                        real_estate_score += (double)total_real_estate_score / words.Length; // Додаємо вагу категорії "Нерухомість"
                    else if (category == 2)
                        finance_score += (double)total_finance_score / words.Length; // Додаємо вагу категорії "фінанси"
                    else if (category == 3)
                        energy_score += (double)total_energy_score / words.Length; // Додаємо вагу категорії "енергетика"
                }
            }
        }
    }
}
