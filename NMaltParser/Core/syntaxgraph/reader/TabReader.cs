using System.Collections.Generic;
using System.IO;

namespace org.maltparser.core.syntaxgraph.reader
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.syntaxgraph.edge;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class TabReader : SyntaxGraphReader
	{
		private StreamReader reader;
		private int sentenceCount;
	//	private final StringBuilder input;
		private DataFormatInstance dataFormatInstance;
		private const string IGNORE_COLUMN_SIGN = "_";
	//	private static final char TAB = '\t';
	//	private static final char NEWLINE = '\n';
	//	private static final char CARRIAGE_RETURN = '\r';
		private string fileName = null;
		private URL url = null;
		private string charsetName;
		private int nIterations;
		private int cIterations;
		private bool closeStream = true;

		public TabReader()
		{
	//		input = new StringBuilder();
			nIterations = 1;
			cIterations = 1;
		}

	//	private void reopen() throws MaltChainedException {
	//		close();
	//		if (fileName != null) {
	//			open(fileName, charsetName);
	//		} else if (url != null) {
	//			open(url, charsetName);
	//		} else {
	//			throw new DataFormatException("The input stream cannot be reopen. ");
	//		}
	//	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(String fileName, String charsetName) throws org.maltparser.core.exception.MaltChainedException
		public virtual void open(string fileName, string charsetName)
		{
			FileName = fileName;
			CharsetName = charsetName;
			try
			{
				open(new FileStream(fileName, FileMode.Open, FileAccess.Read), charsetName);
			}
			catch (FileNotFoundException e)
			{
				throw new DataFormatException("The input file '" + fileName + "' cannot be found. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(java.net.URL url, String charsetName) throws org.maltparser.core.exception.MaltChainedException
		public virtual void open(URL url, string charsetName)
		{
			Url = url;
			CharsetName = charsetName;
			if (url == null)
			{
				throw new DataFormatException("The input file cannot be found. ");
			}
			try
			{
				open(url.openStream(), charsetName);
			}
			catch (IOException e)
			{
				throw new DataFormatException("The URL '" + url.ToString() + "' cannot be opened. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(java.io.InputStream is, String charsetName) throws org.maltparser.core.exception.MaltChainedException
		public virtual void open(Stream @is, string charsetName)
		{
			try
			{
				if (@is == System.in)
				{
					closeStream = false;
				}
				open(new StreamReader(@is, charsetName));
			}
			catch (UnsupportedEncodingException e)
			{
				throw new DataFormatException("The character encoding set '" + charsetName + "' isn't supported. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void open(java.io.InputStreamReader isr) throws org.maltparser.core.exception.MaltChainedException
		private void open(StreamReader isr)
		{
			Reader = new StreamReader(isr);
			SentenceCount = 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readProlog() throws org.maltparser.core.exception.MaltChainedException
		public virtual void readProlog()
		{

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean readSentence(org.maltparser.core.syntaxgraph.TokenStructure syntaxGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool readSentence(TokenStructure syntaxGraph)
		{
			if (syntaxGraph == null || dataFormatInstance == null)
			{
				return false;
			}
			syntaxGraph.clear();
			syntaxGraph.SymbolTables.cleanUp();
			Element node = null;
			Edge edge = null;


			List<string> tokens = new List<string>();
			try
			{
				string line;
				while (!string.ReferenceEquals((line = reader.ReadLine()), null))
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
			}
			catch (IOException e)
			{
				close();
				throw new DataFormatException("Error when reading from the input file. ", e);
			}

			int terminalCounter = 0;
			for (int i = 0; i < tokens.Count; i++)
			{
				string token = tokens[i];

				if (token[0] == '#')
				{
					syntaxGraph.addComment(token, terminalCounter + 1);
					continue;
				}
				string[] columns = token.Split("\t", true);
				if (columns[0].Contains("-") || columns[0].Contains("."))
				{
					syntaxGraph.addComment(token, terminalCounter + 1);
					continue;
				}
				terminalCounter++;
				node = syntaxGraph.addTokenNode(terminalCounter);

				IEnumerator<ColumnDescription> columnDescriptions = dataFormatInstance.GetEnumerator();
				for (int j = 0; j < columns.Length; j++)
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					ColumnDescription columnDescription = columnDescriptions.next();

					if (columnDescription.Category == ColumnDescription.INPUT && node != null)
					{
						syntaxGraph.addLabel(node, columnDescription.Name, columns[j]);
					}
					else if (columnDescription.Category == ColumnDescription.HEAD)
					{
						if (syntaxGraph is DependencyStructure)
						{
							if (columnDescription.Category != ColumnDescription.IGNORE && !columns[j].Equals(IGNORE_COLUMN_SIGN))
							{
								edge = ((DependencyStructure)syntaxGraph).addDependencyEdge(int.Parse(columns[j]), terminalCounter);
							}
						}
						else
						{
							close();
							throw new DataFormatException("The input graph is not a dependency graph and therefore it is not possible to add dependncy edges. ");
						}
					}
					else if (columnDescription.Category == ColumnDescription.DEPENDENCY_EDGE_LABEL && edge != null)
					{
						syntaxGraph.addLabel(edge, columnDescription.Name, columns[j]);
					}
				}
			}

			if (!syntaxGraph.hasTokens())
			{
				return false;
			}
			sentenceCount++;
			return true;
		}

	//	public boolean readSentence(TokenStructure syntaxGraph) throws MaltChainedException  {
	//		if (syntaxGraph == null || dataFormatInstance == null) {
	//			return false;
	//		}
	//
	//		Element node = null;
	//		Edge edge = null;
	//		input.setLength(0);
	//		int i = 0;
	//		int terminalCounter = 0;
	//		int nNewLines = 0;
	//		syntaxGraph.clear();
	//		syntaxGraph.getSymbolTables().cleanUp();
	//		Iterator<ColumnDescription> columns = dataFormatInstance.iterator();
	//		while (true) {
	//			int c;
	//
	//			try {
	//				c = reader.read();
	//			} catch (IOException e) {
	//				close();
	//				throw new DataFormatException("Error when reading from the input file. ", e);
	//			}
	//			if (c == TAB || c == NEWLINE || c == CARRIAGE_RETURN || c == -1) {
	//				if (input.length() != 0) {
	//					if (i == 0) {
	//						terminalCounter++;
	//						node = syntaxGraph.addTokenNode(terminalCounter);
	//					}
	//					if (columns.hasNext()) {
	//						ColumnDescription column = columns.next();
	//						if (column.getCategory() == ColumnDescription.INPUT && node != null) {
	//							syntaxGraph.addLabel(node, column.getName(), input.toString());
	//						} else if (column.getCategory() == ColumnDescription.HEAD) {
	//							if (syntaxGraph instanceof DependencyStructure) {
	//								if (column.getCategory() != ColumnDescription.IGNORE && !input.toString().equals(IGNORE_COLUMN_SIGN)) {
	////								if (column.getType() != ColumnDescription.IGNORE && !input.toString().equals(IGNORE_COLUMN_SIGN)) { // bugfix
	//								//if (!input.toString().equals(IGNORE_COLUMN_SIGN)) {
	//									edge = ((DependencyStructure)syntaxGraph).addDependencyEdge(Integer.parseInt(input.toString()), terminalCounter);
	//								}
	//							}
	//							else {
	//								close();
	//								throw new DataFormatException("The input graph is not a dependency graph and therefore it is not possible to add dependncy edges. ");
	//							}
	//						} else if (column.getCategory() == ColumnDescription.DEPENDENCY_EDGE_LABEL && edge != null) {
	//							//if (column.getType() != ColumnDescription.IGNORE && !input.toString().equals(IGNORE_COLUMN_SIGN)) { // bugfix not working for everybody
	//								syntaxGraph.addLabel(edge, column.getName(), input.toString());
	//							//} // bugfix
	//						}
	//					}
	//					input.setLength(0);
	//					nNewLines = 0;
	//					i++;
	//				} else if (c == TAB) {
	//					throw new MaltChainedException("The input file '"+fileName+"' contains a column where the value is an empty string. Please check your input file. ");
	//				}
	//				if (c == NEWLINE) {
	//					nNewLines++;
	//					i = 0;
	//					columns = dataFormatInstance.iterator();
	//				}
	//			} else {
	//				input.append((char)c);
	//			}
	//
	//			if (nNewLines == 2 && c == NEWLINE) {
	//				if (syntaxGraph.hasTokens()) {
	//					sentenceCount++;
	//				}
	//				return true;
	//			} else if (c == -1) {
	//				if (syntaxGraph.hasTokens()) {
	//					sentenceCount++;
	//				}
	//				if (cIterations < nIterations) {
	//					cIterations++;
	//					reopen();
	//					return true;
	//				}
	//
	//				return false;
	//			}
	//		}
	//	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readEpilog() throws org.maltparser.core.exception.MaltChainedException
		public virtual void readEpilog()
		{

		}

		public virtual StreamReader Reader
		{
			get
			{
				return reader;
			}
			set
			{
				close();
				this.reader = value;
			}
		}


		public virtual DataFormatInstance DataFormatInstance
		{
			get
			{
				return dataFormatInstance;
			}
			set
			{
				this.dataFormatInstance = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getSentenceCount() throws org.maltparser.core.exception.MaltChainedException
		public virtual int SentenceCount
		{
			get
			{
				return sentenceCount;
			}
			set
			{
				this.sentenceCount = value;
			}
		}


		public virtual string Options
		{
			get
			{
				return null;
			}
			set
			{
    
			}
		}


		public virtual string FileName
		{
			get
			{
				return fileName;
			}
			set
			{
				this.fileName = value;
			}
		}


		public virtual URL Url
		{
			get
			{
				return url;
			}
			set
			{
				this.url = value;
			}
		}


		public virtual string CharsetName
		{
			get
			{
				return charsetName;
			}
			set
			{
				this.charsetName = value;
			}
		}


		public virtual int NIterations
		{
			get
			{
				return nIterations;
			}
			set
			{
				nIterations = value;
			}
		}


		public virtual int IterationCounter
		{
			get
			{
				return cIterations;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws org.maltparser.core.exception.MaltChainedException
		public virtual void close()
		{
			try
			{
				if (reader != null)
				{
					if (closeStream)
					{
						reader.Close();
					}
					reader = null;
				}
			}
			catch (IOException e)
			{
				throw new DataFormatException("Error when closing the input file. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public virtual void clear()
		{
			close();
	//		input.setLength(0);
			dataFormatInstance = null;
			sentenceCount = 0;
		}
	}

}