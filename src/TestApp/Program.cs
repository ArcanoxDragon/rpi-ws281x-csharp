using System;
using System.Threading;
using rpi_ws281x;
using TestApp;

var cancelSource = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) => {
	e.Cancel = true;
	// ReSharper disable once AccessToModifiedClosure
	cancelSource.Cancel();
};

var input = 0;

do
{
	Console.Clear();
	Console.WriteLine("What do you want to test:");
	Console.WriteLine("Press CTRL + C to abort to current test.");
	Console.WriteLine("0 - Exit");
	Console.WriteLine("1 - Color wipe animation");
	Console.WriteLine("2 - Rainbow color animation");
	Console.Write("What is your choice: ");

	if (!int.TryParse(Console.ReadLine(), out input))
	{
		Console.WriteLine("Invalid option.");
		Thread.Sleep(1000);
		input = -1;
		continue;
	}

	var animation = GetAnimation(input);

	if (animation != null)
	{
		Console.Clear();
		Console.Write("How many LEDs do you want to use? ");

		if (!int.TryParse(Console.ReadLine(), out var ledCount))
		{
			Console.WriteLine("Invalid number.");
			Thread.Sleep(1000);
			continue;
		}

		var settings = Settings.CreateDefaultSettings();

		settings.Channels[0] = new Channel(ledCount, 21, stripType: StripType.SK6812W_STRIP);

		animation.Execute(settings, cancelSource.Token);

		if (!cancelSource.TryReset())
			cancelSource = new CancellationTokenSource();
	}
}
while (input != 0);

static IAnimation GetAnimation(int code)
{
	IAnimation result = code switch {
		1 => new ColorWipe(),
		2 => new RainbowColorAnimation(),
		_ => null,
	};

	return result;
}