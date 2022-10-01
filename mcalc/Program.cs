using System;
using System.Text;
using Mathos.Parser;

namespace mcalc
{
	internal class Program
	{
		internal static void Main(string[] args)
		{

#if DEBUG
			Console.Write("Enter expression: ");
			args = new string[] { Console.ReadLine() };
#endif

			MathParser parser = new();
			try
			{
				Console.ResetColor();
				Console.ForegroundColor = ConsoleColor.Green;
				string argsstr = Parse(args);
				if(argsstr != null)
				{
					if(argsstr.Trim().ToLower() == "lv")
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
					else if (argsstr.Trim().ToLower() == "help" || argsstr.Trim().ToLower() == "?")
					{
						Console.Write("> ");
						Console.Write("lf");
						Console.Write(' ');
						Console.WriteLine("Built-In functions");
						Console.Write("> ");
						Console.Write("lv");
						Console.Write(' ');
						Console.WriteLine("Built-In variables");
					}
				}
				Console.Write("> ");
				Console.Write(argsstr);
				Console.Write(" = ");
				Console.WriteLine(parser.Parse(Parse(args)));
			}
			catch (Exception ex)
			{
				Console.ForegroundColor=ConsoleColor.Red;
				Console.Write("Exception: ");
				Console.Write(ex.ToString());
				Console.ResetColor();
			}
			finally
			{
				Console.ResetColor();
			}
#if DEBUG
			Console.Read();
#endif

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
