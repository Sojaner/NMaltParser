using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph.Writer
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class TabWriter : SyntaxGraphWriter
	{
		private StreamWriter writer;
		private DataFormatInstance dataFormatInstance;
		private readonly StringBuilder output;
		private bool closeStream = true;
	//	private String ID = "ID";
	//	private String IGNORE_COLUMN_SIGN = "_";
		private readonly char TAB = '\t';
		private readonly char NEWLINE = '\n';


		public TabWriter()
		{
			output = new StringBuilder();
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
//ORIGINAL LINE: public void writeProlog() throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeProlog()
		{

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeComments(org.maltparser.core.syntaxgraph.TokenStructure syntaxGraph, int at_index) throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeComments(ITokenStructure syntaxGraph, int at_index)
		{
			List<string> commentList = syntaxGraph.GetComment(at_index);
			if (commentList != null)
			{
				try
				{
					for (int i = 0; i < commentList.Count; i++)
					{
						writer.Write(commentList[i]);
						writer.BaseStream.WriteByte(NEWLINE);
					}
				}
				catch (IOException e)
				{
					close();
					throw new DataFormatException("Could not write to the output file. ", e);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeSentence(org.maltparser.core.syntaxgraph.TokenStructure syntaxGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeSentence(ITokenStructure syntaxGraph)
		{
			if (syntaxGraph == null || dataFormatInstance == null || !syntaxGraph.HasTokens())
			{
				return;
			}
			IEnumerator<ColumnDescription> columns = dataFormatInstance.GetEnumerator();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.symbol.SymbolTableHandler symbolTables = syntaxGraph.getSymbolTables();
			SymbolTableHandler symbolTables = syntaxGraph.SymbolTables;

			foreach (int i in syntaxGraph.TokenIndices)
			{
				writeComments(syntaxGraph, i);
				try
				{
					ColumnDescription column = null;
					while (columns.MoveNext())
					{
						column = columns.Current;

						if (column.Category == ColumnDescription.INPUT)
						{ // && column.getType() != ColumnDescription.IGNORE) {
							TokenNode node = syntaxGraph.GetTokenNode(i);
							if (!column.Name.Equals("ID"))
							{
								if (node.hasLabel(symbolTables.getSymbolTable(column.Name)))
								{
									output.Append(node.getLabelSymbol(symbolTables.getSymbolTable(column.Name)));
									if (output.Length != 0)
									{
										writer.Write(output.ToString());
									}
									else
									{
										writer.BaseStream.WriteByte('_');
									}
								}
								else
								{
									writer.BaseStream.WriteByte('_');
								}
							}
							else
							{
								writer.Write(Convert.ToString(i));
							}
						}
						else if (column.Category == ColumnDescription.HEAD && syntaxGraph is IDependencyStructure)
						{
							if (((IDependencyStructure)syntaxGraph).GetDependencyNode(i).hasHead())
							{
								writer.Write(Convert.ToString(((IDependencyStructure)syntaxGraph).GetDependencyNode(i).Head.Index));
							}
							else
							{
								writer.Write(Convert.ToString(0));
							}

						}
						else if (column.Category == ColumnDescription.DEPENDENCY_EDGE_LABEL && syntaxGraph is IDependencyStructure)
						{
							if (((IDependencyStructure)syntaxGraph).GetDependencyNode(i).hasHead() && ((IDependencyStructure)syntaxGraph).GetDependencyNode(i).hasHeadEdgeLabel(symbolTables.getSymbolTable(column.Name)))
							{
								output.Append(((IDependencyStructure)syntaxGraph).GetDependencyNode(i).getHeadEdgeLabelSymbol(symbolTables.getSymbolTable(column.Name)));
							}
							else
							{
								output.Append(((IDependencyStructure)syntaxGraph).GetDefaultRootEdgeLabelSymbol(symbolTables.getSymbolTable(column.Name)));
							}

							if (output.Length != 0)
							{
								writer.Write(output.ToString());
							}
						}
						else
						{
							writer.Write(column.DefaultOutput);
						}
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						if (columns.hasNext())
						{
							writer.BaseStream.WriteByte(TAB);
						}
						output.Length = 0;
					}
					writer.BaseStream.WriteByte(NEWLINE);
					columns = dataFormatInstance.GetEnumerator();
				}
				catch (IOException e)
				{
					close();
					throw new DataFormatException("Could not write to the output file. ", e);
				}
			}
			writeComments(syntaxGraph, syntaxGraph.NTokenNode() + 1);
			try
			{
				writer.BaseStream.WriteByte('\n');
				writer.Flush();
			}
			catch (IOException e)
			{
				close();
				throw new DataFormatException("Could not write to the output file. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeEpilog() throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeEpilog()
		{

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