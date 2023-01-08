using System;

namespace Anagram
{
    class Program
    {
        static void Main(string[] args)
        {
            Program pg = new Program();
            string string1 = Console.ReadLine();
            string string2 = Console.ReadLine();
            pg.AreStringsEqual(string1, string2);
            Console.Read();

        }
        public void AreStringsEqual(string str1, string str2)
        {
            if (str1.Equals(str2,StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("true");
            }
            else
            {
                Console.WriteLine("false");
            }
        }

    }
}
