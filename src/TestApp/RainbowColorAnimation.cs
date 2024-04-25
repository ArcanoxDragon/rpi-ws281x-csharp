using System;
using System.Drawing;
using System.Threading;
using rpi_ws281x;

namespace TestApp
{
	internal class RainbowColorAnimation : IAnimation
	{
		private static readonly Color[] AnimationColors = [
			Color.FromArgb(0x201000),
			Color.FromArgb(0x202000),
			Color.Green,
			Color.FromArgb(0x002020),
			Color.Blue,
			Color.FromArgb(0x100010),
			Color.FromArgb(0x200010),
		];

		private static int colorOffset;

		public void Execute(CancellationToken cancellationToken)
		{
			Console.Clear();
			Console.Write("How many LEDs to you want to use: ");

			var ledCount = int.Parse(Console.ReadLine());

			//The default settings uses a frequency of 800000 Hz and the DMA channel 10.
			var settings = Settings.CreateDefaultSettings();

			//Set brightness to maximum (255)
			//Use Unknown as strip type. Then the type will be set in the native assembly.
			settings.Channels[0] = new Channel(ledCount, 18, 255, false, StripType.WS2812_STRIP);

			using var controller = new WS281x(settings);

			while (!cancellationToken.IsCancellationRequested)
			{
				for (int i = 0; i <= controller.Settings.Channels[0].LEDCount - 1; i++)
				{
					var colorIndex = ( i + colorOffset ) % AnimationColors.Length;
					controller.SetLEDColor(0, i, AnimationColors[colorIndex]);
				}

				controller.Render();

				if (colorOffset == int.MaxValue)
				{
					colorOffset = 0;
				}

				colorOffset++;
				Thread.Sleep(50);
			}
		}
	}
}