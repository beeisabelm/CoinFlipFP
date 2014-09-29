using System;
using System.Linq;
using FunctionalProgramming.Monad;

namespace CoinFlipGame
{
    class Program
    {
        private static readonly int MinNr = new Random().Next(), MaxNr = new Random().Next(MinNr);
        private static readonly CoinFace[] TheFaces = { new CoinFace { ShortName = "t", Name = "Tail" }, new CoinFace { ShortName = "h", Name = "Head" } };
        class CoinFace
        {
            public string ShortName { get; set; }
            public string Name { get; set; }
        }

        static void Main(string[] args)
        {
            var toEnumerable = TheFaces.AsQueryable();
            while (!Convert(Console.ReadLine().ToMaybe()).Equals("q"))
            {
                var tossed = GetResult(Randomizer(MinNr, MaxNr));
                Console.WriteLine("Head (h) or Tail (t)?");
                var userInput = Console.ReadLine().ToMaybe();
                Console.WriteLine("Result: " + tossed);
                Console.WriteLine("Your Answer: " + userInput.ToString());
                //Console.WriteLine(from c in TheFaces.FirstOrDefault(x => x.Name == userInput.ToString()).ToMaybe() select c.Name);
                if (Compare(tossed, userInput))
                {
                    Console.WriteLine("You win.");
                }
                else
                {
                    Console.WriteLine("Nope!" + from t in toEnumerable.SingleOrDefault(x => x.ShortName == Convert(tossed)).ToMaybe() select t.Name);
                }
                Console.WriteLine("Press Enter to play again or q to Quit.");
            }
            Console.WriteLine("GoodBye");
        }

        static IMaybe<string> GetResult(int theNumber)
        {
            return theNumber % 2 == 0 ? from c in TheFaces.FirstOrDefault(x => x.ShortName == "h").ToMaybe() select c.ShortName 
                : from c in TheFaces.FirstOrDefault(x => x.ShortName == "t").ToMaybe() select c.ShortName;
        }

        static int Randomizer(int minNr, int maxNr)
        {
            return new Random().Next(new Random().Next(Math.Abs(maxNr - minNr) + 1));
        }

        static bool Compare(IMaybe<string> aVal, IMaybe<string> anotherVal)
        {
            return aVal.ToString().ToLower().Equals(anotherVal.ToString().ToLower());
        }

        static string Convert(IMaybe<string> aVal)
        {
            return aVal.ToString().Replace("Just(", "").Replace(")", "");
        }

    }
}
