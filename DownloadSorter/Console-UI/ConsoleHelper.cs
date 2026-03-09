using System;
using System.Collections.Generic;
using System.Text;

namespace DownloadSorter.Console_UI
{
	public class ConsoleHelper
	{

		public static string TitleArt()
		{
			return """
                  ██████   ██████  ██     ██ ███    ██ ██       ██████   █████  ██████
                  ██   ██ ██    ██ ██     ██ ████   ██ ██      ██    ██ ██   ██ ██   ██ 
                  ██   ██ ██    ██ ██  █  ██ ██ ██  ██ ██      ██    ██ ███████ ██   ██ 
                  ██   ██ ██    ██ ██ ███ ██ ██  ██ ██ ██      ██    ██ ██   ██ ██   ██ 
                  ██████   ██████   ███ ███  ██   ████ ███████  ██████  ██   ██ ██████  

                  ███████  ██████  ██████  ████████ ███████ ██████                     
                  ██      ██    ██ ██   ██    ██    ██      ██   ██                    
                  ███████ ██    ██ ██████     ██    █████   ██████                     
                       ██ ██    ██ ██   ██    ██    ██      ██   ██                    
                  ███████  ██████  ██   ██    ██    ███████ ██   ██ 
                """;
		}

		public static string Version()
		{
			return $"						[v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}]";
		}

		public static void ConsoleWriteLine(string text, ConsoleColor? color = null)
		{
			if (color != null)
			{
				Console.ForegroundColor = color.Value;
			}
			Console.WriteLine(text);
			Console.ResetColor();
		}
		public static void ConsoleWrite(string text, ConsoleColor? color = null)
		{
			if (color != null)
			{
				Console.ForegroundColor = color.Value;
			}
			Console.Write(text);
			Console.ResetColor();
		}
	}
}
