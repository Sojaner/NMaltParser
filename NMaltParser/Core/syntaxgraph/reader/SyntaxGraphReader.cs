using System.IO;

namespace org.maltparser.core.syntaxgraph.reader
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.io.dataformat;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface SyntaxGraphReader
	{
		/// <summary>
		/// Opens a file for read only
		/// </summary>
		/// <param name="fileName">	the file name of the file </param>
		/// <param name="charsetName">	the name of the character encoding set </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(String fileName, String charsetName) throws org.maltparser.core.exception.MaltChainedException;
		void open(string fileName, string charsetName);
		/// <summary>
		/// Opens an URL for read only
		/// </summary>
		/// <param name="url"> the URL of the resource </param>
		/// <param name="charsetName"> the name of the character encoding set </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(java.net.URL url, String charsetName) throws org.maltparser.core.exception.MaltChainedException;
		void open(URL url, string charsetName);
		/// <summary>
		/// Opens an input stream
		/// </summary>
		/// <param name="is"> an input stream </param>
		/// <param name="charsetName"> the name of the character encoding set </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(java.io.InputStream is, String charsetName) throws org.maltparser.core.exception.MaltChainedException;
		void open(Stream @is, string charsetName);
		/// <summary>
		/// Cause the syntax graph reader to read the beginning of the file (such as header information)
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readProlog() throws org.maltparser.core.exception.MaltChainedException;
		void readProlog();

		/// <summary>
		/// Reads a sentence (token structure, dependency structure or/and phrase structure)
		/// </summary>
		/// <param name="syntaxGraph"> a syntax graph (token structure, dependency structure or/and phrase structure) </param>
		/// <returns> true if there is more sentences to be processed, otherwise false. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean readSentence(org.maltparser.core.syntaxgraph.TokenStructure syntaxGraph) throws org.maltparser.core.exception.MaltChainedException;
		bool readSentence(TokenStructure syntaxGraph);
		/// <summary>
		/// Reads the end of the file, after all sentences have been processed, 
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readEpilog() throws org.maltparser.core.exception.MaltChainedException;
		void readEpilog();
		/// <summary>
		/// Returns the current number of the sentence.
		/// </summary>
		/// <returns> the current number of the sentence. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getSentenceCount() throws org.maltparser.core.exception.MaltChainedException;
		int SentenceCount {get;}
		/// <summary>
		/// Returns the input data format instance
		/// </summary>
		/// <returns> the input data format instance </returns>
		DataFormatInstance DataFormatInstance {get;set;}
		/// <summary>
		/// Returns a string representation of the reader specific options.
		/// </summary>
		/// <returns> a string representation of the reader specific options. </returns>
		string Options {get;set;}
		/// <summary>
		/// Sets the reader specific options.
		/// </summary>
		/// <param name="optionString"> a string representation of the reader specific options </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setOptions(String optionString) throws org.maltparser.core.exception.MaltChainedException;
		/// <summary>
		/// Closes the file or the input stream.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws org.maltparser.core.exception.MaltChainedException;
		void close();

		int NIterations {get;set;}
		int IterationCounter {get;}
	}

}