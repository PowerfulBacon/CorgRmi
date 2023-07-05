using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Logging
{
	public class ConsoleLogger : ILogger
	{

		public void LogError(object target, string message)
		{
			Console.Error.WriteLine($"{target?.GetType()?.Name} {DateTime.Now.TimeOfDay}: {message}");
		}

		public void LogMessage(object target, string message)
		{
			Console.WriteLine($"{target?.GetType()?.Name} {DateTime.Now.TimeOfDay}: {message}");
		}

		public void LogWarning(object target, string message)
		{
			var previous = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"{target?.GetType()?.Name} {DateTime.Now.TimeOfDay}: {message}");
			Console.ForegroundColor = previous;
		}
	}
}
