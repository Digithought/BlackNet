using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digithought.BlackNet
{
	public class Gpio : PortBase
	{
		private static readonly Dictionary<BbbPort, int> GpioTestMappings =
			new Dictionary<BbbPort, int>
			{
				{ BbbPort.P9_11, 30 }, { BbbPort.P9_12, 60 }, { BbbPort.P9_13, 31 }, { BbbPort.P9_14, 50 }, { BbbPort.P9_15, 48 }, { BbbPort.P9_16, 51 },
				{ BbbPort.P9_21, 3 }, { BbbPort.P9_22, 2 }, { BbbPort.P9_23, 49 }, { BbbPort.P9_24, 15 }, { BbbPort.P9_25, 117 }, { BbbPort.P9_26, 14 },
				{ BbbPort.P9_27, 115 }, { BbbPort.P9_28, 113 }, { BbbPort.P9_29, 111 }, { BbbPort.P9_30, 112 }, { BbbPort.P9_31, 110 }, { BbbPort.P9_41, 20 },
				{ BbbPort.P9_42, 7 },

				{ BbbPort.P8_3, 38 }, { BbbPort.P8_4, 39 }, { BbbPort.P8_5, 34 }, { BbbPort.P8_6, 35 }, { BbbPort.P8_7, 66 }, { BbbPort.P8_8, 67 },
				{ BbbPort.P8_9, 69 }, { BbbPort.P8_10, 68 }, { BbbPort.P8_11, 45 }, { BbbPort.P8_12, 44 }, { BbbPort.P8_13, 23 }, { BbbPort.P8_14, 26 },
				{ BbbPort.P8_15, 47 }, { BbbPort.P8_16, 46 }, { BbbPort.P8_17, 27 }, { BbbPort.P8_18, 65 }, { BbbPort.P8_19, 22 }, { BbbPort.P8_20, 63 },
				{ BbbPort.P8_21, 62 }, { BbbPort.P8_22, 37 }, { BbbPort.P8_23, 36 }, { BbbPort.P8_24, 33 }, { BbbPort.P8_25, 32 }, { BbbPort.P8_26, 61 },
				{ BbbPort.P8_27, 86 }, { BbbPort.P8_28, 88 }, { BbbPort.P8_29, 87 }, { BbbPort.P8_30, 89 }, { BbbPort.P8_31, 10 }, { BbbPort.P8_32, 11 },
				{ BbbPort.P8_33, 9 }, { BbbPort.P8_34, 81 }, { BbbPort.P8_35, 8 }, { BbbPort.P8_36, 81 }, { BbbPort.P8_37, 78 }, { BbbPort.P8_38, 79 },
				{ BbbPort.P8_39, 76 }, { BbbPort.P8_40, 77 }, { BbbPort.P8_41, 74 }, { BbbPort.P8_42, 75 }, { BbbPort.P8_43, 72 }, { BbbPort.P8_44, 73 },
				{ BbbPort.P8_45, 70 }, { BbbPort.P8_46, 71 },
			};

		private const string GpioPath = "/sys/class/gpio/";
		private const string GpioPrefix = "gpio";

		private bool _checkSafety;

		/// <summary> Wraps a digital I/O port. </summary>
		/// <param name="autoConfigure"> Whether to automatically configure the port.  Set to true unless you're certain the port is already configured. </param>
		/// <param name="checkSafety"> When true, extra checks are performed to be sure the port is in ready state before writing. </param>
		public Gpio(BbbPort port, bool autoConfigure = true, bool checkSafety = true) : base(port, autoConfigure)
		{
			_checkSafety = checkSafety;
		}

		public override void Configure()
		{
			if (!Directory.Exists(GetGioDevicePath()))
				WriteToFile(GpioPath + "export", GpioTestMappings[Port].ToString());
		}

		public override void Unconfigure()
		{
			if (Directory.Exists(GetGioDevicePath()))
				WriteToFile(GpioPath + "unexport", GpioTestMappings[Port].ToString());
		}

		public BbbDirection? Direction
		{
			get
			{
				var value = ReadFromFile(DirectionFileName()).Trim();
				return value == "in" ? BbbDirection.In 
					: value == "out" ? BbbDirection.Out 
					: (BbbDirection?)null;
			}
			set
			{
				WriteToFile(DirectionFileName(), value.Value == BbbDirection.In ? "in" : "out");
			}
		}

		private string DirectionFileName()
		{
			return Path.Combine(GetGioDevicePath(), "direction");
		}

		private string GetGioDevicePath()
		{
			return Path.Combine(GpioPath, GpioPrefix + GpioTestMappings[Port]);
		}

		public bool IsReady
		{
			get { return IsConfigured && Direction != null; }
		}

		public bool IsConfigured
		{
			get { return File.Exists(ValueFileName()); }
		}

		public int Value
		{
			get
			{
				return int.Parse(ReadFromFile(ValueFileName()));
			}
			set
			{
				if (_checkSafety && !IsReady)
					throw new BlackNetException(String.Format("Cannot write to port {0} as it is not ready.  The port must be configured and direction set.", Port));
				WriteToFile(ValueFileName(), value.ToString());
			}
		}

		private string ValueFileName()
		{
			return Path.Combine(GetGioDevicePath(), "value");
		}

		public bool IsHigh
		{
			get
			{
				return Value != 0;
			}
			set
			{
				Value = value ? 1 : 0;
			}
		}
	}
}
