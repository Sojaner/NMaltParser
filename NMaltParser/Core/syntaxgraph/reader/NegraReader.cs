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
	public class NegraReader : SyntaxGraphReader
	{
		private enum NegraTables
		{
			ORIGIN,
			EDITOR,
			WORDTAG,
			MORPHTAG,
			NODETAG,
			EDGETAG,
			SECEDGETAG,
			SENTENCE,
			UNDEF
		}
		private StreamReader reader;
		private DataFormatInstance dataFormatInstance;
		private int sentenceCount;
		private string optionString;
		private int formatVersion;
		private NegraTables currentHeaderTable;
		private int currentTerminalSize;
		private int currentNonTerminalSize;
		private SortedDictionary<int, PhraseStructureNode> nonterminals;
		private StringBuilder edgelabelSymbol;
		private StringBuilder edgelabelTableName;
		private int START_ID_OF_NONTERMINALS = 500;
		private string fileName = null;
		private URL url = null;
		private string charsetName;
		private int nIterations;
		private int cIterations;
		private bool closeStream = true;

		public NegraReader()
		{
			currentHeaderTable = NegraTables.UNDEF;
			edgelabelSymbol = new StringBuilder();
			edgelabelTableName = new StringBuilder();
			nonterminals = new SortedDictionary<int, PhraseStructureNode>();
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
			if (syntaxGraph == null || !(syntaxGraph is PhraseStructure))
			{
				return false;
			}
			syntaxGraph.clear();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.PhraseStructure phraseStructure = (org.maltparser.core.syntaxgraph.PhraseStructure)syntaxGraph;
			PhraseStructure phraseStructure = (PhraseStructure)syntaxGraph;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.symbol.SymbolTableHandler symbolTables = phraseStructure.getSymbolTables();
			SymbolTableHandler symbolTables = phraseStructure.SymbolTables;
			PhraseStructureNode parent = null;
			PhraseStructureNode child = null;
			currentHeaderTable = NegraTables.UNDEF;
			string line = null;
			syntaxGraph.clear();
			nonterminals.Clear();
			try
			{
				while (true)
				{
					line = reader.ReadLine();
					if (ReferenceEquals(line, null))
					{
						if (syntaxGraph.hasTokens())
						{
							sentenceCount++;
							if (syntaxGraph is MappablePhraseStructureGraph)
							{
								((MappablePhraseStructureGraph)syntaxGraph).Mapping.updateDependenyGraph(((MappablePhraseStructureGraph)syntaxGraph), ((PhraseStructure)syntaxGraph).PhraseStructureRoot);
							}
						}
						if (cIterations < nIterations)
						{
							cIterations++;
							reopen();
							return true;
						}
						return false;
					}
					else if (line.StartsWith("#EOS", StringComparison.Ordinal))
					{
						currentTerminalSize = 0;
						currentNonTerminalSize = 0;
						currentHeaderTable = NegraTables.UNDEF;
						if (syntaxGraph is MappablePhraseStructureGraph)
						{
							((MappablePhraseStructureGraph)syntaxGraph).Mapping.updateDependenyGraph(((MappablePhraseStructureGraph)syntaxGraph), ((PhraseStructure)syntaxGraph).PhraseStructureRoot);
						}
						return true;
					}
					else if (line.StartsWith("#BOS", StringComparison.Ordinal))
					{
						currentHeaderTable = NegraTables.SENTENCE;
						int s = -1, e = -1;
						for (int i = 5, n = line.Length; i < n; i++)
						{
							if (char.IsDigit(line[i]) && s == -1)
							{
								s = i;
							}
							if (line[i] == ' ')
							{
								e = i;
								break;
							}
						}
						if (s != e && s != -1 && e != -1)
						{
							phraseStructure.SentenceID = int.Parse(line.Substring(s, e - s));
						}
						sentenceCount++;
					}
					else if (currentHeaderTable == NegraTables.SENTENCE)
					{
						if (line.Length >= 2 && line[0] == '#' && char.IsDigit(line[1]))
						{ // Non-terminal
							IEnumerator<ColumnDescription> columns = dataFormatInstance.GetEnumerator();
							ColumnDescription column = null;
							currentNonTerminalSize++;
							char[] lineChars = line.ToCharArray();
							int start = 0;
							int secedgecounter = 0;
							for (int i = 0, n = lineChars.Length; i < n; i++)
							{
								if (lineChars[i] == '\t' && start == i)
								{
									start++;
								}
								else if (lineChars[i] == '\t' || i == n - 1)
								{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
									if (columns.hasNext())
									{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
										column = columns.next();
									}
									if (column.Position == 0)
									{
										int index = int.Parse((i == n - 1)?line.Substring(start + 1):line.Substring(start + 1, i - (start + 1)));
										child = nonterminals[index];
										if (child == null)
										{
											if (index != 0)
											{
												child = ((PhraseStructure)syntaxGraph).addNonTerminalNode(index - START_ID_OF_NONTERMINALS + 1);
											}
											nonterminals[index] = child;
										}
									}
									else if (column.Position == 2 && child != null)
									{
										syntaxGraph.addLabel(child, "CAT", (i == n - 1)?line.Substring(start):line.Substring(start, i - start));
									}
									else if (column.Category == ColumnDescription.PHRASE_STRUCTURE_EDGE_LABEL)
									{
										edgelabelSymbol.Length = 0;
										edgelabelSymbol.Append((i == n - 1)?line.Substring(start):line.Substring(start, i - start));
										edgelabelTableName.Length = 0;
										edgelabelTableName.Append(column.Name);
									}
									else if (column.Category == ColumnDescription.PHRASE_STRUCTURE_NODE_LABEL && child != null)
									{
										int index = int.Parse((i == n - 1)?line.Substring(start):line.Substring(start, i - start));
										parent = nonterminals[index];
										if (parent == null)
										{
											if (index == 0)
											{
												parent = phraseStructure.PhraseStructureRoot;
											}
											else
											{
												parent = phraseStructure.addNonTerminalNode(index - START_ID_OF_NONTERMINALS + 1);
											}
											nonterminals[index] = parent;
										}
										Edge e = phraseStructure.addPhraseStructureEdge(parent, child);
										syntaxGraph.addLabel(e, edgelabelTableName.ToString(), edgelabelSymbol.ToString());
									}
									else if (column.Category == ColumnDescription.SECONDARY_EDGE_LABEL && child != null)
									{
										if (secedgecounter % 2 == 0)
										{
											edgelabelSymbol.Length = 0;
											edgelabelSymbol.Append((i == n - 1)?line.Substring(start):line.Substring(start, i - start));
											secedgecounter++;
										}
										else
										{
											int index = int.Parse((i == n - 1)?line.Substring(start):line.Substring(start, i - start));
											if (index == 0)
											{
												parent = phraseStructure.PhraseStructureRoot;
											}
											else if (index < START_ID_OF_NONTERMINALS)
											{
												parent = phraseStructure.getTokenNode(index);
											}
											else
											{
												parent = nonterminals[index];
												if (parent == null)
												{
													parent = phraseStructure.addNonTerminalNode(index - START_ID_OF_NONTERMINALS + 1);
													nonterminals[index] = parent;
												}
											}
											Edge e = phraseStructure.addSecondaryEdge(parent, child);
											e.addLabel(symbolTables.getSymbolTable(column.Name), edgelabelSymbol.ToString());
											secedgecounter++;
										}
									}
									start = i + 1;
								}
							}
						}
						else
						{ // Terminal
							IEnumerator<ColumnDescription> columns = dataFormatInstance.GetEnumerator();
							ColumnDescription column = null;

							currentTerminalSize++;
							child = syntaxGraph.addTokenNode(currentTerminalSize);
							char[] lineChars = line.ToCharArray();
							int start = 0;
							int secedgecounter = 0;
							for (int i = 0, n = lineChars.Length; i < n; i++)
							{
								if (lineChars[i] == '\t' && start == i)
								{
									start++;
								}
								else if (lineChars[i] == '\t' || i == n - 1)
								{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
									if (columns.hasNext())
									{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
										column = columns.next();
									}
									if (column.Category == ColumnDescription.INPUT && child != null)
									{
										syntaxGraph.addLabel(child, column.Name, (i == n - 1)?line.Substring(start):line.Substring(start, i - start));
									}
									else if (column.Category == ColumnDescription.PHRASE_STRUCTURE_EDGE_LABEL && child != null)
									{ // && column.getName().equals("EDGELABEL")) {
										edgelabelSymbol.Length = 0;
										edgelabelSymbol.Append((i == n - 1)?line.Substring(start):line.Substring(start, i - start));
										edgelabelTableName.Length = 0;
										edgelabelTableName.Append(column.Name);
									}
									else if (column.Category == ColumnDescription.PHRASE_STRUCTURE_NODE_LABEL && child != null)
									{
										int index = int.Parse((i == n - 1)?line.Substring(start):line.Substring(start, i - start));
										parent = nonterminals[index];
										if (parent == null)
										{
											if (index == 0)
											{
												parent = phraseStructure.PhraseStructureRoot;
											}
											else
											{
												parent = phraseStructure.addNonTerminalNode(index - START_ID_OF_NONTERMINALS + 1);
											}
											nonterminals[index] = parent;
										}

										Edge e = phraseStructure.addPhraseStructureEdge(parent, child);
										syntaxGraph.addLabel(e, edgelabelTableName.ToString(), edgelabelSymbol.ToString());
									}
									else if (column.Category == ColumnDescription.SECONDARY_EDGE_LABEL && child != null)
									{
										if (secedgecounter % 2 == 0)
										{
											edgelabelSymbol.Length = 0;
											edgelabelSymbol.Append((i == n - 1)?line.Substring(start):line.Substring(start, i - start));
											secedgecounter++;
										}
										else
										{
											int index = int.Parse((i == n - 1)?line.Substring(start):line.Substring(start, i - start));
											if (index == 0)
											{
												parent = phraseStructure.PhraseStructureRoot;
											}
											else if (index < START_ID_OF_NONTERMINALS)
											{
												parent = phraseStructure.getTokenNode(index);
											}
											else
											{
												parent = nonterminals[index];
												if (parent == null)
												{
													parent = phraseStructure.addNonTerminalNode(index - START_ID_OF_NONTERMINALS + 1);
													nonterminals[index] = parent;
												}
											}
											Edge e = phraseStructure.addSecondaryEdge(parent, child);
											e.addLabel(symbolTables.getSymbolTable(column.Name), edgelabelSymbol.ToString());
											secedgecounter++;
										}
									}
									start = i + 1;
								}
							}
						}
					}
					else if (line.StartsWith("%%", StringComparison.Ordinal))
					{ // comment skip

					}
					else if (line.StartsWith("#FORMAT", StringComparison.Ordinal))
					{
	//				int index = line.indexOf(' ');
	//				if (index > -1) {
	//					try {
	//						formatVersion = Integer.parseInt(line.substring(index+1));
	//					} catch (NumberFormatException e) {
	//						
	//					}
	//				}
					}
					else if (line.StartsWith("#BOT", StringComparison.Ordinal))
					{
	//				int index = line.indexOf(' ');
	//				if (index > -1) {
	//					if (line.substring(index+1).equals("ORIGIN")) {
	//						currentHeaderTable = NegraTables.ORIGIN;
	//					} else if (line.substring(index+1).equals("EDITOR")) {
	//						currentHeaderTable = NegraTables.EDITOR;
	//					} else if (line.substring(index+1).equals("WORDTAG")) {
	//						currentHeaderTable = NegraTables.WORDTAG;
	//					} else if (line.substring(index+1).equals("MORPHTAG")) {
	//						currentHeaderTable = NegraTables.MORPHTAG;
	//					} else if (line.substring(index+1).equals("NODETAG")) {
	//						currentHeaderTable = NegraTables.NODETAG;
	//					} else if (line.substring(index+1).equals("EDGETAG")) {
	//						currentHeaderTable = NegraTables.EDGETAG;
	//					} else if (line.substring(index+1).equals("SECEDGETAG")) {
	//						currentHeaderTable = NegraTables.SECEDGETAG;
	//					} else {
	//						currentHeaderTable = NegraTables.UNDEF;
	//					}
	//				}
					}
					else if (line.StartsWith("#EOT", StringComparison.Ordinal))
					{
						currentHeaderTable = NegraTables.UNDEF;
					}
				}
			}
			catch (IOException e)
			{
				throw new DataFormatException("Error when reading from the input file. ", e);
			}
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


		public virtual int FormatVersion
		{
			get
			{
				return formatVersion;
			}
			set
			{
				formatVersion = value;
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
    
				string[] argv;
				try
				{
					argv = value.Split("[_\\p{Blank}]", true);
				}
				catch (PatternSyntaxException e)
				{
					throw new DataFormatException("Could not split the penn writer option '" + value + "'. ", e);
				}
				for (int i = 0; i < argv.Length - 1; i++)
				{
					if (argv[i][0] != '-')
					{
						throw new DataFormatException("The argument flag should start with the following character '-', not with " + argv[i][0]);
					}
					if (++i >= argv.Length)
					{
						throw new DataFormatException("The last argument does not have any value. ");
					}
					switch (argv[i - 1][1])
					{
					case 's':
						try
						{
							START_ID_OF_NONTERMINALS = int.Parse(argv[i]);
						}
						catch (FormatException)
						{
							throw new MaltChainedException("The TigerXML Reader option -s must be an integer value. ");
						}
						break;
					default:
						throw new DataFormatException("Unknown NegraReader parameter: '" + argv[i - 1] + "' with value '" + argv[i] + "'. ");
					}
				}
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