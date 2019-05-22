using System;
using System.Collections.Generic;
using System.IO;

namespace org.maltparser.concurrent
{

	/// <summary>
	/// This class contains some basic methods to read sentence from file, write sentence to file, 
	/// strip gold-standard information from the input, print sentence to stream and check difference between two sentences.
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ConcurrentUtils
	{
		/// <summary>
		/// Reads a sentence from the a reader and returns a string array with tokens.
		/// 
		/// The method expect that each line contains a token and empty line is equal to end of sentence.
		/// 
		/// There are no check for particular data format so if the input is garbage then the output will also be garbage. 
		/// </summary>
		/// <param name="reader"> a buffered reader </param>
		/// <returns> a string array with tokens </returns>
		/// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String[] readSentence(java.io.BufferedReader reader) throws java.io.IOException
		public static string[] readSentence(StreamReader reader)
		{
			List<string> tokens = new List<string>();
			string line;
			while (!ReferenceEquals(line = reader.ReadLine(), null))
			{
				if (line.Trim().Length == 0)
				{
					break;
				}
				else
				{
					tokens.Add(line.Trim());
				}

			}
			return tokens.ToArray();
		}



		/// <summary>
		/// Writes a sentence to a writer. It expect a string array with tokens.
		/// 
		/// Each token will be one line and after all tokens are written there will be one empty line marking the ending of sentence.
		/// </summary>
		/// <param name="inTokens"> </param>
		/// <param name="writer"> a buffered writer </param>
		/// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void writeSentence(String[] inTokens, java.io.BufferedWriter writer) throws java.io.IOException
		public static void writeSentence(string[] inTokens, StreamWriter writer)
		{
			foreach (var t in inTokens)
            {
                writer.Write(t);
                writer.WriteLine();
            }
			writer.WriteLine();
			writer.Flush();
		}

		/// <summary>
		/// Strips the two last columns for each tokens. This method can be useful when reading a file with gold-standard 
		/// information in the last two columns and you want to parse without gold-standard information.  
		/// 
		/// The method expect that each columns are separated with a tab-character.
		/// </summary>
		/// <param name="inTokens"> a string array with tokens where each column are separated with a tab-character </param>
		/// <returns> a string array with tokens without the last two columns </returns>
		public static string[] stripGold(string[] inTokens)
		{
			return stripGold(inTokens, 2);
		}

		/// <summary>
		/// Strips the <i>stripNumberOfEndingColumns</i> last columns for each tokens. This method can be useful when reading 
		/// a file with gold-standard information in the last <i>stripNumberOfEndingColumns</i> columns and you want to 
		/// parse without gold-standard information.
		/// 
		/// </summary>
		/// <param name="inTokens"> a string array with tokens where each column are separated with a tab-character </param>
		/// <param name="stripNumberOfEndingColumns"> a string array with tokens without the last <i>stripNumberOfEndingColumns</i> columns
		/// @return </param>
		public static string[] stripGold(string[] inTokens, int stripNumberOfEndingColumns)
		{
			string[] outTokens = new string[inTokens.Length];

			for (int i = 0; i < inTokens.Length; i++)
			{
				int tabCounter = 0;
				int j = inTokens[i].Length - 1;
				for (; j >= 0; j--)
				{
					if (inTokens[i][j] == '\t')
					{
						tabCounter++;
					}
					if (tabCounter == stripNumberOfEndingColumns)
					{
						outTokens[i] = inTokens[i].Substring(0, j);
						break;
					}
				}
			}
			return outTokens;
		}

		/// <summary>
		/// Prints a sentence to the Standard-out stream. It expect a string array with tokens.
		/// 
		/// Each token will be one line and after all tokens are printed there will be one empty line marking the ending of sentence.
		/// </summary>
		/// <param name="inTokens"> a string array with tokens </param>
		public static void printTokens(string[] inTokens)
		{
            using (Stream stream = Console.OpenStandardOutput())
            {
                printTokens(inTokens, new StreamWriter(stream));
            }
		}

		/// <summary>
		/// Prints a sentence to a stream. It expect a string array with tokens.
		/// 
		/// Each token will be one line and after all tokens are printed there will be one empty line marking the ending of sentence. </summary>
		/// <param name="inTokens"> a string array with tokens </param>
		/// <param name="stream"> a print stream </param>
		public static void printTokens(string[] inTokens, StreamWriter stream)
        {
            foreach (var t in inTokens)
            {
                stream.WriteLine(t);
            }

            stream.WriteLine();
        }

		/// <summary>
		/// Check if there are difference between two sentences
		/// </summary>
		/// <param name="goldTokens"> the sentence one with an array of tokens </param>
		/// <param name="outputTokens"> the sentence two with an array of tokens </param>
		/// <returns> true, if the sentences differ otherwise false </returns>
		public static bool diffSentences(string[] goldTokens, string[] outputTokens)
		{
			if (goldTokens.Length != outputTokens.Length)
			{
				return true;
			}
			for (int i = 0; i < goldTokens.Length; i++)
			{
				if (!goldTokens[i].Equals(outputTokens[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static void simpleEvaluation(IList<string[]> goldSentences, IList<string[]> parsedSentences, int headColumn, int dependencyLabelColumn, StreamWriter stream)
		{
			if (goldSentences.Count != parsedSentences.Count)
			{
				stream.WriteLine("Number of sentences in gold and output differs");
				return;
			}
			int nTokens = 0;
			int nCorrectHead = 0;
			int nCorrectLabel = 0;
			int nCorrectBoth = 0;

			for (int i = 0; i < goldSentences.Count; i++)
			{
				string[] goldTokens = goldSentences[i];
				string[] parsedTokens = parsedSentences[i];
				if (goldTokens.Length != parsedTokens.Length)
				{
					stream.WriteLine("Number of tokens in gold and output differs in sentence " + i);
					return;
				}
				for (int j = 0; j < goldTokens.Length; j++)
				{
					nTokens++;
					string[] goldColumns = goldTokens[j].Split("\t", true);
					string[] parsedColumns = parsedTokens[j].Split("\t", true);
	//        		System.out.format("%s %s", goldColumns[headColumn],parsedColumns[headColumn]);
					if (goldColumns[headColumn].Equals(parsedColumns[headColumn]))
					{
						nCorrectHead++;
					}
					if (goldColumns[dependencyLabelColumn].Equals(parsedColumns[dependencyLabelColumn]))
					{
						nCorrectLabel++;
					}
					if (goldColumns[headColumn].Equals(parsedColumns[headColumn]) && goldColumns[dependencyLabelColumn].Equals(parsedColumns[dependencyLabelColumn]))
					{
						nCorrectBoth++;
					}
				}
			}
			stream.WriteLine("Labeled   attachment score: {0} / {1} * 100 = {2:##0.00}", nCorrectBoth, nTokens, (float)nCorrectBoth / nTokens * 100.0);
			stream.WriteLine("Unlabeled attachment score: {0} / {1} * 100 = {2:##0.00}", nCorrectHead, nTokens, nCorrectHead / (float)nTokens * 100.0);
			stream.WriteLine("Label accuracy score:       {0} / {1} * 100 = {2:##0.00}", nCorrectLabel, nTokens, nCorrectLabel / (float)nTokens * 100.0);
		}
	}

}