using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace Native
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct ws2811_t
	{
		public long            render_wait_time;
		public IntPtr          device;
		public IntPtr          rpi_hw;
		public uint            freq;
		public int             dmanum;
		public ws2811_channels channels;
	}

	[InlineArray(PInvoke.RPI_PWM_CHANNELS)]
	internal struct ws2811_channels
	{
		public ws2811_channel_t channel;
	}
}