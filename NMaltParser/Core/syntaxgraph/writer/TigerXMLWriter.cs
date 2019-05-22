using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Helper;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Core.SyntaxGraph.Reader;
using NMaltParser.Utilities;

namespace NMaltParser.Core.SyntaxGraph.Writer
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class TigerXMLWriter : SyntaxGraphWriter
	{
		private enum RootHandling
		{
			TALBANKEN,
			NORMAL
		}

		private StreamWriter writer;
		private DataFormatInstance dataFormatInstance;
		private string optionString;
		private int sentenceCount;
		private TigerXMLHeader header;
	//	private boolean hasWriteTigerXMLHeader = false;
		private RootHandling rootHandling;
		private string sentencePrefix = "s";
		private StringBuilder sentenceID;
		private StringBuilder tmpID;
		private StringBuilder rootID;
		private int START_ID_OF_NONTERMINALS = 500;
		private bool labeledTerminalID;
		private string VROOT_SYMBOL = "VROOT";
		private bool useVROOT = false;
	//	private String fileName = null;
	//	private String charsetName = null;
		private bool closeStream = true;

		public TigerXMLWriter()
		{
			sentenceID = new StringBuilder();
			tmpID = new StringBuilder();
			rootID = new StringBuilder();
			labeledTerminalID = false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(String fileName, String charsetName) throws org.maltparser.core.exception.MaltChainedException
		public virtual void open(string fileName, string charsetName)
		{
			try
			{
	//			this.fileName = fileName;
	//			this.charsetName = charsetName;
				open(new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write), charsetName));
			}
			catch (FileNotFoundException e)
			{
				throw new DataFormatException("The output file '" + fileName + "' cannot be found.", e);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new DataFormatException("The character encoding set '" + charsetName + "' isn't supported.", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(java.io.OutputStream os, String charsetName) throws org.maltparser.core.exception.MaltChainedException
		public virtual void open(Stream os, string charsetName)
		{
			try
			{
				if (os == System.out || os == System.err)
				{
					closeStream = false;
				}
				open(new StreamWriter(os, charsetName));
			}
			catch (UnsupportedEncodingException e)
			{
				throw new DataFormatException("The character encoding set '" + charsetName + "' isn't supported.", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void open(java.io.OutputStreamWriter osw) throws org.maltparser.core.exception.MaltChainedException
		private void open(StreamWriter osw)
		{
			Writer = new StreamWriter(osw);
			SentenceCount = 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeProlog() throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeProlog()
		{
	//		if (fileName == null || charsetName == null) {
				writeHeader();
	//		}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeSentence(org.maltparser.core.syntaxgraph.TokenStructure syntaxGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeSentence(TokenStructure syntaxGraph)
		{
			if (syntaxGraph == null || dataFormatInstance == null)
			{
				return;
			}
			if (syntaxGraph.hasTokens())
			{
				sentenceCount++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.PhraseStructure phraseStructure = (org.maltparser.core.syntaxgraph.PhraseStructure)syntaxGraph;
				PhraseStructure phraseStructure = (PhraseStructure)syntaxGraph;
				try
				{
					sentenceID.Length = 0;
					sentenceID.Append(sentencePrefix);
					if (phraseStructure.SentenceID != 0)
					{
						sentenceID.Append(Convert.ToString(phraseStructure.SentenceID));
					}
					else
					{
						sentenceID.Append(Convert.ToString(sentenceCount));
					}
					writer.Write("    <s id=\"");
					writer.Write(sentenceID.ToString());
					writer.Write("\">\n");

					RootID = phraseStructure;
					writer.Write("      <graph root=\"");
					writer.Write(rootID.ToString());
					writer.Write("\" ");
					writer.Write("discontinuous=\"");
					writer.Write(Convert.ToString(!phraseStructure.Continuous));
					writer.Write("\">\n");

					writeTerminals(phraseStructure);
					if (phraseStructure.nTokenNode() != 1 || rootHandling.Equals(RootHandling.TALBANKEN))
					{
						writeNonTerminals(phraseStructure);
					}
					else
					{
						writer.Write("        <nonterminals/>\n");
					}
					writer.Write("      </graph>\n");
					writer.Write("    </s>\n");
				}
				catch (IOException e)
				{
					throw new DataFormatException("The TigerXML writer could not write to file. ", e);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void setRootID(org.maltparser.core.syntaxgraph.PhraseStructure phraseStructure) throws org.maltparser.core.exception.MaltChainedException
		private PhraseStructure RootID
		{
			set
			{
				useVROOT = false;
				PhraseStructureNode root = value.PhraseStructureRoot;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.maltparser.core.symbol.SymbolTableHandler symbolTables = value.getSymbolTables();
				SymbolTableHandler symbolTables = value.SymbolTables;
				foreach (ColumnDescription column in dataFormatInstance.PhraseStructureNodeLabelColumnDescriptionSet)
				{
					if (root.hasLabel(symbolTables.getSymbolTable(column.Name)) && root.getLabelSymbol(symbolTables.getSymbolTable(column.Name)).Equals(VROOT_SYMBOL))
					{
						useVROOT = true;
						break;
					}
				}
				if (useVROOT)
				{
					rootID.Length = 0;
					rootID.Append(sentenceID);
					rootID.Append('_');
					rootID.Append(VROOT_SYMBOL);
				}
				else if (value.nTokenNode() == 1 && value.nNonTerminals() == 0 && !root.Labeled)
				{
					rootID.Length = 0;
					rootID.Append(sentenceID);
					rootID.Append("_1");
				}
				else
				{
					rootID.Length = 0;
					rootID.Append(sentenceID);
					rootID.Append('_');
		//			if (rootHandling.equals(RootHandling.NORMAL)) { 
						rootID.Append(Convert.ToString(START_ID_OF_NONTERMINALS + value.nNonTerminals()));
		//			} else if (rootHandling.equals(RootHandling.TALBANKEN)) {
		//				rootID.append(Integer.toString(START_ID_OF_NONTERMINALS+1));
		//			}
				}
    
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeEpilog() throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeEpilog()
		{
			writeTail();
		}

		public virtual StreamWriter Writer
		{
			get
			{
				return writer;
			}
			set
			{
				writer = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws org.maltparser.core.exception.MaltChainedException
		public virtual void close()
		{
			try
			{
				if (writer != null)
				{
					writer.Flush();
					if (closeStream)
					{
						writer.Close();
					}
					writer = null;
				}
			}
			catch (IOException e)
			{
				throw new DataFormatException("Could not close the output file. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeHeader() throws org.maltparser.core.exception.MaltChainedException
		private void writeHeader()
		{
			try
			{
				if (header == null)
				{
					header = new TigerXMLHeader();
				}
				writer.Write(header.toTigerXML());
	//			hasWriteTigerXMLHeader = true;
			}
			catch (IOException e)
			{
				throw new DataFormatException("The TigerXML writer could not write to file. ", e);
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeTerminals(org.maltparser.core.syntaxgraph.PhraseStructure phraseStructure) throws org.maltparser.core.exception.MaltChainedException
		private void writeTerminals(PhraseStructure phraseStructure)
		{
			try
			{
				writer.Write("        <terminals>\n");
				foreach (int index in phraseStructure.TokenIndices)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.PhraseStructureNode t = phraseStructure.getTokenNode(index);
					PhraseStructureNode t = phraseStructure.getTokenNode(index);
					writer.Write("          <t ");
					if (!labeledTerminalID)
					{
						tmpID.Length = 0;
						tmpID.Append(sentenceID);
						tmpID.Append('_');
						tmpID.Append(Convert.ToString(t.Index));
						writer.Write("id=\"");
						writer.Write(tmpID.ToString());
						writer.Write("\" ");
					}

					foreach (ColumnDescription column in dataFormatInstance.InputColumnDescriptionSet)
					{
						writer.Write(column.Name.ToLower());
						writer.Write("=\"");
						writer.Write(Util.xmlEscape(t.getLabelSymbol(phraseStructure.SymbolTables.getSymbolTable(column.Name))));
						writer.Write("\" ");
					}
					writer.Write("/>\n");
				}
				writer.Write("        </terminals>\n");
			}
			catch (IOException e)
			{
				throw new DataFormatException("The TigerXML writer is not able to write. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeNonTerminals(org.maltparser.core.syntaxgraph.PhraseStructure phraseStructure) throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeNonTerminals(PhraseStructure phraseStructure)
		{
			try
			{
				SortedDictionary<int, int> heights = new SortedDictionary<int, int>();
				foreach (int index in phraseStructure.NonTerminalIndices)
				{
					heights[index] = ((NonTerminalNode)phraseStructure.getNonTerminalNode(index)).Height;
				}
				writer.Write("        <nonterminals>\n");
				bool done = false;
				int h = 1;
				while (!done)
				{
					done = true;
					foreach (int index in phraseStructure.NonTerminalIndices)
					{
						if (heights[index] == h)
						{
							NonTerminalNode nt = (NonTerminalNode)phraseStructure.getNonTerminalNode(index);
							tmpID.Length = 0;
							tmpID.Append(sentenceID);
							tmpID.Append('_');
							tmpID.Append(Convert.ToString(nt.Index + START_ID_OF_NONTERMINALS - 1));
							writeNonTerminal(phraseStructure.SymbolTables, nt, tmpID.ToString());
							done = false;
						}
					}
					h++;
				}

				writeNonTerminal(phraseStructure.SymbolTables, (NonTerminalNode)phraseStructure.PhraseStructureRoot,rootID.ToString());
				writer.Write("        </nonterminals>\n");
			}
			catch (IOException e)
			{
				throw new DataFormatException("The TigerXML writer is not able to write. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeNonTerminal(org.maltparser.core.symbol.SymbolTableHandler symbolTables, org.maltparser.core.syntaxgraph.node.NonTerminalNode nt, String id) throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeNonTerminal(SymbolTableHandler symbolTables, NonTerminalNode nt, string id)
		{
			try
			{
				writer.Write("          <nt");
				writer.Write(" id=\"");
				writer.Write(id);
				writer.Write("\" ");
				foreach (ColumnDescription column in dataFormatInstance.PhraseStructureNodeLabelColumnDescriptionSet)
				{
					if (nt.hasLabel(symbolTables.getSymbolTable(column.Name)))
					{
						writer.Write(column.Name.ToLower());
						writer.Write("=");
						writer.Write("\"");
						writer.Write(Util.xmlEscape(nt.getLabelSymbol(symbolTables.getSymbolTable(column.Name))));
						writer.Write("\" ");
					}
				}
				writer.Write(">\n");

				for (int i = 0, n = nt.nChildren(); i < n; i++)
				{
					PhraseStructureNode child = nt.getChild(i);
					writer.Write("            <edge ");

					foreach (ColumnDescription column in dataFormatInstance.PhraseStructureEdgeLabelColumnDescriptionSet)
					{
						if (child.hasParentEdgeLabel(symbolTables.getSymbolTable(column.Name)))
						{
							writer.Write(column.Name.ToLower());
							writer.Write("=\"");
							writer.Write(Util.xmlEscape(child.getParentEdgeLabelSymbol(symbolTables.getSymbolTable(column.Name))));
							writer.Write("\" ");
						}
					}
					if (child is TokenNode)
					{
						if (!labeledTerminalID)
						{
							tmpID.Length = 0;
							tmpID.Append(sentenceID);
							tmpID.Append('_');
							tmpID.Append(Convert.ToString(child.Index));
							writer.Write(" idref=\"");
							writer.Write(tmpID.ToString());
							writer.Write("\"");
						}
						else
						{
							writer.Write(" idref=\"");
							writer.Write(child.getLabelSymbol(symbolTables.getSymbolTable("ID")));
							writer.Write("\"");
						}

					}
					else
					{
						tmpID.Length = 0;
						tmpID.Append(sentenceID);
						tmpID.Append('_');
						tmpID.Append(Convert.ToString(child.Index + START_ID_OF_NONTERMINALS - 1));
						writer.Write(" idref=\"");
						writer.Write(tmpID.ToString());
						writer.Write("\"");
					}
					writer.Write(" />\n");
				}
				writer.Write("          </nt>\n");
			}
			catch (IOException e)
			{
				throw new DataFormatException("The TigerXML writer is not able to write. ", e);
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeTail() throws org.maltparser.core.exception.MaltChainedException
		private void writeTail()
		{
			try
			{
				writer.Write("  </body>\n");
				writer.Write("</corpus>\n");
				writer.Flush();
	//			if (fileName != null && charsetName != null) {
	//				writer.close();
	//				writer = null;
	//				BufferedWriter headerWriter = new BufferedWriter(new OutputStreamWriter(new FileOutputStream(fileName+".header"),charsetName));
	//				if (header == null) {
	//					header = new TigerXMLHeader(dataFormatInstance.getSymbolTables());
	//				}
	//				
	//				headerWriter.write(header.toTigerXML());
	//				headerWriter.flush();
	//				headerWriter.close();
	//			}
			}
			catch (IOException e)
			{
				throw new DataFormatException("The TigerXML writer is not able to write. ", e);
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


		public virtual DataFormatInstance DataFormatInstance
		{
			get
			{
				return dataFormatInstance;
			}
			set
			{
				dataFormatInstance = value;
				labeledTerminalID = (value.InputColumnDescriptions.ContainsKey("id") || value.InputColumnDescriptions.ContainsKey("ID"));
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
				rootHandling = RootHandling.NORMAL;
    
				string[] argv;
				try
				{
					argv = value.Split("[_\\p{Blank}]", true);
				}
				catch (PatternSyntaxException e)
				{
					throw new DataFormatException("Could not split the TigerXML writer option '" + value + "'. ", e);
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
					case 'r':
						if (argv[i].Equals("n"))
						{
							rootHandling = RootHandling.NORMAL;
						}
						else if (argv[i].Equals("tal"))
						{
							rootHandling = RootHandling.TALBANKEN;
						}
						break;
					case 's':
						try
						{
							START_ID_OF_NONTERMINALS = int.Parse(argv[i]);
						}
						catch (FormatException)
						{
							throw new MaltChainedException("The TigerXML writer option -s must be an integer value. ");
						}
						break;
					case 'v':
						VROOT_SYMBOL = argv[i];
						break;
					default:
						throw new DataFormatException("Unknown TigerXML writer option: '" + argv[i - 1] + "' with value '" + argv[i] + "'. ");
					}
				}
			}
		}

	}

}