using System;
using System.IO;
using System.Threading;
using NMaltParser.Core.Exception;

namespace NMaltParser.Concurrent.Test
{
    /// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class ThreadClass : Thread
	{
		private URL inURL;
		private File outFile;
		private string charSet;
		private ConcurrentMaltParserModel model;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ThreadClass(String _charSet, String _inFile, String _outFile, org.maltparser.concurrent.ConcurrentMaltParserModel _model) throws java.net.MalformedURLException
		public ThreadClass(string _charSet, string _inFile, string _outFile, ConcurrentMaltParserModel _model)
		{
			charSet = _charSet;
			inURL = (new File(_inFile)).toURI().toURL();
			outFile = new File(_outFile);
			model = _model;
		}

		public ThreadClass(string _charSet, URL _inUrl, File _outFile, ConcurrentMaltParserModel _model)
		{
			charSet = _charSet;
			inURL = _inUrl;
			outFile = _outFile;
			model = _model;
		}

		public virtual void run()
		{
			StreamReader reader = null;
			StreamWriter writer = null;
			try
			{
				reader = new StreamReader(inURL.openStream(), charSet);
				writer = new StreamWriter(new FileStream(outFile, FileMode.Create, FileAccess.Write), charSet);
				int diffCount = 0;
				int sentenceCount = 0;
				while (true)
				{
					string[] goldTokens = ConcurrentUtils.readSentence(reader);
					if (goldTokens.Length == 0)
					{
						break;
					}
					string[] inputTokens = ConcurrentUtils.stripGold(goldTokens);
					string[] outputTokens = model.parseTokens(inputTokens);
					diffCount = ConcurrentUtils.diffSentences(goldTokens,outputTokens)?diffCount + 1:diffCount;
					sentenceCount++;
					ConcurrentUtils.writeSentence(outputTokens, writer);
				}
				Console.WriteLine("DiffCount: " + diffCount + "/" + sentenceCount + "(ThreadID:" + CurrentThread.Id + ")");
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (MaltChainedException e)
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
		}
	}

}