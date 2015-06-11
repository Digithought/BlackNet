using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Digithought.ConsoleUi
{
    public static class Ui
    {
		public static void Menu(params Option[] options)
		{
			while (true)
			{
				Console.WriteLine("0. Exit");
				for (var i = 0; i < options.Length; i++)
					Console.WriteLine((i + 1).ToString() + ". " + options[i].Description);
				Console.Write(">");
				var selection = Console.ReadLine();
				Console.WriteLine();
				if (selection == "0")
					return;
				int number;
				if (int.TryParse(selection, out number))
				{
					try
					{
						options[number - 1].Action();
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
			}
		}

		public static void DynamicMenu(Object instance)
		{
			var type = instance.GetType();
			Menu
			(
				type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
					.Select(m => new Option(m.Name, () => InvokeMethod(instance, m)))
					.ToArray()
			);
		}

		public static void InvokeMethod(object instance, System.Reflection.MethodInfo method)
		{
			try
			{
				var result = method.Invoke
				(
					instance,
					method.GetParameters().Select(p =>
					{
						Console.WriteLine(p.Name + ": ");
						var value = Console.ReadLine();
						Console.WriteLine();
						return Newtonsoft.Json.JsonConvert.DeserializeObject(value, p.ParameterType);
					}
					).ToArray()
				);

				Console.WriteLine("Result: " + Newtonsoft.Json.JsonConvert.SerializeObject(result));
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}
	}
}
