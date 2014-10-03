using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Schema;
using FunctionalProgramming.Basics;
using FunctionalProgramming.Helpers;
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

        enum Command
        {
            Heads,
            Tails,
            Quit
        }

        static void Main(string[] args)
        {
            var toEnumerable = TheFaces.AsQueryable();
            Console.WriteLine("Head (h), Tail (t), or Quit (q)?");
            var maybeCommand = ProcessCommand(Console.ReadLine());
            //GetOrElse returns true if the user input is not a command
            while (maybeCommand.Select(command => command != Command.Quit).GetOrElse(() => true))
            {
                var isHeadsOrTails = maybeCommand.Select(command => command != Command.Quit).GetOrElse(() => false);
                if (isHeadsOrTails)
                {

                    ForEach(Rnr((int) DateTime.Now.Ticks, 8).Take(20), Console.WriteLine);

                    var tossed = GetResult(Randomizer(MinNr, MaxNr));
                    //var userInput = Console.ReadLine().ToMaybe();
                    Console.WriteLine("Result: {0}", tossed);
                    //Console.WriteLine("Your Answer: " + userInput.ToString());
                    //Console.WriteLine(from c in TheFaces.FirstOrDefault(x => x.Name == userInput.ToString()).ToMaybe() select c.Name);
                    if (Compare(tossed, maybeCommand))
                    {
                        Console.WriteLine("You win.");
                    }
                    else
                    {
                        Console.WriteLine("Nope!" + from t in toEnumerable.SingleOrDefault(x => x.ShortName == Convert(tossed)).ToMaybe() select t.Name);
                    }

                }
                else
                {
                    Console.WriteLine("I'm sorry. I am unable to understand that.");
                }
                Console.WriteLine("Head (h), Tail (t), or Quit (q)?");
                maybeCommand = ProcessCommand(Console.ReadLine());
            }
            Console.WriteLine("GoodBye");
        }

        static IMaybe<Command> ProcessCommand(string userInput)
        {
            var retVal = Maybe.Nothing<Command>();
            if (userInput.Equals("h"))
            {
                retVal = Command.Heads.ToMaybe();
            }
            else if (userInput.Equals("t"))
            {
                retVal = Command.Tails.ToMaybe();
            }
            else if (userInput.Equals("q"))
            {
                retVal = Command.Quit.ToMaybe();
            }
            return retVal;
        }

        static IStream<int> Rnr(int a, int b)
        {
            return b.Cons(() => Rnr(new Random().Next(), a + b));
        }

        static Unit ForEach<T>(IStream<T> ts, Action<T> action)
        {
            return ts.FoldL<T, Func<Unit, Unit>>(BasicFunctions.Identity<Unit>, (f, t) => DoWhatYouAreTold(t, action).Compose(f))(Unit.Only);
        }

        static Func<Unit, Unit> DoWhatYouAreTold<T>(T t, Action<T> action)
        {
            return u =>
            {
                action(t);
                return u;
            };
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

        static bool Compare(IMaybe<string> maybeVal, IMaybe<Command> maybeCommand)
        {
            return (from command in maybeCommand
                from val in maybeVal
                select Compare(val, command)).GetOrElse(() => false);
        }

        static bool Compare(string val, Command command)
        {
            return (val.Equals("h") && command == Command.Heads) || (val.Equals("t") && command == Command.Tails);
        }

        static string Convert(IMaybe<string> aVal)
        {
            return aVal.ToString().Replace("Just(", "").Replace(")", "");
        }

    }
}
