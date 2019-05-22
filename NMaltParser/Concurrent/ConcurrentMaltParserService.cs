using NMaltParser.Core.Exception;
using NMaltParser.Core.LW.Helper;
using NMaltParser.Core.Options;

namespace NMaltParser.Concurrent
{
    /// <summary>
	/// The purpose of ConcurrentMaltParserService is to provide an interface to MaltParser that makes it easier to parse sentences from 
	/// other programs. ConcurrentMaltParserService is hopefully thread-safe and can be used in a multi threaded environment. This class 
	/// replace the old interface org.maltparser.MaltParserService when you want to load a MaltParser model and then use it 
	/// to parse sentences in both a single threaded or multi threaded application. 
	/// 
	/// How to use ConcurrentMaltParserService, please see the examples provided in the directory 'examples/apiexamples/srcex'
	/// 
	/// @author Johan Hall
	/// </summary>
	public sealed class ConcurrentMaltParserService
	{
		private static int optionContainerCounter = 0;


		private static int NextOptionContainerCounter
		{
			get
			{
				lock (typeof(ConcurrentMaltParserService))
				{
					return optionContainerCounter++;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static synchronized void loadOptions() throws org.maltparser.core.exception.MaltChainedException
		private static void loadOptions()
		{
			lock (typeof(ConcurrentMaltParserService))
			{
				if (!OptionManager.instance().hasOptions())
				{
					OptionManager.instance().loadOptionDescriptionFile();
					OptionManager.instance().generateMaps();
				}
			}
		}

		/// <summary>
		/// Initialize a MaltParser model from a MaltParser model file (.mco)
		/// </summary>
		/// <param name="mcoFile"> File object containing the path to a valid MaltParser model file. Usually the file extension is ".mco" </param>
		/// <returns> MaltParser model </returns>
		/// <exception cref="MaltChainedException"> </exception>
		/// <exception cref="MalformedURLException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static ConcurrentMaltParserModel initializeParserModel(java.io.File mcoFile) throws org.maltparser.core.exception.MaltChainedException, java.net.MalformedURLException
		public static ConcurrentMaltParserModel initializeParserModel(File mcoFile)
		{
			return initializeParserModel(mcoFile.toURI().toURL());
		}

		/// <summary>
		/// Initialize a MaltParser model from a MaltParser model file (.mco)
		/// </summary>
		/// <param name="mcoURL"> URL to a valid MaltParser model file. Usually the file extension is ".mco" </param>
		/// <returns> a concurrent MaltParser model </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static ConcurrentMaltParserModel initializeParserModel(java.net.URL mcoURL) throws org.maltparser.core.exception.MaltChainedException
		public static ConcurrentMaltParserModel initializeParserModel(URL mcoURL)
		{
			loadOptions();
			int optionContainer = NextOptionContainerCounter;
			string parserModelName = Utils.getInternalParserModelName(mcoURL);
			OptionManager.instance().parseCommandLine("-m parse", optionContainer);
			OptionManager.instance().loadOptions(optionContainer, Utils.getInputStreamReaderFromConfigFileEntry(mcoURL, parserModelName, "savedoptions.sop", "UTF-8"));
			return new ConcurrentMaltParserModel(optionContainer, mcoURL);
		}
	}

}