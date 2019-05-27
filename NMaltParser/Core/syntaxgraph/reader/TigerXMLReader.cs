using System.Collections.Generic;
using System.IO;
using System.Text;
using NMaltParser.Core.Exception;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Utilities;

namespace NMaltParser.Core.SyntaxGraph.Reader
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class TigerXMLReader : SyntaxGraphReader
	{
	//	private TigerXMLHeader header;
		private XMLStreamReader reader;
		private int sentenceCount;
		private DataFormatInstance dataFormatInstance;
		private StringBuilder ntid;
		private readonly StringBuilder graphRootID;
	//	private StringBuilder elementContent; 
	//	private StringBuilder valueName;
	//	private StringBuilder currentFeatureName;
	//	private Domain domain;
	//	private boolean collectChar = false;
		private string optionString;
		private string fileName = null;
		private URL url = null;
		private string charsetName;
		private int nIterations;
		private int cIterations;
		private int START_ID_OF_NONTERMINALS = 500;
		private bool closeStream = true;

		public TigerXMLReader()
		{
			ntid = new StringBuilder();
	//		elementContent = new StringBuilder();
	//		valueName = new StringBuilder();
	//		currentFeatureName = new StringBuilder(); 
			graphRootID = new StringBuilder();
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
			try
			{
				XMLInputFactory factory = XMLInputFactory.newInstance();
				Reader = factory.createXMLStreamReader(new StreamReader(isr));
			}
			catch (XMLStreamException e)
			{
				throw new DataFormatException("XML input file could be opened. ", e);
			}
			SentenceCount = 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readProlog() throws org.maltparser.core.exception.MaltChainedException
		public virtual void readProlog()
		{

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean readSentence(org.maltparser.core.syntaxgraph.TokenStructure syntaxGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool readSentence(ITokenStructure syntaxGraph)
		{
			if (syntaxGraph == null || !(syntaxGraph is PhraseStructure))
			{
				return false;
			}
			syntaxGraph.Clear();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.PhraseStructure phraseStructure = (org.maltparser.core.syntaxgraph.PhraseStructure)syntaxGraph;
			PhraseStructure phraseStructure = (PhraseStructure)syntaxGraph;
			PhraseStructureNode parent = null;
			PhraseStructureNode child = null;
	//		if (header == null) {
	//			header = new TigerXMLHeader(syntaxGraph.getSymbolTables());
	//		}

			try
			{
				while (true)
				{
					int @event = reader.next();
					if (@event == XMLStreamConstants.START_ELEMENT)
					{
						if (reader.LocalName.length() == 0)
						{
							continue;
						}
						if (reader.LocalName.charAt(0) == 'e')
						{
							// e -> edge, edgelabel
							if (reader.LocalName.length() == 4)
							{ //edge
								int childid = -1;
								int indexSep = reader.getAttributeValue(null, "idref").IndexOf('_');

								try
								{
									if (indexSep != -1)
									{
										childid = int.Parse(reader.getAttributeValue(null, "idref").substring(indexSep + 1));
									}
									else
									{
										childid = int.Parse(reader.getAttributeValue(null, "idref"));
									}
									if (childid == -1)
									{
										throw new SyntaxGraphException("The tiger reader couldn't recognize the idref attribute '" + reader.getAttributeValue(null, "idref") + "' of the edge element. ");
									}
								}
								catch (System.FormatException)
								{
									throw new SyntaxGraphException("The tiger reader couldn't recognize the idref attribute '" + reader.getAttributeValue(null, "idref") + "' of the edge element. ");
								}

								if (childid < START_ID_OF_NONTERMINALS)
								{
									child = phraseStructure.GetTokenNode(childid);
								}
								else
								{

									child = phraseStructure.getNonTerminalNode(childid - START_ID_OF_NONTERMINALS + 1);
								}

								Edge.Edge e = phraseStructure.addPhraseStructureEdge(parent, child);
								SortedDictionary<string, SymbolTable> inputTables = dataFormatInstance.getPhraseStructureEdgeLabelSymbolTables(phraseStructure.SymbolTables);
								foreach (string name in inputTables.Keys)
								{
									e.addLabel(inputTables[name], reader.getAttributeValue(null, name.ToLower()));
								}
							}
							else if (reader.LocalName.Equals("edgelabel"))
							{ // edgelabel
	//							domain = Domain.EL;
							}
						}
						else if (reader.LocalName.charAt(0) == 'n')
						{
							// n -> nt, nonterminals, name
							if (reader.LocalName.length() == 2)
							{ // nt
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String id = reader.getAttributeValue(null, "id");
								string id = reader.getAttributeValue(null, "id");
								if (graphRootID.Length == id.Length && graphRootID.ToString().Equals(id))
								{
									parent = phraseStructure.PhraseStructureRoot;
								}
								else
								{
									int index = id.IndexOf('_');
									if (index != -1)
									{
										parent = phraseStructure.addNonTerminalNode(int.Parse(id.Substring(index + 1)) - START_ID_OF_NONTERMINALS + 1);
									}
								}
								SortedDictionary<string, SymbolTable> inputTables = dataFormatInstance.getPhraseStructureNodeLabelSymbolTables(phraseStructure.SymbolTables);
								foreach (string name in inputTables.Keys)
								{
									parent.addLabel(inputTables[name], reader.getAttributeValue(null, name.ToLower()));
								}
							}
							else if (reader.LocalName.Equals("name"))
							{ // name
	//							elementContent.setLength(0);
	//							collectChar = true;
							}
						}
						else if (reader.LocalName.charAt(0) == 't')
						{
							// t -> t, terminals
							if (reader.LocalName.length() == 1)
							{ // t
								SortedDictionary<string, SymbolTable> inputTables = dataFormatInstance.getInputSymbolTables(phraseStructure.SymbolTables);
								child = syntaxGraph.AddTokenNode();
								foreach (string name in inputTables.Keys)
								{
									child.addLabel(inputTables[name], reader.getAttributeValue(null, name.ToLower()));
								}
							}
						}
						else if (reader.LocalName.charAt(0) == 's')
						{
							// s -> subcorpus, secedge, s, secedgelabel
							if (reader.LocalName.length() == 1)
							{ // s
								string id = reader.getAttributeValue(null, "id");
								bool indexable = false;
								int index = -1;
								if (!ReferenceEquals(id, null) && id.Length > 0)
								{
									for (int i = 0, n = id.Length; i < n; i++)
									{
										if (char.IsDigit(id[i]))
										{
											if (index == -1)
											{
												index = i;
											}
											indexable = true;
										}
									}
								}
								if (indexable)
								{
									phraseStructure.SentenceID = int.Parse(id.Substring(index));
								}
								else
								{
									phraseStructure.SentenceID = sentenceCount + 1;
								}
							}
						}
						else if (reader.LocalName.charAt(0) == 'v')
						{
							// v -> variable, value
	//						if (reader.getLocalName().equals("value")) {
	//							valueName.setLength(0);
	//							valueName.append(reader.getAttributeValue(null, "name"));
	//							elementContent.setLength(0);
	//							collectChar = true;
	//						}
						}
						else
						{
	//						 a -> annotation, author
	//						 b -> body
	//						 c -> corpus
	//						 d -> date, description,
	//						 f -> feature, format
	//						 g -> graph
	//						 h -> head, history
	//						 m -> matches, match
							if (reader.LocalName.Equals("graph"))
							{
								graphRootID.Length = 0;
								graphRootID.Append(reader.getAttributeValue(null, "root"));
							}
							else if (reader.LocalName.Equals("corpus"))
							{
	//							header.setCorpusID(reader.getAttributeValue(null, "id"));
	//							header.setCorpusID(reader.getAttributeValue(null, "version"));
							}
							else if (reader.LocalName.Equals("feature"))
							{
	//							if (header != null) {
	//								currentFeatureName.setLength(0);
	//								currentFeatureName.append(reader.getAttributeValue(null, "name"));
	//								header.addFeature(reader.getAttributeValue(null, "name"), reader.getAttributeValue(null, "domain"));
	//							}
	//							domain = Domain.valueOf(reader.getAttributeValue(null, "domain"));
							}
							else if (reader.LocalName.Equals("secedgelabel"))
							{
	//							domain = Domain.SEL;
							}
							else if (reader.LocalName.Equals("author"))
							{
	//							elementContent.setLength(0);
	//							collectChar = true;
							}
							else if (reader.LocalName.Equals("date"))
							{
	//							elementContent.setLength(0);
	//							collectChar = true;
							}
							else if (reader.LocalName.Equals("description"))
							{
	//							elementContent.setLength(0);
	//							collectChar = true;
							}
							else if (reader.LocalName.Equals("format"))
							{
	//							elementContent.setLength(0);
	//							collectChar = true;
							}
							else if (reader.LocalName.Equals("history"))
							{
	//							elementContent.setLength(0);
	//							collectChar = true;
							}
						}
					}
					else if (@event == XMLStreamConstants.END_ELEMENT)
					{
						if (reader.LocalName.length() == 0)
						{
							continue;
						}
						if (reader.LocalName.charAt(0) == 'e')
						{
							// e -> edge, edgelabel
						}
						else if (reader.LocalName.charAt(0) == 'n')
						{
							// n -> nt, nonterminals, name
							if (reader.LocalName.Equals("nt"))
							{
								ntid.Length = 0;
							}
							else if (reader.LocalName.Equals("nonterminals"))
							{
								if (phraseStructure.NTokenNode() == 1 && phraseStructure.nNonTerminals() == 0 && ((NonTerminalNode)phraseStructure.PhraseStructureRoot).nChildren() == 0)
								{
									Edge.Edge e = phraseStructure.addPhraseStructureEdge(phraseStructure.PhraseStructureRoot, phraseStructure.GetTokenNode(1));
									SortedDictionary<string, SymbolTable> inputTables = dataFormatInstance.getPhraseStructureEdgeLabelSymbolTables(phraseStructure.SymbolTables);
									foreach (string name in inputTables.Keys)
									{
										e.addLabel(inputTables[name], "--");
									}
								}
							}
	//						else if (reader.getLocalName().equals("name")) {
	//							if (header != null) {
	//								header.setMetaName(elementContent.toString());
	//							}
	//							collectChar = false;
	//						}
						}
						else if (reader.LocalName.charAt(0) == 't')
						{
							// t -> t, terminals
						}
						else if (reader.LocalName.charAt(0) == 's')
						{
							// s -> subcorpus, secedge, s, secedgelabel
							if (reader.LocalName.Equals("s"))
							{
								if (syntaxGraph.HasTokens())
								{
									sentenceCount++;
								}
								if (syntaxGraph is MappablePhraseStructureGraph)
								{
									((MappablePhraseStructureGraph)syntaxGraph).Mapping.updateDependenyGraph(((MappablePhraseStructureGraph)syntaxGraph), ((PhraseStructure)syntaxGraph).PhraseStructureRoot);
								}
								return true;
							}
						}
						else if (reader.LocalName.charAt(0) == 'v')
						{
							// v -> variable, value
	//						if (reader.getLocalName().equals("value")) {
	//							if (header != null) {
	//								if (domain == Domain.T || domain == Domain.NT || domain == Domain.FREC) {
	//									header.addFeatureValue(currentFeatureName.toString(), valueName.toString(), elementContent.toString());
	//								} else if (domain == Domain.EL) {
	//									header.addEdgeLabelValue(valueName.toString(), elementContent.toString());
	//								} else if (domain == Domain.SEL) {
	//									header.addSecEdgeLabelValue(valueName.toString(), elementContent.toString());
	//								}
	//							}
	//							collectChar = false;
	//						}
						}
						else
						{
	//						 a -> annotation, author
	//						 b -> body
	//						 c -> corpus
	//						 d -> date, description,
	//						 f -> feature, format
	//						 g -> graph
	//						 h -> head, history
	//						 m -> matches, match
							if (reader.LocalName.Equals("body"))
							{
								//sentence = dataStructures.getSentence();
								//phraseTree = dataStructures.getInPhraseTree();
								//sentence.clear();
								//phraseTree.clear();
								//dataStructures.setLastProcessObject(true);
							}
							else if (reader.LocalName.Equals("author"))
							{
	//							if (header != null) {
	//								header.setMetaAuthor(elementContent.toString());
	//							}
	//							collectChar = false;
							}
							else if (reader.LocalName.Equals("date"))
							{
	//							if (header != null) {
	//								header.setMetaInDate(elementContent.toString());
	//							}
	//							collectChar = false;
							}
							else if (reader.LocalName.Equals("description"))
							{
	//							if (header != null) {
	//								header.setMetaDescription(elementContent.toString());
	//							}
	//							collectChar = false;
							}
							else if (reader.LocalName.Equals("format"))
							{
	//							if (header != null) {
	//								header.setMetaFormat(elementContent.toString());
	//							}
	//							collectChar = false;
							}
							else if (reader.LocalName.Equals("history"))
							{
	//							if (header != null) {
	//								header.setMetaHistory(elementContent.toString());
	//							}
	//							collectChar = false;
							} /* else if (reader.getLocalName().equals("annotation")) {
								if (header != null) {
									System.out.println(header.toTigerXML());
								}
								collectChar = false;
							} */
						}
					}
					else if (@event == XMLStreamConstants.END_DOCUMENT)
					{
						if (syntaxGraph.HasTokens())
						{
							sentenceCount++;
						}
						if (cIterations < nIterations)
						{
							cIterations++;
							reopen();
							return true;
						}
						return false;
					}
					else if (@event == XMLStreamConstants.CHARACTERS)
					{
	//					if (collectChar) {
	//						char[] ch = reader.getTextCharacters();
	//						final int size = reader.getTextStart()+reader.getTextLength();
	//						for (int i = reader.getTextStart(); i < size; i++) {
	//							elementContent.append(ch[i]);
	//						}
	//					}
					}
				}
			}
			catch (XMLStreamException e)
			{
				throw new DataFormatException("", e);
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


		public virtual XMLStreamReader Reader
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
//ORIGINAL LINE: public void readEpilog() throws org.maltparser.core.exception.MaltChainedException
		public virtual void readEpilog()
		{

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
						reader.close();
					}
					reader = null;
				}
			}
			catch (XMLStreamException e)
			{
				throw new DataFormatException("The XML input file could be closed. ", e);
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
					throw new DataFormatException("Could not split the TigerXML reader option '" + value + "'. ", e);
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
						catch (System.FormatException)
						{
							throw new MaltChainedException("The TigerXML Reader option -s must be an integer value. ");
						}
						break;
					default:
						throw new DataFormatException("Unknown TigerXMLReader parameter: '" + argv[i - 1] + "' with value '" + argv[i] + "'. ");
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
	//	public TigerXMLHeader getHeader() {
	//		return header;
	//	}
	//	
	//	public void setHeader(TigerXMLHeader header) {
	//		this.header = header;
	//	}
	}

}