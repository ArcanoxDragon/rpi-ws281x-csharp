using System.Threading;
using rpi_ws281x;

namespace TestApp
{
	internal interface IAnimation
	{
		void Execute(Settings settings, CancellationToken cancellationToken);
	}
}