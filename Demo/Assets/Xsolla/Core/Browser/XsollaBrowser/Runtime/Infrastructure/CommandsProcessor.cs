using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public class CommandsProcessor
	{
		private readonly ConcurrentQueue<IBrowserCommand> Queue;
		private readonly MainThreadLogger MainThreadLogger;

		public CommandsProcessor(MainThreadLogger mainThreadLogger)
		{
			Queue = new ConcurrentQueue<IBrowserCommand>();
			MainThreadLogger = mainThreadLogger;
		}

		public void AddCommand(IBrowserCommand command)
		{
			Queue.Enqueue(command);
		}

		public async Task ProcessCommands(IPage page, CancellationToken cancellationToken)
		{
			while (Queue.Count > 0)
			{
				if (cancellationToken.IsCancellationRequested)
					break;

				try
				{
					if (Queue.TryDequeue(out var command))
					{
						await command.Perform(page);
					}
				}
				catch (Exception e)
				{
					MainThreadLogger.Log(e.Message);
				}
				finally
				{
					Thread.Sleep(1);
				}
			}
		}
	}
}