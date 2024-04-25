using System.Threading;

namespace TestApp
{
	internal interface IAnimation
	{
		void Execute(CancellationToken cancellationToken);
	}
}