using System;
using System.Drawing;
using System.Threading;
using rpi_ws281x;

namespace TestApp
{
	internal class ColorWipe : IAnimation
	{
		public void Execute(Settings settings, CancellationToken cancellationToken)
		{
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