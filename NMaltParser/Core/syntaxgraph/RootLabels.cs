using System.Collections.Generic;

namespace org.maltparser.core.syntaxgraph
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class RootLabels
	{
		public const string DEFAULT_ROOTSYMBOL = "ROOT";
		private readonly LabelSet rootLabelCodes;

		public RootLabels()
		{
			rootLabelCodes = new LabelSet();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setRootLabels(String rootLabelOption, java.util.SortedMap<String, org.maltparser.core.symbol.SymbolTable> edgeSymbolTables) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setRootLabels(string rootLabelOption, SortedDictionary<string, SymbolTable> edgeSymbolTables)
		{
			if (edgeSymbolTables == null)
			{
				return;
			}
			else if (string.ReferenceEquals(rootLabelOption, null) || rootLabelOption.Trim().Length == 0)
			{
				foreach (SymbolTable table in edgeSymbolTables.Values)
				{
					rootLabelCodes.put(table, table.addSymbol(RootLabels.DEFAULT_ROOTSYMBOL));
				}
			}
			else if (rootLabelOption.Trim().IndexOf(',') == -1)
			{
				int index = rootLabelOption.Trim().IndexOf('=');
				if (index == -1)
				{
					foreach (SymbolTable table in edgeSymbolTables.Values)
					{
						rootLabelCodes.put(table, table.addSymbol(rootLabelOption.Trim()));
					}
				}
				else
				{
					string name = rootLabelOption.Trim().Substring(0, index);
					if (edgeSymbolTables[name] == null)
					{
						throw new SyntaxGraphException("The symbol table '" + name + "' cannot be found when defining the root symbol. ");
					}
					else
					{
						rootLabelCodes.put(edgeSymbolTables[name], edgeSymbolTables[name].addSymbol(rootLabelOption.Trim().Substring(index + 1)));
						if (edgeSymbolTables.Count > 1)
						{
							foreach (SymbolTable table in edgeSymbolTables.Values)
							{
								if (!table.Name.Equals(name))
								{
									rootLabelCodes.put(table, table.addSymbol(RootLabels.DEFAULT_ROOTSYMBOL));
								}
							}
						}
					}
				}
			}
			else
			{
				string[] items = rootLabelOption.Trim().Split(",", true);
				for (int i = 0; i < items.Length; i++)
				{
					int index = items[i].Trim().IndexOf('=');
					if (index == -1)
					{
						throw new SyntaxGraphException("The root symbol is undefinied. ");
					}
					else
					{
						string name = items[i].Trim().Substring(0, index);
						if (edgeSymbolTables[name] == null)
						{
							throw new SyntaxGraphException("The symbol table'" + name + "' cannot be found when defining the root symbol. ");
						}
						else
						{
							rootLabelCodes.put(edgeSymbolTables[name], edgeSymbolTables[name].addSymbol(items[i].Trim().Substring(index + 1)));
						}
					}
				}
				foreach (SymbolTable table in edgeSymbolTables.Values)
				{
					if (!rootLabelCodes.containsKey(table))
					{
						rootLabelCodes.put(table, table.addSymbol(RootLabels.DEFAULT_ROOTSYMBOL));
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDefaultRootLabel(org.maltparser.core.symbol.SymbolTable table, String defaultRootSymbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setDefaultRootLabel(SymbolTable table, string defaultRootSymbol)
		{
			rootLabelCodes.put(table, table.addSymbol(defaultRootSymbol));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public System.Nullable<int> getDefaultRootLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual int? getDefaultRootLabelCode(SymbolTable table)
		{
			int? res = rootLabelCodes.get(table);
			if (res == null)
			{
				return table.addSymbol(RootLabels.DEFAULT_ROOTSYMBOL);
			}
			return res;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LabelSet getDefaultRootLabels() throws org.maltparser.core.exception.MaltChainedException
		public virtual LabelSet DefaultRootLabels
		{
			get
			{
				return new LabelSet(rootLabelCodes);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getDefaultRootLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getDefaultRootLabelSymbol(SymbolTable table)
		{
			return table.getSymbolCodeToString(getDefaultRootLabelCode(table).Value);
		}


		public virtual bool checkRootLabelCodes(LabelSet rlc)
		{
			if (rlc == null && rootLabelCodes == null)
			{
				return true; // or false ?
			}
			else if ((rlc == null && rootLabelCodes != null) || (rlc != null && rootLabelCodes == null))
			{
				return false;
			}
			else if (rlc.size() != rootLabelCodes.size())
			{
				return false;
			}
			else
			{
				foreach (SymbolTable table in rootLabelCodes.Keys)
				{
					if (!rootLabelCodes.get(table).Equals(rlc.get(table)))
					{
						return false;
					}
				}
				return true;
			}
		}
	}

}