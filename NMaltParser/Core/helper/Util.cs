using System;
using System.Text;
using System.IO;

namespace org.maltparser.core.helper
{


	using  org.apache.log4j;
	using  org.maltparser.core.exception;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class Util
	{
		  private const int BUFFER = 4096;
		  private const char AMP_CHAR = '&';
		  private const char LT_CHAR = '<';
		  private const char GT_CHAR = '>';
		  private const char QUOT_CHAR = '"';
		  private const char APOS_CHAR = '\'';

		  public static string xmlEscape(string str)
		  {
			  bool needEscape = false;
			  char c;
			  for (int i = 0; i < str.Length; i++)
			  {
				  c = str[i];
				  if (c == AMP_CHAR || c == LT_CHAR || c == GT_CHAR || c == QUOT_CHAR || c == APOS_CHAR)
				  {
					  needEscape = true;
					  break;
				  }
			  }
			  if (!needEscape)
			  {
				  return str;
			  }
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			  StringBuilder sb = new StringBuilder();
			  for (int i = 0; i < str.Length; i++)
			  {
				  c = str[i];
				  if (str[i] == AMP_CHAR)
				  {
					  sb.Append("&amp;");
				  }
				  else if (str[i] == LT_CHAR)
				  {
					  sb.Append("&lt;");
				  }
				  else if (str[i] == GT_CHAR)
				  {
					  sb.Append("&gt;");
				  }
				  else if (str[i] == QUOT_CHAR)
				  {
					  sb.Append("&quot;");
				  }
				  else if (str[i] == APOS_CHAR)
				  {
					  sb.Append("&apos;");
				  }
				  else
				  {
					  sb.Append(c);
				  }
			  }
			  return sb.ToString();
		  }

		public static int simpleTicer(Logger logger, long startTime, int nTicxRow, int inTic, int subject)
		{
			logger.info(".");
			int tic = inTic + 1;
			if (tic >= nTicxRow)
			{
				ticInfo(logger, startTime, subject);
				tic = 0;
			}
			return tic;
		}

		public static void startTicer(Logger logger, long startTime, int nTicxRow, int subject)
		{
			logger.info(".");
			for (int i = 1; i <= nTicxRow; i++)
			{
				logger.info(" ");
			}
			ticInfo(logger, startTime, subject);
		}

		public static void endTicer(Logger logger, long startTime, int nTicxRow, int inTic, int subject)
		{
			for (int i = inTic; i <= nTicxRow; i++)
			{
				logger.info(" ");
			}
			ticInfo(logger, startTime, subject);
		}

		private static void ticInfo(Logger logger, long startTime, int subject)
		{
			logger.info("\t");
			int a = 1000000;
			if (subject != 0)
			{
				while (subject / a == 0)
				{
					logger.info(" ");
					a /= 10;
				}
			}
			else
			{
				logger.info("      ");
			}
			logger.info(subject);
			logger.info("\t");
			long time = (DateTimeHelper.CurrentUnixTimeMillis() - startTime) / 1000;
			a = 1000000;
			if (time != 0)
			{
				while (time / a == 0)
				{
					logger.info(" ");
					a /= 10;
				}
				logger.info(time);
				logger.info("s");
			}
			else
			{
				logger.info("      0s");
			}
			logger.info("\t");
			long memory = (Runtime.Runtime.totalMemory() - Runtime.Runtime.freeMemory()) / 1000000;
			a = 1000000;
			if (memory != 0)
			{
				while (memory / a == 0)
				{
					logger.info(" ");
					a /= 10;
				}
				logger.info(memory);
				logger.info("MB\n");
			}
			else
			{
				logger.info("      0MB\n");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copyfile(String source, String destination) throws org.maltparser.core.exception.MaltChainedException
		public static void copyfile(string source, string destination)
		{
			try
			{
				sbyte[] readBuffer = new sbyte[BUFFER];
				BufferedInputStream bis = new BufferedInputStream(new FileStream(source, FileMode.Open, FileAccess.Read));
				BufferedOutputStream bos = new BufferedOutputStream(new FileStream(destination, FileMode.Create, FileAccess.Write), BUFFER);
				int n = 0;
				while ((n = bis.read(readBuffer, 0, BUFFER)) != -1)
				{
					bos.write(readBuffer, 0, n);
				}
				bos.flush();
				bos.close();
				bis.close();
			}
			catch (FileNotFoundException e)
			{
				throw new MaltChainedException("The destination file '" + destination + "' cannot be created when coping the file. ", e);
			}
			catch (IOException e)
			{
				throw new MaltChainedException("The source file '" + source + "' cannot be copied to destination '" + destination + "'. ", e);
			}
		}

		/// <param name="s"> the string to parse for the double value </param>
		/// <exception cref="IllegalArgumentException"> if s is empty or represents NaN or Infinity </exception>
		/// <exception cref="NumberFormatException"> see <seealso cref="Double#parseDouble(String)"/> </exception>
		public static double atof(string s)
		{
			if (string.ReferenceEquals(s, null) || s.Length < 1)
			{
				throw new System.ArgumentException("Can't convert empty string to integer");
			}
			double d = double.Parse(s);
			if (double.IsNaN(d) || double.IsInfinity(d))
			{
				throw new System.ArgumentException("NaN or Infinity in input: " + s);
			}
			return (d);
		}

			/// <param name="s"> the string to parse for the integer value </param>
			/// <exception cref="IllegalArgumentException"> if s is empty </exception>
			/// <exception cref="NumberFormatException"> see <seealso cref="Integer#parseInt(String)"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int atoi(String s) throws NumberFormatException
		public static int atoi(string s)
		{
			if (string.ReferenceEquals(s, null) || s.Length < 1)
			{
				throw new System.ArgumentException("Can't convert empty string to integer");
			}
			// Integer.parseInt doesn't accept '+' prefixed strings
			if (s[0] == '+')
			{
				s = s.Substring(1);
			}
			return int.Parse(s);
		}

		public static void closeQuietly(System.IDisposable c)
		{
			if (c == null)
			{
				return;
			}
			try
			{
				c.Dispose();
			}
			catch (Exception)
			{
			}
		}

		public static double[] copyOf(double[] original, int newLength)
		{
			double[] copy = new double[newLength];
			Array.Copy(original, 0, copy, 0, Math.Min(original.Length, newLength));
			return copy;
		}

		public static int[] copyOf(int[] original, int newLength)
		{
			int[] copy = new int[newLength];
			Array.Copy(original, 0, copy, 0, Math.Min(original.Length, newLength));
			return copy;
		}

		public static bool Equals(double[] a, double[] a2)
		{
			if (a == a2)
			{
				return true;
			}
			if (a == null || a2 == null)
			{
				return false;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = a.length;
			int length = a.Length;
			if (a2.Length != length)
			{
				return false;
			}

			for (int i = 0; i < length; i++)
			{
				if (a[i] != a2[i])
				{
					return false;
				}
			}

			return true;
		}
	}

}