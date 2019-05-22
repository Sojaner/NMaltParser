using System;
using System.Collections.Generic;
using System.IO;

namespace org.maltparser.core.syntaxgraph.writer
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using ColumnDescription = org.maltparser.core.io.dataformat.ColumnDescription;
	using DataFormatException = org.maltparser.core.io.dataformat.DataFormatException;
	using DataFormatInstance = org.maltparser.core.io.dataformat.DataFormatInstance;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;
	using Edge = org.maltparser.core.syntaxgraph.edge.Edge;
	using NonTerminalNode = org.maltparser.core.syntaxgraph.node.NonTerminalNode;
	using PhraseStructureNode = org.maltparser.core.syntaxgraph.node.PhraseStructureNode;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class NegraWriter : SyntaxGraphWriter
	{
		private StreamWriter writer;
		private DataFormatInstance dataFormatInstance;
		private string optionString;
		private int sentenceCount;
		private LinkedHashMap<int, int> nonTerminalIndexMap;
		private int START_ID_OF_NONTERMINALS = 500;
		private bool closeStream = true;

		public NegraWriter()
		{
			nonTerminalIndexMap = new LinkedHashMap<int, int>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(String fileName, String charsetName) throws org.maltparser.core.exception.MaltChainedException
		public virtual void open(string fileName, string charsetName)
		{
			try
			{
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
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeSentence(org.maltparser.core.syntaxgraph.TokenStructure syntaxGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeSentence(TokenStructure syntaxGraph)
		{
			if (syntaxGraph == null || dataFormatInstance == null || !(syntaxGraph is PhraseStructure) || !syntaxGraph.hasTokens())
			{
				return;
			}
			PhraseStructure phraseStructure = (PhraseStructure)syntaxGraph;
			sentenceCount++;
			try
			{
				writer.Write("#BOS ");
				if (phraseStructure.SentenceID != 0)
				{
					writer.Write(Convert.ToString(phraseStructure.SentenceID));
				}
				else
				{
					writer.Write(Convert.ToString(sentenceCount));
				}
				writer.BaseStream.WriteByte('\n');

				if (phraseStructure.hasNonTerminals())
				{
					calculateIndices(phraseStructure);
					writeTerminals(phraseStructure);
					writeNonTerminals(phraseStructure);
				}
				else
				{
					writeTerminals(phraseStructure);
				}
				writer.Write("#EOS ");
				if (phraseStructure.SentenceID != 0)
				{
					writer.Write(Convert.ToString(phraseStructure.SentenceID));
				}
				else
				{
					writer.Write(Convert.ToString(sentenceCount));
				}
				writer.BaseStream.WriteByte('\n');
			}
			catch (IOException e)
			{
				throw new DataFormatException("Could not write to the output file. ", e);
			}
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeEpilog() throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeEpilog()
		{
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void calculateIndices(org.maltparser.core.syntaxgraph.PhraseStructure phraseStructure) throws org.maltparser.core.exception.MaltChainedException
		private void calculateIndices(PhraseStructure phraseStructure)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.SortedMap<int,int> heights = new java.util.TreeMap<int,int>();
			SortedDictionary<int, int> heights = new SortedDictionary<int, int>();
			foreach (int index in phraseStructure.NonTerminalIndices)
			{
				heights[index] = ((NonTerminalNode)phraseStructure.getNonTerminalNode(index)).Height;
			}

			bool done = false;
			int h = 1;
			int ntid = START_ID_OF_NONTERMINALS;
			nonTerminalIndexMap.clear();
			while (!done)
			{
				done = true;
				foreach (int index in phraseStructure.NonTerminalIndices)
				{
					if (heights[index] == h)
					{
						NonTerminalNode nt = (NonTerminalNode)phraseStructure.getNonTerminalNode(index);
						nonTerminalIndexMap.put(nt.Index, ntid++);
	//					nonTerminalIndexMap.put(nt.getIndex(), nt.getIndex()+START_ID_OF_NONTERMINALS-1);
						done = false;
					}
				}
				h++;
			}

	//		boolean done = false;
	//		int h = 1;
	////		int ntid = START_ID_OF_NONTERMINALS;
	////		nonTerminalIndexMap.clear();
	//		while (!done) {
	//			done = true;
	//			for (int index : phraseStructure.getNonTerminalIndices()) {
	//				if (heights.get(index) == h) {
	//					NonTerminalNode nt = (NonTerminalNode)phraseStructure.getNonTerminalNode(index);
	////					nonTerminalIndexMap.put(nt.getIndex(), ntid++);
	//					nonTerminalIndexMap.put(nt.getIndex(), nt.getIndex()+START_ID_OF_NONTERMINALS-1);
	//					done = false;
	//				}
	//			}
	//			h++;
	//		}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeTerminals(org.maltparser.core.syntaxgraph.PhraseStructure phraseStructure) throws org.maltparser.core.exception.MaltChainedException
		private void writeTerminals(PhraseStructure phraseStructure)
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.symbol.SymbolTableHandler symbolTables = phraseStructure.getSymbolTables();
				SymbolTableHandler symbolTables = phraseStructure.SymbolTables;
				foreach (int index in phraseStructure.TokenIndices)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.PhraseStructureNode terminal = phraseStructure.getTokenNode(index);
					PhraseStructureNode terminal = phraseStructure.getTokenNode(index);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<org.maltparser.core.io.dataformat.ColumnDescription> columns = dataFormatInstance.iterator();
					IEnumerator<ColumnDescription> columns = dataFormatInstance.GetEnumerator();
					ColumnDescription column = null;
					int ti = 1;
					while (columns.MoveNext())
					{
						column = columns.Current;
						if (column.Category == ColumnDescription.INPUT)
						{
							SymbolTable table = symbolTables.getSymbolTable(column.Name);
							writer.Write(terminal.getLabelSymbol(table));
							int nTabs = 1;
							if (ti == 1 || ti == 2)
							{
								nTabs = 3 - (terminal.getLabelSymbol(table).Length / 8);
							}
							else if (ti == 3)
							{
								nTabs = 1;
							}
							else if (ti == 4)
							{
								nTabs = 2 - (terminal.getLabelSymbol(table).Length / 8);
							}
							if (nTabs < 1)
							{
								nTabs = 1;
							}
							for (int j = 0; j < nTabs; j++)
							{
								writer.BaseStream.WriteByte('\t');
							}
							ti++;
						}
						else if (column.Category == ColumnDescription.PHRASE_STRUCTURE_EDGE_LABEL)
						{
							SymbolTable table = symbolTables.getSymbolTable(column.Name);
							if (terminal.Parent != null && terminal.hasParentEdgeLabel(table))
							{
								writer.Write(terminal.getParentEdgeLabelSymbol(table));
								writer.BaseStream.WriteByte('\t');
							}
							else
							{
								writer.Write("--\t");
							}
						}
						else if (column.Category == ColumnDescription.PHRASE_STRUCTURE_NODE_LABEL)
						{
							if (terminal.Parent == null || terminal.Parent == phraseStructure.PhraseStructureRoot)
							{
								writer.BaseStream.WriteByte('0');
							}
							else
							{
								writer.Write(Convert.ToString(nonTerminalIndexMap.get(terminal.Parent.Index)));
	//							writer.write(Integer.toString(terminal.getParent().getIndex()+START_ID_OF_NONTERMINALS-1));
							}
						}
					}
					SymbolTable table = symbolTables.getSymbolTable(column.Name);
					foreach (Edge e in terminal.IncomingSecondaryEdges)
					{
						if (e.hasLabel(table))
						{
							writer.BaseStream.WriteByte('\t');
							writer.Write(e.getLabelSymbol(table));
							writer.BaseStream.WriteByte('\t');
							if (e.Source is NonTerminalNode)
							{
								writer.Write(Convert.ToString(nonTerminalIndexMap.get(e.Source.Index)));
	//							writer.write(Integer.toString(e.getSource().getIndex()+START_ID_OF_NONTERMINALS-1));
							}
							else
							{
								writer.Write(Convert.ToString(e.Source.Index));
							}
						}
					}
					writer.Write("\n");
				}

			}
			catch (IOException e)
			{
				throw new DataFormatException("The Negra writer is not able to write. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeNonTerminals(org.maltparser.core.syntaxgraph.PhraseStructure phraseStructure) throws org.maltparser.core.exception.MaltChainedException
		private void writeNonTerminals(PhraseStructure phraseStructure)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.symbol.SymbolTableHandler symbolTables = phraseStructure.getSymbolTables();
			SymbolTableHandler symbolTables = phraseStructure.SymbolTables;

			foreach (int index in nonTerminalIndexMap.Keys)
			{
	//		for (int index : phraseStructure.getNonTerminalIndices()) {
				NonTerminalNode nonTerminal = (NonTerminalNode)phraseStructure.getNonTerminalNode(index);

				if (nonTerminal == null || nonTerminal.Root)
				{
					return;
				}
				try
				{
					writer.BaseStream.WriteByte('#');
	//				writer.write(Integer.toString(index+START_ID_OF_NONTERMINALS-1));
					writer.Write(Convert.ToString(nonTerminalIndexMap.get(index)));
					writer.Write("\t\t\t--\t\t\t");
					if (nonTerminal.hasLabel(symbolTables.getSymbolTable("CAT")))
					{
						writer.BaseStream.WriteByte(nonTerminal.getLabelSymbol(symbolTables.getSymbolTable("CAT")));
					}
					else
					{
						writer.Write("--");
					}
					writer.Write("\t--\t\t");
					if (nonTerminal.hasParentEdgeLabel(symbolTables.getSymbolTable("LABEL")))
					{
						writer.Write(nonTerminal.getParentEdgeLabelSymbol(symbolTables.getSymbolTable("LABEL")));
					}
					else
					{
						writer.Write("--");
					}
					writer.BaseStream.WriteByte('\t');
					if (nonTerminal.Parent == null || nonTerminal.Parent.Root)
					{
						writer.BaseStream.WriteByte('0');
					}
					else
					{
	//					writer.write(Integer.toString(nonTerminal.getParent().getIndex()+START_ID_OF_NONTERMINALS-1));
						writer.Write(Convert.ToString(nonTerminalIndexMap.get(nonTerminal.Parent.Index)));
					}
					foreach (Edge e in nonTerminal.IncomingSecondaryEdges)
					{
						if (e.hasLabel(symbolTables.getSymbolTable("SECEDGELABEL")))
						{
							writer.BaseStream.WriteByte('\t');
							writer.Write(e.getLabelSymbol(symbolTables.getSymbolTable("SECEDGELABEL")));
							writer.BaseStream.WriteByte('\t');
							if (e.Source is NonTerminalNode)
							{
	//							writer.write(Integer.toString(e.getSource().getIndex()+START_ID_OF_NONTERMINALS-1));
								writer.Write(Convert.ToString(nonTerminalIndexMap.get(e.Source.Index)));
							}
							else
							{
								writer.Write(Convert.ToString(e.Source.Index));
							}
						}
					}
					writer.Write("\n");
				}
				catch (IOException e)
				{
					throw new DataFormatException("The Negra writer is not able to write the non-terminals. ", e);
				}
			}
		}

		public virtual StreamWriter Writer
		{
			get
			{
				return writer;
			}
			set
			{
				this.writer = value;
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
				this.sentenceCount = value;
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


		public virtual string Options
		{
			get
			{
				return optionString;
			}
			set
			{
				this.optionString = value;
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
						catch (System.FormatException)
						{
							throw new MaltChainedException("The TigerXML Reader option -s must be an integer value. ");
						}
						break;
					default:
						throw new DataFormatException("Unknown svm parameter: '" + argv[i - 1] + "' with value '" + argv[i] + "'. ");
					}
				}
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
	}

}