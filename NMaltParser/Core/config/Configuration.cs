using System.IO;

namespace org.maltparser.core.config
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface Configuration
	{
	//	public ConfigurationDir getConfigurationDir();
	//	public void setConfigurationDir(ConfigurationDir configDir);
	//	public Logger getConfigLogger(); 
	//	public void setConfigLogger(Logger logger); 
		bool LoggerInfoEnabled {get;}
		bool LoggerDebugEnabled {get;}
		void logErrorMessage(string message);
		void logInfoMessage(string message);
		void logInfoMessage(char character);
		void logDebugMessage(string message);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeInfoToConfigFile(String message) throws org.maltparser.core.exception.MaltChainedException;
		void writeInfoToConfigFile(string message);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.OutputStreamWriter getOutputStreamWriter(String fileName) throws org.maltparser.core.exception.MaltChainedException;
		StreamWriter getOutputStreamWriter(string fileName);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.OutputStreamWriter getAppendOutputStreamWriter(String fileName) throws org.maltparser.core.exception.MaltChainedException;
		StreamWriter getAppendOutputStreamWriter(string fileName);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStreamReader getInputStreamReader(String fileName) throws org.maltparser.core.exception.MaltChainedException;
		StreamReader getInputStreamReader(string fileName);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream getInputStreamFromConfigFileEntry(String fileName) throws org.maltparser.core.exception.MaltChainedException;
		Stream getInputStreamFromConfigFileEntry(string fileName);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL getConfigFileEntryURL(String fileName) throws org.maltparser.core.exception.MaltChainedException;
		URL getConfigFileEntryURL(string fileName);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.File getFile(String fileName) throws org.maltparser.core.exception.MaltChainedException;
		File getFile(string fileName);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getConfigFileEntryObject(String fileName) throws org.maltparser.core.exception.MaltChainedException;
		object getConfigFileEntryObject(string fileName);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getConfigFileEntryString(String fileName) throws org.maltparser.core.exception.MaltChainedException;
		string getConfigFileEntryString(string fileName);
		SymbolTableHandler SymbolTables {get;}
	//	public ConfigurationRegistry getRegistry();
	//	public void addRegistry(Class<?> clazz, Object o);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getOptionValue(String optiongroup, String optionname) throws org.maltparser.core.exception.MaltChainedException;
		object getOptionValue(string optiongroup, string optionname);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getOptionValueString(String optiongroup, String optionname) throws org.maltparser.core.exception.MaltChainedException;
		string getOptionValueString(string optiongroup, string optionname);
	}

}