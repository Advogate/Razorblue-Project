using System;

namespace FizzBuzz
{
    class Program
    {
        public static void Main()
        {
            for (int i = 1; i <= 100; i++)
            {
                //concatenates the strings
                string output = "";

                //performs three separate checks for each number
                if (i % 3 == 0)
                {
                    output += "Fizz";
                }
                if (i % 5 == 0)
                {
                    output += "Buzz";
                }
                if (output == "")
                {
                    output = i.ToString();
                }
                //use a string variable to store the output
                Console.WriteLine(output);
            }
        }

    }
}
