using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Mathos.Parser;

namespace mcalc
{
    internal sealed class Program
    {
        public sealed class CommandCollection : List<Command>
        {
            public bool Exists(string key)
            {
                return this.Where((item) => { return item.Name == key; }).Any();
            }
        }

        internal sealed class Command
        {
            public Command(string name, string description) 
            {
                Name = name;
                Description = description;
            }

            public string Name { get; private set; }
            public string Description{ get; private set; }

        }

        internal static void Main(string[] args)
		{

            string expression = string.Join(' ', args);
            MathParser parser = new();
            string input = expression;

            CommandCollection commands = new()
            {
                new Command("--help", "Display help"),
                new Command("--lv", "List all supported variables"),
                new Command("--lf", "List all supported functions"),
                new Command("--lops", "List all supported operators"),
                new Command("--envv", "List all environment variables"),
                new Command("--version", "Display the version of this app"),
            };

            while (input != null)
			{
                try
                {
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    string argsstr = Parse(input);
                    if (argsstr != null)
                    {
                        if (argsstr.Trim().ToLower() == commands.Where(((c) => { return c.Name == "--lv"; })).First().Name)
                        {
                            int maxLen = parser.LocalVariables.Max(v => v.Key.ToString().Length);
                            foreach (var item in parser.LocalVariables)
                            {
                                Console.Write("> ");
                                Console.Write(item.Key.ToString().PadRight(maxLen));
                                Console.Write(' ');
                                Console.WriteLine(item.Value);
                            }
                            return;
                        }
                        else if (argsstr.Trim().ToLower() == commands.Where(((c) => { return c.Name == "--lf"; })).First().Name)
                        {
                            int maxLen = parser.LocalFunctions.Max(v => v.Key.ToString().Length);
                            foreach (var item in parser.LocalFunctions)
                            {
                                try
                                {
                                    Console.Write("> ");
                                    Console.Write(item.Key.ToString().PadRight(maxLen));
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
                        else if (argsstr.Trim().ToLower() == commands.Where(((c) => { return c.Name == "--version"; })).First().Name)
                        {
                            Console.Write("> ");
                            Console.Write(' ');
                            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                            return;
                        }
                        else if (argsstr.Trim().ToLower() == commands.Where(((c) => { return c.Name == "--lops"; })).First().Name)
                        {
                            int maxLen = parser.Operators.Max(c => c.Key.Length);
                            foreach (var item in parser.Operators)
                            {
                                Console.Write("> ");
                                Console.Write(' ');
                                Console.WriteLine($"{item.Key.PadRight(maxLen)}: {item.Value}");
                            }
                            return;
                        }
                        else if (argsstr.Trim().ToLower() == commands.Where(((c) => { return c.Name == "--envv"; })).First().Name)
                        {
                            var items = Environment.GetEnvironmentVariables().OfType<DictionaryEntry>();
                            int maxLen = items.Max((i) => { return i.Key.ToString().Length; });
                            foreach (DictionaryEntry item in items)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("> ");
                                Console.Write($"{item.Key.ToString().PadRight(maxLen)}: ");
                                Console.ForegroundColor = ConsoleColor.Blue;
                                foreach(string value in item.Value.ToString().Split(';'))
                                {
                                    Console.Write(" ".ToString().PadRight(0));
                                    Console.WriteLine($"{value}");
                                }
                                Console.ResetColor();
                            }
                            return;
                        }
                        else if (argsstr.Trim().ToLower() == commands.Where(((c) => { return c.Name == "--help"; })).First().Name)
                        {
                            int maxLen = commands.Max(c => c.Name.Length);
                            foreach (var c in commands)
                            {
                                Console.Write("> ");
                                Console.Write(' ');
                                Console.WriteLine($"{c.Name.PadRight(maxLen)}: {c.Description}");
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
#if !DEBUG
                    Console.WriteLine("An input error occurred");
#else
                    Console.Write(ex.ToString());
#endif
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
