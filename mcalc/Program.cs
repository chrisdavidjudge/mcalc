using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Mathos.Parser;
using mcalc.Extensions;
using mcalc.XML;

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

#if DEBUG
            Console.Write(">> ");
            string expression = Console.ReadLine();
#else
            string expression = string.Join(' ', args);
#endif

            MathParser parser;
            string input = expression;
            string argsstr = Parse(input);

            Regex cultureInfoRegex = new Regex(@"--cultureinfo\s*:\s*(?<civ>[a-z]+-[a-z]+)", RegexOptions.IgnoreCase);
            Match m = cultureInfoRegex.Match(argsstr);
            if (m.Success)
            {
                parser = new(true, true, true, new CultureInfo(m.Groups["civ"].Value));
                argsstr = argsstr.Replace(m.Groups[0].Value, string.Empty);
            }
            else
            {
                parser = new();
            }

            string localVarFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XML", "LocalVariables.xml");
            FileInfo localVarFileInfo = new FileInfo(localVarFilePath);
            List<LocalVariable> localVars = new List<LocalVariable>();
            if (localVarFileInfo.Exists)
            {
                
                using (FileStream fileStream = new(localVarFileInfo.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        string fileContents = streamReader.ReadToEnd() ?? string.Empty;
                        if (fileContents.Trim() != string.Empty)
                        {
                            localVars = (List<LocalVariable>)Serialization.Deserialize(localVarFilePath, typeof(List<LocalVariable>));
                            foreach(var item in localVars)
                            {
                                parser.LocalVariables.Add(item.Name, item.Value);
                            }
                        }
                    }
                }
            }

            CommandCollection commands = new()
            {
                new Command("--help", "Display help"),
                new Command("--lv", "List all supported variables"),
                new Command("--add-lv <varname varvalue>", "Add a new variable"),
                new Command("--lf", "List all supported functions"),
                new Command("--lops", "List all supported operators"),
                new Command("--envv", "List all environment variables"),
                new Command("--ci", "Display Culture Information"),
                new Command("--version", "Display the version of this app"),
            };

            while (input != null)
			{
                try
                {
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
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
                                    Console.Write(" ".ToString().PadRight(maxLen));
                                    Console.WriteLine($"{value}");
                                }
                                Console.ResetColor();
                            }
                            return;
                        }
                        else if (argsstr.Trim().ToLower() == commands.Where(((c) => { return c.Name == "--ci"; })).First().Name)
                        {
                            CultureInfo cultureInfo = parser.CultureInfo;
                            Console.Write("> ");
                            Console.Write(' ');
                            Console.WriteLine($"EnglishName: {cultureInfo.EnglishName}");
                            Console.Write("> ");
                            Console.Write(' ');
                            Console.WriteLine($"DisplayName: {cultureInfo.DisplayName}");
                            Console.Write("> ");
                            Console.Write(' ');
                            Console.WriteLine($"NativeName : {cultureInfo.NativeName}");
                            Console.Write("> ");
                            Console.Write(' ');
                            Console.WriteLine($"LCID       : {cultureInfo.LCID}");
                            Console.Write("> ");
                            Console.Write(' ');
                            Console.WriteLine($"NumberFormat.CurrencySymbol: {cultureInfo.NumberFormat.CurrencySymbol}");
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
                        else
                        {
                            if(argsstr.StartsWith("--add-lv"))
                            {
                                System.Diagnostics.Debug.WriteLine("");
                                Regex regex = new Regex(@"^--add-lv\s+(?<varname>\w+)\s+(?<varval>\d+(.\d+)?)$", RegexOptions.IgnoreCase);
                                Match match = regex.Match(argsstr);
                                if (match.Success)
                                {
                                    // "--add-lv x 1.234"
                                    string varname = match.Groups["varname"].Value;
                                    double varval = double.Parse(match.Groups["varval"].Value);
                                    if(!parser.LocalVariables.ContainsKey(varname))
                                    {
                                        localVars.Add(new LocalVariable(varname, varval));
                                        parser.LocalVariables.Add(varname, varval);
                                        using FileStream fileStream = new(localVarFileInfo.FullName, FileMode.OpenOrCreate, FileAccess.Write);
                                        Serialization.Serialize(fileStream, localVars);
                                        fileStream.Flush();
                                        Console.WriteLine($"> {varname} = {parser.LocalVariables[varname]}");
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        Console.WriteLine($"> A variable of {varname} already exists.");
                                        Console.ResetColor();
                                    }
                                }
                                return;
                            }
                            else if (argsstr.StartsWith("--rem-lv"))
                            {
                                Regex regex = new Regex(@"^--rem-lv\s+(?<varname>\w+)$", RegexOptions.IgnoreCase);
                                Match match = regex.Match(argsstr);
                                if (match.Success)
                                {
                                    string varname = match.Groups["varname"].Value.Trim();
                                    if(parser.LocalVariables.ContainsKey(varname))
                                    {
                                        LocalVariable localVariable = localVars.FirstOrDefault(x => x.Name == varname);
                                        if (localVariable != null)
                                        {
                                            localVars.Remove(localVariable);
                                            parser.LocalVariables.Remove(varname);
                                            using FileStream fileStream = new(localVarFileInfo.FullName, FileMode.OpenOrCreate, FileAccess.Write);
                                            Serialization.Serialize(fileStream, localVars);
                                            fileStream.Flush();
                                            Console.WriteLine($"> {varname} has been removed.");
                                        }
                                    }
                                }
                                return;
                            }
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
