using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace org.maltparser.concurrent.test
{

	using  core.exception;

	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public sealed class ConcurrentTest
	{

		public static string getMessageWithElapsed(string message, long startTime)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			long elapsed = (System.nanoTime() - startTime) / 1000000;
			sb.Append(message);
			sb.Append(" : ");
			sb.Append(string.Format("{0:D2}:{1:D2}:{2:D2}", elapsed / 3600000, elapsed % 3600000 / 60000, elapsed % 60000 / 1000));
			sb.Append(" ( ");
			sb.Append(elapsed);
			sb.Append(" ms)");
			return sb.ToString();
		}

		public static void Main(string[] args)
		{
			long startTime = System.nanoTime();
			if (args.Length != 1)
			{
				Console.WriteLine("No experiment file.");
			}
			IList<Experiment> experiments = null;

			try
			{
				experiments = Experiment.loadExperiments(args[0]);
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (ExperimentException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			if (experiments == null)
			{
				Console.WriteLine("No experiments to process.");
				Environment.Exit(0);
			}
	//		System.out.println(experiments);

			try
			{

				ConcurrentMaltParserModel[] models = new ConcurrentMaltParserModel[experiments.Count];
				int nThreads = 0;
				for (int i = 0; i < experiments.Count; i++)
				{
					Experiment experiment = experiments[i];
					nThreads += experiment.nInURLs();
					models[i] = ConcurrentMaltParserService.initializeParserModel(experiment.ModelURL);
				}
				Console.WriteLine(getMessageWithElapsed("Finished loading models", startTime));
				Thread[] threads = new Thread[nThreads];
				int t = 0;
				for (int i = 0; i < experiments.Count; i++)
				{
					Experiment experiment = experiments[i];
					IList<URL> inUrls = experiment.InURLs;
					IList<File> outFiles = experiment.OutFiles;
					for (int j = 0; j < inUrls.Count; j++)
					{
						threads[t] = new ThreadClass(experiment.CharSet, inUrls[j], outFiles[j], models[i]);
						t++;
					}
				}
				Console.WriteLine(getMessageWithElapsed("Finished init threads", startTime));
				for (int i = 0; i < threads.Length; i++)
				{
					if (threads[i] != null)
					{
						threads[i].Start();
					}
					else
					{
						Console.WriteLine("Thread " + i + " is null");
					}
				}
				for (int i = 0; i < threads.Length; i++)
				{
					try
					{
						if (threads[i] != null)
						{
							threads[i].Join();
						}
						else
						{
							Console.WriteLine("Thread " + i + " is null");
						}
					}
					catch (InterruptedException)
					{
					}
				}
				Console.WriteLine(getMessageWithElapsed("Finished parsing", startTime));
			}
			catch (MaltChainedException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}
	}

}