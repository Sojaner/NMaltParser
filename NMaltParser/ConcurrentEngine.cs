using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace org.maltparser
{

	using  org.maltparser.concurrent;
	using  org.maltparser.concurrent;
	using  org.maltparser.concurrent;
	using  org.maltparser.concurrent;
	using  org.maltparser.core.config;
	using  org.maltparser.core.exception;
	using  org.maltparser.core.options;

	public class ConcurrentEngine
	{
		private readonly int optionContainer;
		private ConcurrentMaltParserModel model;

		public ConcurrentEngine(int optionContainer)
		{
			Console.WriteLine("Start ConcurrentEngine");
			this.optionContainer = optionContainer;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean canUseConcurrentEngine(int optionContainer) throws org.maltparser.core.exception.MaltChainedException
		public static bool canUseConcurrentEngine(int optionContainer)
		{
			if (!OptionManager.instance().getOptionValueString(optionContainer,"config", "flowchart").Equals("parse"))
			{
				return false;
			}
			if (OptionManager.instance().getOptionValueString(optionContainer,"config", "url").Length > 0)
			{
				return false;
			}
			return true;
		}

		public static string getMessageWithElapsed(string message, long startTime)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			long elapsed = (System.nanoTime() - startTime) / 1000000;
			sb.Append(message);
			sb.Append(": ");
			sb.Append(elapsed);
			sb.Append(" ms");
			return sb.ToString();
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadModel() throws org.maltparser.core.exception.MaltChainedException
		public virtual void loadModel()
		{
			Console.WriteLine("Start loadModel");
			long startTime = System.nanoTime();
			File workingDirectory = getWorkingDirectory(OptionManager.instance().getOptionValue(optionContainer, "config", "workingdir").ToString());
			string configName = OptionManager.instance().getOptionValueString(optionContainer,"config", "name");
			string pathToModel = workingDirectory.Path + File.separator + configName + ".mco";
			try
			{
				model = ConcurrentMaltParserService.initializeParserModel((new File(pathToModel)).toURI().toURL());
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			Console.WriteLine(getMessageWithElapsed("Loading time", startTime));
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.File getWorkingDirectory(String path) throws org.maltparser.core.exception.MaltChainedException
		public virtual File getWorkingDirectory(string path)
		{
			File workingDirectory;
			if (string.ReferenceEquals(path, null) || path.Equals("user.dir", StringComparison.OrdinalIgnoreCase) || path.Equals(".", StringComparison.OrdinalIgnoreCase))
			{
				workingDirectory = new File(System.getProperty("user.dir"));
			}
			else
			{
				workingDirectory = new File(path);
			}

			if (workingDirectory == null || !workingDirectory.Directory)
			{
				new ConfigurationException("The specified working directory '" + path + "' is not a directory. ");
			}
			return workingDirectory;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parse() throws org.maltparser.core.exception.MaltChainedException
		public virtual void parse()
		{
			Console.WriteLine("Start parse");
			long startTime = System.nanoTime();
			IList<string[]> inputSentences = new List<string[]>();

			StreamReader reader = null;
			string infile = OptionManager.instance().getOptionValueString(optionContainer,"input", "infile");
			string incharset = OptionManager.instance().getOptionValueString(optionContainer,"input", "charset");
			try
			{
				reader = new StreamReader(new FileStream(infile, FileMode.Open, FileAccess.Read), incharset);
				while (true)
				{
					string[] tokens = ConcurrentUtils.readSentence(reader);
					if (tokens.Length == 0)
					{
						break;
					}
					inputSentences.Add(tokens);

				}
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				if (reader != null)
				{
					try
					{
						reader.Close();
					}
					catch (IOException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
			}
			Console.WriteLine(getMessageWithElapsed("Read sentences time", startTime));


			int numberOfThreads = 8;

			Thread[] threads = new Thread[numberOfThreads];
			MaltParserRunnable[] runnables = new MaltParserRunnable[numberOfThreads];
			int nSentences = inputSentences.Count;
			int interval = (nSentences / numberOfThreads);
			int startIndex = 0;
			int t = 0;
			Console.WriteLine("Number of sentences : " + nSentences);
			while (startIndex < nSentences)
			{
				int endIndex = (startIndex + interval < nSentences && t < threads.Length - 1?startIndex + interval:nSentences);
				Console.WriteLine("  Thread " + string.Format("{0:D3}",t) + " will parse sentences between " + string.Format("{0:D4}",startIndex) + " - " + string.Format("{0:D4}",(endIndex - 1)) + ", number of sentences: " + (endIndex - startIndex));
				runnables[t] = new MaltParserRunnable(inputSentences.subList(startIndex, endIndex), model);
				threads[t] = new Thread(runnables[t]);
				startIndex = endIndex;
				t++;
			}
			Console.WriteLine(getMessageWithElapsed("Create threads time", startTime));
			startTime = System.nanoTime();

			// Starting threads to parse all sentences.
			for (int i = 0; i < threads.Length; i++)
			{
				if (threads[i] != null)
				{
					threads[i].Start();
				}
				else
				{
					Console.Error.WriteLine("Thread " + i + " is null");
				}
			}

			// Finally joining all threads
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
						Console.Error.WriteLine("Thread " + i + " is null");
					}
				}
				catch (InterruptedException)
				{
				}
			}
			Console.WriteLine(getMessageWithElapsed("Parsing time", startTime));
			startTime = System.nanoTime();
			string outfile = OptionManager.instance().getOptionValueString(optionContainer,"output", "outfile");
			string outcharset = OptionManager.instance().getOptionValueString(optionContainer,"output", "charset");
			StreamWriter writer = null;
			try
			{
				writer = new StreamWriter(new FileStream(outfile, FileMode.Create, FileAccess.Write), outcharset);
				for (int i = 0; i < threads.Length; i++)
				{
					IList<string[]> outputSentences = runnables[i].OutputSentences;
					for (int j = 0; j < outputSentences.Count; j++)
					{
						ConcurrentUtils.writeSentence(outputSentences[j], writer);
					}
				}
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				if (writer != null)
				{
					try
					{
						writer.Close();
					}
					catch (IOException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
			}
			Console.WriteLine(getMessageWithElapsed("Write sentences time", startTime));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate()
		{
			Console.WriteLine("Start terminate");
		}
	}

}