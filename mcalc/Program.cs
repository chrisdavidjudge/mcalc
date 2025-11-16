using System;
using System.Text;
using Mathos.Parser;

namespace mcalc
{
	internal class Program
	{

        private static void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("WINDOWS MCALC");
            Console.WriteLine("-------------");
            Console.WriteLine();
        }

        internal static void Main(string[] args)
		{
            DisplayHeader();

            Console.Write("> ");

            MathParser parser = new();

            string input = Console.ReadLine();

			while (input != null)
			{
                try
                {
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    string argsstr = Parse(input);
                    if (argsstr != null)
                    {
                        if (argsstr.Trim().ToLower() == "lv")
                        {
                            foreach (var item in parser.LocalVariables)
                            {
                                Console.Write("> ");
                                Console.Write(item.Key);
                                Console.Write(' ');
                                Console.WriteLine(item.Value);
                            }
                            return;
                        }
                        else if (argsstr.Trim().ToLower() == "lf")
                        {
                            foreach (var item in parser.LocalFunctions)
                            {
                                try
                                {
                                    Console.Write("> ");
                                    Console.Write(item.Key);
                                    Console.Write(' ');
                                    Console.WriteLine(item.Value);
                                }
                                catch (Exception ex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("Exception:- ");
                                    Console.Write(ex.Message);
                                    Console.ResetColor();
                                }
                            }
                            return;
                        }
                    }
                    Console.Write("> ");
                    Console.Write(argsstr);
                    Console.Write(" = ");
                    Console.WriteLine(parser.Parse(Parse(input)));
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Exception: ");
                    Console.Write(ex.ToString());
                    Console.ResetColor();
                }
                finally
                {
                    Console.ResetColor();
                }
                return;
            }

        }

        private static string Parse(string input)
		{
			return input;
		}

        private static string Parse(string[] args)
		{
			StringBuilder stringBuilder = new();
			foreach(string arg in args)
			{
				stringBuilder.Append(arg.Trim());
			}
			return stringBuilder.ToString();
		}
	}
}
