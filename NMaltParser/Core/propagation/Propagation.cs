using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.propagation
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.propagation.spec;
	using  org.maltparser.core.symbol;
	using  org.maltparser.core.symbol;
	using  org.maltparser.core.syntaxgraph.edge;
	using  org.maltparser.core.syntaxgraph.node;

	/// <summary>
	/// A propagation object propagate a column value from one node to a column in another node based on the propagation specification. 
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class Propagation
	{
		/// 
		private SymbolTable fromTable;
		private SymbolTable toTable;
		private SymbolTable deprelTable;
		private readonly SortedSet<string> forSet;
		private readonly SortedSet<string> overSet;

		private readonly Pattern symbolSeparator;

		/// <summary>
		/// Creates a propagation object based on the propagation specification
		/// </summary>
		/// <param name="spec"> a propagation specification </param>
		/// <param name="dataFormatInstance"> a data format instance </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Propagation(org.maltparser.core.propagation.spec.PropagationSpec spec, org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public Propagation(PropagationSpec spec, DataFormatInstance dataFormatInstance, SymbolTableHandler tableHandler)
		{
			ColumnDescription fromColumn = dataFormatInstance.getColumnDescriptionByName(spec.From);
			if (fromColumn == null)
			{
				throw new PropagationException("The symbol table '" + spec.From + " does not exists.");
			}
			fromTable = tableHandler.getSymbolTable(spec.From);

			ColumnDescription toColumn = dataFormatInstance.getColumnDescriptionByName(spec.To);
			if (toColumn == null)
			{
				toColumn = dataFormatInstance.addInternalColumnDescription(tableHandler, spec.To, fromColumn);
				toTable = tableHandler.getSymbolTable(spec.To);
			}


			forSet = new SortedSet<string>();
			if (!string.ReferenceEquals(spec.For, null) && spec.For.Length > 0)
			{
				string[] items = spec.For.Split("\\|", true);

				foreach (string item in items)
				{
					forSet.Add(item);
				}
			}

			overSet = new SortedSet<string>();
			if (!string.ReferenceEquals(spec.Over, null) && spec.Over.Length > 0)
			{
				string[] items = spec.Over.Split("\\|", true);

				foreach (string item in items)
				{
					overSet.Add(item);
				}
			}

	//		ColumnDescription deprelColumn = dataFormatInstance.getColumnDescriptionByName("DEPREL");
			deprelTable = tableHandler.getSymbolTable("DEPREL");
			symbolSeparator = Pattern.compile("\\|");
		}

		/// <summary>
		/// Propagate columns according to the propagation specification
		/// </summary>
		/// <param name="e"> an edge </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void propagate(org.maltparser.core.syntaxgraph.edge.Edge e) throws org.maltparser.core.exception.MaltChainedException
		public virtual void propagate(Edge e)
		{
			if (e != null && e.hasLabel(deprelTable) && !e.Source.Root)
			{
				if (overSet.Count == 0 || overSet.Contains(e.getLabelSymbol(deprelTable)))
				{
					DependencyNode to = (DependencyNode)e.Source;
					DependencyNode from = (DependencyNode)e.Target;
					string fromSymbol = null;
					if (e.hasLabel(fromTable))
					{
						fromSymbol = e.getLabelSymbol(fromTable);
					}
					else if (from.hasLabel(fromTable))
					{
						fromSymbol = from.getLabelSymbol(fromTable);
					}

					string propSymbol = null;
					if (to.hasLabel(toTable))
					{
						propSymbol = union(fromSymbol, to.getLabelSymbol(toTable));
					}
					else
					{
						if (forSet.Count == 0 || forSet.Contains(fromSymbol))
						{
							propSymbol = fromSymbol;
						}
					}
					if (!string.ReferenceEquals(propSymbol, null))
					{
						to.addLabel(toTable, propSymbol);
					}
				}
			}
		}

		private string union(string fromSymbol, string toSymbol)
		{
			SortedSet<string> symbolSet = new SortedSet<string>();

			if (!string.ReferenceEquals(fromSymbol, null) && fromSymbol.Length != 0)
			{
				string[] fromSymbols = symbolSeparator.split(fromSymbol);
				for (int i = 0; i < fromSymbols.Length; i++)
				{
					if (forSet.Count == 0 || forSet.Contains(fromSymbols[i]))
					{
						symbolSet.Add(fromSymbols[i]);
					}
				}
			}
			if (!string.ReferenceEquals(toSymbol, null) && toSymbol.Length != 0)
			{
				string[] toSymbols = symbolSeparator.split(toSymbol);
				for (int i = 0; i < toSymbols.Length; i++)
				{
					symbolSet.Add(toSymbols[i]);
				}
			}

			if (symbolSet.Count > 0)
			{
				StringBuilder sb = new StringBuilder();
				foreach (string symbol in symbolSet)
				{
					sb.Append(symbol);
					sb.Append('|');
				}
				sb.Length = sb.Length - 1;
				return sb.ToString();
			}


			return "";
		}
		public override string ToString()
		{
			return "Propagation [forSet=" + forSet + ", fromTable=" + fromTable + ", overSet=" + overSet + ", toTable=" + toTable + "]";
		}
	}

}