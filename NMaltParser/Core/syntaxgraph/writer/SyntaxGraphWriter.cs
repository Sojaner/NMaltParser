using System.IO;

namespace org.maltparser.core.syntaxgraph.writer
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using DataFormatInstance = org.maltparser.core.io.dataformat.DataFormatInstance;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface SyntaxGraphWriter
	{
		/// <summary>
		/// Opens a file for writing
		/// </summary>
		/// <param name="fileName">	the file name of the file </param>
		/// <param name="charsetName">	the name of the character encoding set </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(String fileName, String charsetName) throws org.maltparser.core.exception.MaltChainedException;
		void open(string fileName, string charsetName);
		/// <summary>
		/// Opens an output stream
		/// </summary>
		/// <param name="os"> an output stream </param>
		/// <param name="charsetName"> the name of the character encoding set </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(java.io.OutputStream os, String charsetName) throws org.maltparser.core.exception.MaltChainedException;
		void open(Stream os, string charsetName);
		/// <summary>
		/// Cause the syntax graph writer to write the beginning of the file (such as header information)
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeProlog() throws org.maltparser.core.exception.MaltChainedException;
		void writeProlog();
		/// <summary>
		/// Writes a sentence (token structure, dependency structure or/and phrase structure)
		/// </summary>
		/// <param name="syntaxGraph"> a syntax graph (token structure, dependency structure or/and phrase structure) </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeSentence(org.maltparser.core.syntaxgraph.TokenStructure syntaxGraph) throws org.maltparser.core.exception.MaltChainedException;
		void writeSentence(TokenStructure syntaxGraph);
		/// <summary>
		/// Writes the end of the file 
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeEpilog() throws org.maltparser.core.exception.MaltChainedException;
		void writeEpilog();
		/// <summary>
		/// Returns the output data format instance
		/// </summary>
		/// <returns> the output data format instance </returns>
		DataFormatInstance DataFormatInstance {get;set;}
		/// <summary>
		/// Returns a string representation of the writer specific options.
		/// </summary>
		/// <returns> a string representation of the writer specific options. </returns>
		string Options {get;set;}
		/// <summary>
		/// Sets the writer specific options.
		/// </summary>
		/// <param name="optionString"> a string representation of the writer specific options </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setOptions(String optionString) throws org.maltparser.core.exception.MaltChainedException;
		/// <summary>
		/// Closes the file or the output stream.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws org.maltparser.core.exception.MaltChainedException;
		void close();
	}

}