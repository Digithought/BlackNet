using Digithought.BlackNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Digithought.ConsoleUi;

namespace Digithought.BlackNet.InteractiveTests
{
	class Program
	{
		static void Main(string[] args)
		{
			Ui.Menu
			(
				new Option("PWM", PwmTests),
				new Option("GPIO", GpioTests)
			);
		}

		private static void GpioTests()
		{
			Console.Write("port: ");
			var port = (BbbPort)Enum.Parse(typeof(BbbPort), Console.ReadLine());

			Console.Write("\r\nautoConfigure: ");
			var autoConfigure = JsonConvert.DeserializeObject<bool>(Console.ReadLine());

			var gpio = new Gpio(port, autoConfigure);

			Console.WriteLine("\r\nCreated.");

			Ui.DynamicMenu(gpio);
		}

		private static void PwmTests()
		{
			Console.Write("port: ");
			var port = (BbbPort)Enum.Parse(typeof(BbbPort), Console.ReadLine());
			
			Console.Write("\r\nautoConfigure: ");
			var autoConfigure = JsonConvert.DeserializeObject<bool>(Console.ReadLine());

			var pwm = new Pwm(port, autoConfigure);
			
			Console.WriteLine("\r\nCreated.");
			
			Ui.DynamicMenu(pwm);
		}
	}
}
