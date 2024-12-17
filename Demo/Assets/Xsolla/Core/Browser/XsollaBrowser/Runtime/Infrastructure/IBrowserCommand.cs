using System.Threading.Tasks;
using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public interface IBrowserCommand
	{
		Task Perform(IPage page);
	}
}