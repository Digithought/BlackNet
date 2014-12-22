using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digithought.BlackNet
{
	public abstract class SlotPortBase : PortBase
	{
		public SlotPortBase(BbbPort port, bool autoConfigure = true) : base(port, autoConfigure) { }

		private string _slotsPath;

		protected string SlotsPath
		{
			get
			{
				if (_slotsPath == null)
					_slotsPath =
					(
						Directory.EnumerateDirectories("/sys/devices/", "bone_capemgr.*").FirstOrDefault()
							?? "/sys/devices/bone_capemgr.8"	// Default manager number
					) + "/slots";
				return _slotsPath;
			}
		}

		protected abstract string GetSlotName();

		protected void WriteToSlots(string value)
		{
			WriteToFile(SlotsPath, value);
		}

		protected void RemoveSlot(string slotText)
		{
			if (slotText != null && slotText.IndexOf(':') >= 1)
				WriteToSlots("-" + slotText.Substring(0, slotText.IndexOf(':')).Trim());
		}
	}
}
