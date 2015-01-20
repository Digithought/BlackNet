using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digithought.BlackNet
{
	public class Pwm : SlotPortBase
	{
		private static readonly Dictionary<BbbPort, string> PwmTestMappings = 
			new Dictionary<BbbPort, string>
			{
				{ BbbPort.P8_13, "pwm_test_P8_13.*" },
				{ BbbPort.P8_19, "pwm_test_P8_19.*" },
				{ BbbPort.P9_14, "pwm_test_P9_14.*" },
				{ BbbPort.P9_16, "pwm_test_P9_15.*" },
				{ BbbPort.P9_21, "pwm_test_P9_21.*" },
				{ BbbPort.P9_22, "pwm_test_P9_22.*" },
				{ BbbPort.P9_42, "pwm_test_P9_42.*" },
			};
		private const string PwmSlotName = "am33xx_pwm";
		private const string PwmPrefix = "bone_pwm_";
		private const int SlotConfigurationDelayTime = 300;	// Time to wait before attempting use after configuration


		private string devicePath;

		/// <summary> Constructs a Pulse Width Modulation (PWM) GPIO port wrapper. </summary>
		/// <param name="port"> Must be an available port supporting PWM. </param>
		/// <param name="autoConfigure"> Configure the device tree.  Should be true unless you are certain that it is already configured or you are constructing the instance to unconfgure it. </param>
		public Pwm(BbbPort port, bool autoConfigure = true) : base(port, autoConfigure) { }

		private string DevicePath
		{
			get
			{
				if (devicePath == null)
					devicePath = GetDevicePath(Port, PwmTestMappings);
				return devicePath;
			}
		}

		protected static string GetDevicePath(BbbPort port, IDictionary<BbbPort, string> mappings)
		{
			string result;
			var ocpPath = Directory.EnumerateDirectories("/sys/devices/", "ocp.*").FirstOrDefault()
				?? "/sys/devices/ocp.2";	// Default OCP number
			var mapping = mappings[port];
			result = Directory.EnumerateDirectories(ocpPath, mapping).FirstOrDefault();
			if (result == null)
				throw new BlackNetException(String.Format("Unable to find the device path for '{0}' in OCP path '{1}'.", mapping, ocpPath));
			return result;
		}

		public override void Configure()
		{
			if (!ReadLinesFromFile(SlotsPath).Where(s => s.Contains(GetSlotName())).Any())
			{
				WriteToSlots(PwmSlotName);
				WriteToSlots(GetSlotName());

				// Wait briefly for the configuration to take, else an error will occur on first use
				System.Threading.Thread.Sleep(SlotConfigurationDelayTime);
			}
		}

		protected override string GetSlotName()
		{
			return PwmPrefix + Port.ToString();
		}

		/// <remarks> Does nothing if not configured.  Unconfigures PWM in general in the event that no PWM ports are used. </remarks>
		public override void Unconfigure()
		{
			var slots = ReadLinesFromFile(SlotsPath);

			// If bone_pwm_<port> is found, write "-x" where x is the slot number
			RemoveSlot(slots.Where(s => s.Contains(GetSlotName())).FirstOrDefault());

			// Unconfigure PWM in general if no more PWM ports are in-use
			if (slots.Any(s => s.Contains(PwmPrefix)))
			{
				RemoveSlot(slots.Where(s => s.Contains(PwmSlotName)).FirstOrDefault());
			}
		}

		public int Period
		{
			get
			{
				return int.Parse(ReadFromFile(PeriodFileName()));
			}
			set
			{
				WriteToFile(PeriodFileName(), value.ToString());
			}
		}

		private string PeriodFileName()
		{
			return Path.Combine(DevicePath, "period");
		}

		public int Duty
		{
			get
			{
				return int.Parse(ReadFromFile(DutyFileName()));
			}
			set
			{
				WriteToFile(DutyFileName(), value.ToString());
			}
		}

		private string DutyFileName()
		{
			return Path.Combine(DevicePath, "duty");
		}

		public bool Run
		{
			get
			{
				return ParseBool(ReadFromFile(RunFileName()));
			}
			set
			{
				WriteToFile(RunFileName(), value ? "1" : "0");
			}
		}

		private string RunFileName()
		{
			return Path.Combine(DevicePath, "run");
		}

		public bool PolarityStraight
		{
			get
			{
				return ParseBool(ReadFromFile(PolarityFileName()));
			}
			set
			{
				WriteToFile(PolarityFileName(), value ? "1" : "0");
			}
		}

		private string PolarityFileName()
		{
			return Path.Combine(DevicePath, "polarity");
		}

		public float DutyPercent
		{
			get
			{
				return (1f - (float)Duty / (float)Period) * 100;
			}
			set
			{
				Duty = (int)Math.Round((float)Period * (1f - (value / 100f)));
			}
		}
	}
}
