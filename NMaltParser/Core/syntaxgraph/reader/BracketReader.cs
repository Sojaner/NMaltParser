using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.core.syntaxgraph.reader
{

	using  exception;
	using  io.dataformat;
    using  symbol;
	using  edge;
	using  node;

    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class BracketReader : SyntaxGraphReader
	{
		private StreamReader reader;
		private DataFormatInstance dataFormatInstance;
		private int sentenceCount;
		private StringBuilder input;
		private int terminalCounter;
		private int nonTerminalCounter;
		private string optionString;
		private SortedDictionary<string, ColumnDescription> inputColumns;
		private SortedDictionary<string, ColumnDescription> edgeLabelColumns;
		private SortedDictionary<string, ColumnDescription> phraseLabelColumns;

		private string fileName = null;
		private URL url = null;
		private string charsetName;
		private int nIterations;
		private int cIterations;
		private bool closeStream = true;

		private char STARTING_BRACKET = '(';
		private char CLOSING_BRACKET = ')';
		private char INPUT_SEPARATOR = ' ';
		private char EDGELABEL_SEPARATOR = '-';
		private char SENTENCE_SEPARATOR = '\n';
		private char BLANK = ' ';
		private char CARRIAGE_RETURN = '\r';
		private char TAB = '\t';

		public BracketReader()
		{
			input = new StringBuilder();
			nIterations = 1;
			cIterations = 1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void reopen() throws org.maltparser.core.exception.MaltChainedException
		private void reopen()
		{
			close();
			if (!ReferenceEquals(fileName, null))
			{
				open(fileName, charsetName);
			}
			else if (url != null)
			{
				open(url, charsetName);
			}
			else
			{
				throw new DataFormatException("The input stream cannot be reopen. ");
			}
		}

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
			int brackets = 0;
			try
			{
				int l = reader.Read();
				char c;
				input.Length = 0;

				while (true)
				{
					if (l == -1)
					{
						input.Length = 0;
						return false;
					}

					c = (char)l;
					l = reader.Read();

					if (c == SENTENCE_SEPARATOR || c == CARRIAGE_RETURN || c == TAB || c == (char) - 1)
					{

					}
					else if (c == STARTING_BRACKET)
					{
						input.Append(c);
						brackets++;
					}
					else if (c == CLOSING_BRACKET)
					{
						input.Append(c);
						brackets--;
					}
					else if (c == INPUT_SEPARATOR)
					{
						if ((char)l != STARTING_BRACKET && (char)l != CLOSING_BRACKET && (char)l != INPUT_SEPARATOR && (char)l != SENTENCE_SEPARATOR && (char)l != CARRIAGE_RETURN && (char)l != TAB && l != -1)
						{
							input.Append(c);
						}
					// Start BracketProgLangReader
					}
					else if (c == '\\')
					{
						c = (char) l;
						l = reader.Read();
						if (c != ' ' && c != '(' && c != ')' && c != '\\' && c != 'n' && c != 'r' && c != 't' && c != '\"' && c != '\'')
						{
	//						System.out.println("Error");
							Environment.Exit(1);
						}
						else
						{
							input.Append("\\" + c);
						}
					// End BracketProgLangReader
					}
					else if (brackets != 0)
					{
						input.Append(c);
					}
					if (brackets == 0 && input.Length != 0)
					{
						sentenceCount++;
						terminalCounter = 1;
						nonTerminalCounter = 1;
						if (syntaxGraph is PhraseStructure)
						{
							bracketing((PhraseStructure)syntaxGraph, 0, input.Length, null);
							if (syntaxGraph is MappablePhraseStructureGraph)
							{
								((MappablePhraseStructureGraph)syntaxGraph).Mapping.updateDependenyGraph(((MappablePhraseStructureGraph)syntaxGraph), ((PhraseStructure)syntaxGraph).PhraseStructureRoot);
							}
						}
						return true;
					}

					if (c == (char) - 1)
					{
						if (brackets != 0)
						{
							close();
							throw new MaltChainedException("Error when reading from the input file. ");
						}
						if (cIterations < nIterations)
						{
							cIterations++;
							reopen();
							return true;
						}
						return false;
					}
				}
			}
			catch (IOException e)
			{
				close();
				throw new MaltChainedException("Error when reading from the input file. ", e);
			}

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void bracketing(org.maltparser.core.syntaxgraph.PhraseStructure phraseStructure, int start, int end, org.maltparser.core.syntaxgraph.node.PhraseStructureNode parent) throws org.maltparser.core.exception.MaltChainedException
		private void bracketing(PhraseStructure phraseStructure, int start, int end, PhraseStructureNode parent)
		{
			int bracketsdepth = 0;
			int startpos = start - 1;
			for (int i = start, n = end; i < n; i++)
			{
				if (input[i] == STARTING_BRACKET && (i == 0 || input[i - 1] != '\\'))
				{
					if (bracketsdepth == 0)
					{
						startpos = i;
					}
					bracketsdepth++;
				}
				else if (input[i] == CLOSING_BRACKET && (i == 0 || input[i - 1] != '\\'))
				{
					bracketsdepth--;
					if (bracketsdepth == 0)
					{
						extract(phraseStructure, startpos + 1, i, parent);
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void extract(org.maltparser.core.syntaxgraph.PhraseStructure phraseStructure, int begin, int end, org.maltparser.core.syntaxgraph.node.PhraseStructureNode parent) throws org.maltparser.core.exception.MaltChainedException
		private void extract(PhraseStructure phraseStructure, int begin, int end, PhraseStructureNode parent)
		{
			SymbolTableHandler symbolTables = phraseStructure.SymbolTables;
			int index = -1;
			for (int i = begin; i < end; i++)
			{
				if (input[i] == STARTING_BRACKET && (i == begin || input[i - 1] != '\\'))
				{
					index = i;
					break;
				}
			}
			if (index == -1)
			{
				TokenNode t = phraseStructure.addTokenNode(terminalCounter);
				if (t == null)
				{
					close();
					throw new MaltChainedException("Bracket Reader error: could not create a terminal node. ");
				}

				terminalCounter++;
				Edge e = null;

				if (parent != null)
				{
					e = phraseStructure.addPhraseStructureEdge(parent, (PhraseStructureNode)t);
				}
				else
				{
					close();
					throw new MaltChainedException("Bracket Reader error: could not find the parent node. ");
				}

				int start = begin;

				IEnumerator<string> inputColumnsIterator = inputColumns.Keys.GetEnumerator();
				IEnumerator<string> edgeLabelsColumnsIterator = edgeLabelColumns.Keys.GetEnumerator();
				bool noneNode = false;
				bool edgeLabels = false;
				for (int i = begin; i < end; i++)
				{
					if (input[i] == EDGELABEL_SEPARATOR || (input[i] == INPUT_SEPARATOR && (i == begin || input[i - 1] != '\\')) || i == end - 1)
					{
						if (i == begin && input[i] == EDGELABEL_SEPARATOR)
						{
							noneNode = true;
						}
						else if (start == begin)
						{
							if ((noneNode && input[i] != EDGELABEL_SEPARATOR) || !noneNode)
							{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								if (inputColumnsIterator.hasNext())
								{

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
									t.addLabel(symbolTables.getSymbolTable(inputColumns[inputColumnsIterator.next()].Name), decodeString((i == end - 1)?input.substring(start, end - start):input.substring(start, i - start)));
								}
								start = i + 1;
								if (input[i] == EDGELABEL_SEPARATOR)
								{
									edgeLabels = true;
								}
							}
						}
						else if (edgeLabels && e != null)
						{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							if (edgeLabelsColumnsIterator.hasNext())
							{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								e.addLabel(symbolTables.getSymbolTable(edgeLabelColumns[edgeLabelsColumnsIterator.next()].Name), (i == end - 1)?input.substring(start, end - start):input.substring(start, i - start));
							}
							start = i + 1;
							if (input[i] == INPUT_SEPARATOR && (i == begin || input[i - 1] != '\\'))
							{
								edgeLabels = false;
							}
						}
						else if (input[i] == EDGELABEL_SEPARATOR && i != end - 1 && (input[i + 1] != INPUT_SEPARATOR && (i == begin || input[i - 1] != '\\')))
						{
						}
						else
						{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							if (inputColumnsIterator.hasNext())
							{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								t.addLabel(symbolTables.getSymbolTable(inputColumns[inputColumnsIterator.next()].Name), (i == end - 1)?input.substring(start, end - start):input.substring(start, i - start));
							}
							start = i + 1;
						}
					}
				}
			}
			else
			{
				PhraseStructureNode nt;
				Edge e = null;
				if (parent == null)
				{
					nt = phraseStructure.PhraseStructureRoot;
				}
				else
				{
					nt = phraseStructure.addNonTerminalNode(nonTerminalCounter);
					if (nt == null)
					{
						close();
						throw new MaltChainedException("Bracket Reader error: could not create a nonterminal node. ");
					}
					nonTerminalCounter++;

					e = phraseStructure.addPhraseStructureEdge(parent, nt);
				}
				IEnumerator<string> phraseLabelColumnsIterator = phraseLabelColumns.Keys.GetEnumerator();
				IEnumerator<string> edgeLabelsColumnsIterator = edgeLabelColumns.Keys.GetEnumerator();
				int newbegin = begin;
				int start = begin;

				for (int i = begin; i < index; i++)
				{
					if (input[i] == EDGELABEL_SEPARATOR || i == index - 1)
					{
						if (start == newbegin)
						{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							if (phraseLabelColumnsIterator.hasNext())
							{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								nt.addLabel(symbolTables.getSymbolTable(phraseLabelColumns[phraseLabelColumnsIterator.next()].Name), (i == index - 1)?input.substring(start, index - start):input.substring(start, i - start));
							}
							start = i + 1;
						}
						else if (e != null)
						{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							if (edgeLabelsColumnsIterator.hasNext())
							{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								e.addLabel(symbolTables.getSymbolTable(edgeLabelColumns[edgeLabelsColumnsIterator.next()].Name), (i == index - 1)?input.substring(start, index - start):input.substring(start, i - start));
							}
							start = i + 1;
						}
					}
					else if (input[i] == BLANK)
					{
						start++;
						newbegin++;
					}
				}

				bracketing(phraseStructure, index, end, nt);
			}
		}

		private string decodeString(string @string)
		{
			return @string.Replace("\\(", "(").Replace("\\)", ")").Replace("\\ ", " ");
		}

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
				reader = value;
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
				sentenceCount = value;
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
				dataFormatInstance = value;
				inputColumns = dataFormatInstance.InputColumnDescriptions;
				edgeLabelColumns = dataFormatInstance.PhraseStructureEdgeLabelColumnDescriptions;
				phraseLabelColumns = dataFormatInstance.PhraseStructureNodeLabelColumnDescriptions;
			}
		}


		public virtual string Options
		{
			get
			{
				return optionString;
			}
			set
			{
				optionString = value;
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
				fileName = value;
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
				url = value;
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
				charsetName = value;
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
				throw new DataFormatException("Error when closing the input file.", e);
			}
		}
	}

}