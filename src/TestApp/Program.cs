using System;
using TestApp;

var abort = new AbortRequest();

Console.CancelKeyPress += (_, e) => {
	e.Cancel = true;
	abort.IsAbortRequested = true;
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
	input = int.Parse(Console.ReadLine());

	var animation = GetAnimation(input);
	if (animation != null)
	{
		abort.IsAbortRequested = false;
		animation.Execute(abort);
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