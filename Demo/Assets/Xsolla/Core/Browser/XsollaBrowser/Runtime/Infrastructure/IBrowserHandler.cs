using System.Threading;

namespace Xsolla.XsollaBrowser
{
	public interface IBrowserHandler
	{
		void Run(BrowserPage page, CancellationToken cancellationToken);

		void Stop();
	}
}