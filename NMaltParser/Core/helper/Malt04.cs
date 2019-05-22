using System;

namespace org.maltparser.core.helper
{
	using Logger = org.apache.log4j.Logger;
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using ColumnDescription = org.maltparser.core.io.dataformat.ColumnDescription;
	using OptionManager = org.maltparser.core.options.OptionManager;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class Malt04
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void loadAllMalt04Tagset(org.maltparser.core.options.OptionManager om, int containerIndex, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler, org.apache.log4j.Logger logger) throws org.maltparser.core.exception.MaltChainedException
		public static void loadAllMalt04Tagset(OptionManager om, int containerIndex, SymbolTableHandler symbolTableHandler, Logger logger)
		{
			string malt04Posset = om.getOptionValue(containerIndex, "malt0.4", "posset").ToString();
			string malt04Cposset = om.getOptionValue(containerIndex, "malt0.4", "cposset").ToString();
			string malt04Depset = om.getOptionValue(containerIndex, "malt0.4", "depset").ToString();
			string nullValueStrategy = om.getOptionValue(containerIndex, "singlemalt", "null_value").ToString();
	//		String rootLabels = om.getOptionValue(containerIndex, "graph", "root_label").toString();
			string inputCharSet = om.getOptionValue(containerIndex, "input", "charset").ToString();
			loadMalt04Posset(malt04Posset, inputCharSet, nullValueStrategy, symbolTableHandler, logger);
			loadMalt04Cposset(malt04Cposset, inputCharSet, nullValueStrategy, symbolTableHandler, logger);
			loadMalt04Depset(malt04Depset, inputCharSet, nullValueStrategy, symbolTableHandler, logger);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void loadMalt04Posset(String fileName, String charSet, String nullValueStrategy, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler, org.apache.log4j.Logger logger) throws org.maltparser.core.exception.MaltChainedException
		public static void loadMalt04Posset(string fileName, string charSet, string nullValueStrategy, SymbolTableHandler symbolTableHandler, Logger logger)
		{
			if (!fileName.ToString().Equals("", StringComparison.OrdinalIgnoreCase))
			{
				if (logger.InfoEnabled)
				{
					logger.info("Loading part-of-speech tagset '" + fileName + "'...\n");
				}
				symbolTableHandler.loadTagset(fileName, "POSTAG", charSet, ColumnDescription.INPUT, ColumnDescription.STRING, nullValueStrategy);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void loadMalt04Cposset(String fileName, String charSet, String nullValueStrategy, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler, org.apache.log4j.Logger logger) throws org.maltparser.core.exception.MaltChainedException
		public static void loadMalt04Cposset(string fileName, string charSet, string nullValueStrategy, SymbolTableHandler symbolTableHandler, Logger logger)
		{
			if (!fileName.Equals("", StringComparison.OrdinalIgnoreCase))
			{
				if (logger.InfoEnabled)
				{
					logger.info("Loading coarse-grained part-of-speech tagset '" + fileName + "'...\n");
				}
				symbolTableHandler.loadTagset(fileName, "CPOSTAG", charSet, ColumnDescription.INPUT, ColumnDescription.STRING, nullValueStrategy);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void loadMalt04Depset(String fileName, String charSet, String nullValueStrategy, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler, org.apache.log4j.Logger logger) throws org.maltparser.core.exception.MaltChainedException
		public static void loadMalt04Depset(string fileName, string charSet, string nullValueStrategy, SymbolTableHandler symbolTableHandler, Logger logger)
		{
			if (!fileName.Equals("", StringComparison.OrdinalIgnoreCase))
			{
				if (logger.InfoEnabled)
				{
					logger.info("Loading dependency type tagset '" + fileName + "'...\n");
				}
				symbolTableHandler.loadTagset(fileName, "DEPREL", charSet, ColumnDescription.DEPENDENCY_EDGE_LABEL, ColumnDescription.STRING, nullValueStrategy);
			}
		}
	}

}