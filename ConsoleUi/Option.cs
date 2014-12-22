using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Digithought.ConsoleUi
{
	public class Option
	{
		public string Description { get; private set; }
		public Action Action { get; private set; }

		public Option(string description, Action action)
		{
			Description = description;
			Action = action;
		}
	}
}
