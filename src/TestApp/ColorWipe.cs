using System;
using System.Drawing;
using System.Threading;
using rpi_ws281x;

namespace TestApp
{
	internal class ColorWipe : IAnimation
	{
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

			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					Wipe(controller, Color.Red, cancellationToken);
					Wipe(controller, Color.Green, cancellationToken);
					Wipe(controller, Color.Blue, cancellationToken);
				}
			}
			catch (OperationCanceledException)
			{
				// Ignored
			}
		}

		private static void Wipe(WS281x controller, Color color, CancellationToken cancellationToken)
		{
			for (int i = 0; i <= controller.Settings.Channels[0].LEDs.Length - 1; i++)
			{
				controller.SetLEDColor(0, i, color);
				controller.Render();

				cancellationToken.ThrowIfCancellationRequested();

				Thread.Sleep(1000 / 15);
			}
		}
	}
}