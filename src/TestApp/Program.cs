using System;
using System.Threading;
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
		animation.Execute(cancelSource.Token);

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