using System.Collections.Generic;
using System.IO;
using System.Text;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Utilities;

namespace NMaltParser.Core.SyntaxGraph.Writer
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class BracketWriter : SyntaxGraphWriter
	{
		private enum PennWriterFormat
		{
			DEFAULT,
			PRETTY
		}
		private PennWriterFormat format;
		private StreamWriter writer;
		private DataFormatInstance dataFormatInstance;
		private SortedDictionary<string, ColumnDescription> inputColumns;
		private SortedDictionary<string, ColumnDescription> edgeLabelColumns;
		private SortedDictionary<string, ColumnDescription> phraseLabelColumns;
		private char STARTING_BRACKET = '(';
		private string EMPTY_EDGELABEL = "??";
		private char CLOSING_BRACKET = ')';
		private char INPUT_SEPARATOR = ' ';
		private char EDGELABEL_SEPARATOR = '-';
		private char SENTENCE_SEPARATOR = '\n';
		private string optionString;
		private bool closeStream = true;

		public BracketWriter()
		{
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
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeEpilog() throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeEpilog()
		{

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeProlog() throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeProlog()
		{

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeSentence(org.maltparser.core.syntaxgraph.TokenStructure syntaxGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeSentence(ITokenStructure syntaxGraph)
		{
			if (syntaxGraph == null || dataFormatInstance == null)
			{
				return;
			}
			if (syntaxGraph is PhraseStructure && syntaxGraph.HasTokens())
			{
	//			PhraseStructure phraseStructure = ((PhraseStructure) syntaxGraph);
				if (format == PennWriterFormat.PRETTY)
				{
					writeElement(syntaxGraph.SymbolTables, ((PhraseStructure) syntaxGraph).PhraseStructureRoot, 0);
				}
				else
				{
					writeElement(syntaxGraph.SymbolTables, ((PhraseStructure) syntaxGraph).PhraseStructureRoot);
				}
				try
				{
					writer.BaseStream.WriteByte(SENTENCE_SEPARATOR);
					writer.Flush();
				}
				catch (IOException e)
				{
					close();
					throw new DataFormatException("Could not write to the output file. ", e);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeElement(org.maltparser.core.symbol.SymbolTableHandler symbolTables, org.maltparser.core.syntaxgraph.node.PhraseStructureNode element) throws org.maltparser.core.exception.MaltChainedException
		private void writeElement(SymbolTableHandler symbolTables, PhraseStructureNode element)
		{
			try
			{
				if (element is TokenNode)
				{
					PhraseStructureNode t = (PhraseStructureNode)element;
					SymbolTable table = null;
					writer.BaseStream.WriteByte(STARTING_BRACKET);
					int i = 0;
					foreach (string inputColumn in inputColumns.Keys)
					{
						if (i != 0)
						{
							writer.BaseStream.WriteByte(INPUT_SEPARATOR);
						}
						table = symbolTables.getSymbolTable(inputColumns[inputColumn].Name);
						if (t.hasLabel(table))
						{
							writer.Write(t.getLabelSymbol(table));
						}
						if (i == 0)
						{
							foreach (string edgeLabelColumn in edgeLabelColumns.Keys)
							{
								table = symbolTables.getSymbolTable(edgeLabelColumns[edgeLabelColumn].Name);
								if (t.hasParentEdgeLabel(table) && !t.Parent.Root && !t.getParentEdgeLabelSymbol(table).Equals(EMPTY_EDGELABEL))
								{
									writer.BaseStream.WriteByte(EDGELABEL_SEPARATOR);
									writer.Write(t.getParentEdgeLabelSymbol(table));
								}
							}
						}
						i++;
					}
					writer.BaseStream.WriteByte(CLOSING_BRACKET);
				}
				else
				{
					NonTerminalNode nt = (NonTerminalNode)element;
					writer.BaseStream.WriteByte(STARTING_BRACKET);
					SymbolTable table = null;
					int i = 0;
					foreach (string phraseLabelColumn in phraseLabelColumns.Keys)
					{
						if (i != 0)
						{
							writer.BaseStream.WriteByte(INPUT_SEPARATOR);
						}
						table = symbolTables.getSymbolTable(phraseLabelColumns[phraseLabelColumn].Name);
						if (nt.hasLabel(table))
						{
							writer.BaseStream.WriteByte(nt.getLabelSymbol(table));
						}
						if (i == 0)
						{
							foreach (string edgeLabelColumn in edgeLabelColumns.Keys)
							{
								table = symbolTables.getSymbolTable(edgeLabelColumns[edgeLabelColumn].Name);
								if (nt.hasParentEdgeLabel(table) && !nt.Parent.Root && !nt.getParentEdgeLabelSymbol(table).Equals(EMPTY_EDGELABEL))
								{
									writer.BaseStream.WriteByte(EDGELABEL_SEPARATOR);
									writer.Write(nt.getParentEdgeLabelSymbol(table));
								}
							}
						}
						i++;
					}
					foreach (PhraseStructureNode node in ((NonTerminalNode)element).Children)
					{
						writeElement(symbolTables, node);
					}
					writer.BaseStream.WriteByte(CLOSING_BRACKET);
				}
			}
			catch (IOException e)
			{
				throw new DataFormatException("Could not write to the output file. ", e);
			}
		}

		private string getIndentation(int depth)
		{
			StringBuilder sb = new StringBuilder("");
			for (int i = 0; i < depth; i++)
			{
				sb.Append("\t");
			}
			return sb.ToString();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeElement(org.maltparser.core.symbol.SymbolTableHandler symbolTables, org.maltparser.core.syntaxgraph.node.PhraseStructureNode element, int depth) throws org.maltparser.core.exception.MaltChainedException
		private void writeElement(SymbolTableHandler symbolTables, PhraseStructureNode element, int depth)
		{
			try
			{
				if (element is TokenNode)
				{
					PhraseStructureNode t = (PhraseStructureNode)element;
					SymbolTable table = null;
					writer.Write("\n" + getIndentation(depth) + STARTING_BRACKET);
					int i = 0;
					foreach (string inputColumn in inputColumns.Keys)
					{
						if (i != 0)
						{
							writer.BaseStream.WriteByte(INPUT_SEPARATOR);
						}
						table = symbolTables.getSymbolTable(inputColumns[inputColumn].Name);
						if (t.hasLabel(table))
						{
							writer.Write(encodeString(t.getLabelSymbol(table)));
						}
						if (i == 0)
						{
							foreach (string edgeLabelColumn in edgeLabelColumns.Keys)
							{
								table = symbolTables.getSymbolTable(edgeLabelColumns[edgeLabelColumn].Name);
								if (t.hasParentEdgeLabel(table) && !t.Parent.Root && !t.getParentEdgeLabelSymbol(table).Equals(EMPTY_EDGELABEL))
								{
									writer.BaseStream.WriteByte(EDGELABEL_SEPARATOR);
									writer.Write(t.getParentEdgeLabelSymbol(table));
								}
							}
						}
						i++;
					}
					writer.BaseStream.WriteByte(CLOSING_BRACKET);
				}
				else
				{
					NonTerminalNode nt = (NonTerminalNode)element;
					writer.Write("\n" + getIndentation(depth) + STARTING_BRACKET);
					SymbolTable table = null;
					int i = 0;
					foreach (string phraseLabelColumn in phraseLabelColumns.Keys)
					{
						if (i != 0)
						{
							writer.BaseStream.WriteByte(INPUT_SEPARATOR);
						}
						table = symbolTables.getSymbolTable(phraseLabelColumns[phraseLabelColumn].Name);
						if (nt.hasLabel(table))
						{
							writer.BaseStream.WriteByte(nt.getLabelSymbol(table));
						}
						if (i == 0)
						{
							foreach (string edgeLabelColumn in edgeLabelColumns.Keys)
							{
								table = symbolTables.getSymbolTable(edgeLabelColumns[edgeLabelColumn].Name);
								if (nt.hasParentEdgeLabel(table) && !nt.Parent.Root && !nt.getParentEdgeLabelSymbol(table).Equals(EMPTY_EDGELABEL))
								{
									writer.BaseStream.WriteByte(EDGELABEL_SEPARATOR);
									writer.Write(nt.getParentEdgeLabelSymbol(table));
								}
							}
						}
						i++;
					}
					foreach (PhraseStructureNode node in ((NonTerminalNode)element).Children)
					{
						writeElement(symbolTables, node, depth + 1);
					}
					writer.Write("\n" + getIndentation(depth) + CLOSING_BRACKET);
				}
			}
			catch (IOException e)
			{
				throw new DataFormatException("Could not write to the output file. ", e);
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
				close();
				writer = value;
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
				inputColumns = value.InputColumnDescriptions;
				edgeLabelColumns = value.PhraseStructureEdgeLabelColumnDescriptions;
				phraseLabelColumns = value.PhraseStructureNodeLabelColumnDescriptions;
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
				format = PennWriterFormat.DEFAULT;
    
				string[] argv;
				try
				{
					argv = value.Split("[_\\p{Blank}]", true);
				}
				catch (PatternSyntaxException e)
				{
					throw new DataFormatException("Could not split the bracket writer option '" + value + "'. ", e);
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
					case 'f':
						if (argv[i].Equals("p"))
						{
							format = PennWriterFormat.PRETTY;
						}
						else if (argv[i].Equals("p"))
						{
							format = PennWriterFormat.DEFAULT;
						}
						break;
					default:
						throw new DataFormatException("Unknown bracket writer option: '" + argv[i - 1] + "' with value '" + argv[i] + "'. ");
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

		private string encodeString(string @string)
		{
			return @string.Replace("(", "-LRB-").Replace(")", "-RRB-").Replace("[", "-LSB-").Replace("]", "-RSB-").Replace("{", "-LCB-").Replace("}", "-RCB-");
		}
	}

}