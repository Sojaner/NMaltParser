﻿using System.Collections.Generic;
using System.IO;

namespace org.maltparser.core.symbol.parse
{
    using  helper;

	public class ParseSymbolTableHandler : SymbolTableHandler
	{
		private readonly SymbolTableHandler parentSymbolTableHandler;
		private readonly HashMap<string, ParseSymbolTable> symbolTables;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParseSymbolTableHandler(org.maltparser.core.symbol.SymbolTableHandler parentSymbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public ParseSymbolTableHandler(SymbolTableHandler parentSymbolTableHandler)
		{
			this.parentSymbolTableHandler = parentSymbolTableHandler;
			symbolTables = new HashMap<string, ParseSymbolTable>();
			foreach (string tableName in parentSymbolTableHandler.SymbolTableNames)
			{
				addSymbolTable(tableName);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.symbol.SymbolTable addSymbolTable(String tableName) throws org.maltparser.core.exception.MaltChainedException
		public virtual SymbolTable addSymbolTable(string tableName)
		{
			ParseSymbolTable symbolTable = symbolTables[tableName];
			if (symbolTable == null)
			{
				symbolTable = new ParseSymbolTable(tableName, parentSymbolTableHandler);
				symbolTables[tableName] = symbolTable;
			}
			return symbolTable;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.symbol.SymbolTable addSymbolTable(String tableName, org.maltparser.core.symbol.SymbolTable parentTable) throws org.maltparser.core.exception.MaltChainedException
		public virtual SymbolTable addSymbolTable(string tableName, SymbolTable parentTable)
		{
			ParseSymbolTable symbolTable = symbolTables[tableName];
			if (symbolTable == null)
			{
				symbolTable = new ParseSymbolTable(tableName, parentTable, parentSymbolTableHandler);
				symbolTables[tableName] = symbolTable;
			}
			return symbolTable;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.symbol.SymbolTable addSymbolTable(String tableName, int columnCategory, int columnType, String nullValueStrategy) throws org.maltparser.core.exception.MaltChainedException
		public virtual SymbolTable addSymbolTable(string tableName, int columnCategory, int columnType, string nullValueStrategy)
		{
			ParseSymbolTable symbolTable = symbolTables[tableName];
			if (symbolTable == null)
			{
				symbolTable = new ParseSymbolTable(tableName, columnCategory, columnType, nullValueStrategy, parentSymbolTableHandler);
				symbolTables[tableName] = symbolTable;
			}
			return symbolTable;
		}

		public virtual SymbolTable getSymbolTable(string tableName)
		{
			return symbolTables[tableName];
		}

		public virtual ISet<string> SymbolTableNames
		{
			get
			{
				return symbolTables.Keys;
			}
		}

		public virtual void cleanUp()
		{
			foreach (ParseSymbolTable table in symbolTables.Values)
			{
				table.clearTmpStorage();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void save(java.io.OutputStreamWriter osw) throws org.maltparser.core.exception.MaltChainedException
		public virtual void save(StreamWriter osw)
		{
			parentSymbolTableHandler.save(osw);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void save(String fileName, String charSet) throws org.maltparser.core.exception.MaltChainedException
		public virtual void save(string fileName, string charSet)
		{
			parentSymbolTableHandler.save(fileName, charSet);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadHeader(java.io.BufferedReader bin) throws org.maltparser.core.exception.MaltChainedException
		public virtual void loadHeader(StreamReader bin)
		{
			string fileLine = "";
			Pattern tabPattern = Pattern.compile("\t");
			try
			{
				while (!ReferenceEquals((fileLine = bin.ReadLine()), null))
				{
					if (fileLine.Length == 0 || fileLine[0] != '\t')
					{
						break;
					}
					string[] items;
					try
					{
						items = tabPattern.split(fileLine.Substring(1));
					}
					catch (PatternSyntaxException e)
					{
						throw new SymbolException("The header line of the symbol table  '" + fileLine.Substring(1) + "' could not split into atomic parts. ", e);
					}
					if (items.Length == 4)
					{
						addSymbolTable(items[0], int.Parse(items[1]), int.Parse(items[2]), items[3]);
					}
					else if (items.Length == 3)
					{
						addSymbolTable(items[0], int.Parse(items[1]), SymbolTable_Fields.STRING, items[2]);
					}
					else
					{
						throw new SymbolException("The header line of the symbol table  '" + fileLine.Substring(1) + "' must contain three or four columns. ");
					}

				}
			}
			catch (System.FormatException e)
			{
				throw new SymbolException("The symbol table file (.sym) contains a non-integer value in the header. ", e);
			}
			catch (IOException e)
			{
				throw new SymbolException("Could not load the symbol table. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.io.InputStreamReader isr) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(StreamReader isr)
		{
			try
			{
				StreamReader bin = new StreamReader(isr);
				string fileLine;
				SymbolTable table = null;
				bin.mark(2);
				if (bin.Read() == '\t')
				{
					bin.reset();
					loadHeader(bin);
				}
				else
				{
					bin.reset();
				}
				while (!ReferenceEquals((fileLine = bin.ReadLine()), null))
				{
					if (fileLine.Length > 0)
					{
						table = addSymbolTable(fileLine);
						table.load(bin);
					}
				}
				bin.Close();
			}
			catch (IOException e)
			{
				throw new SymbolException("Could not load the symbol tables. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(String fileName, String charSet) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(string fileName, string charSet)
		{
			try
			{
				load(new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read), charSet));
			}
			catch (FileNotFoundException e)
			{
				throw new SymbolException("The symbol table file '" + fileName + "' cannot be found. ", e);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new SymbolException("The char set '" + charSet + "' is not supported. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.symbol.SymbolTable loadTagset(String fileName, String tableName, String charSet, int columnCategory, int columnType, String nullValueStrategy) throws org.maltparser.core.exception.MaltChainedException
		public virtual SymbolTable loadTagset(string fileName, string tableName, string charSet, int columnCategory, int columnType, string nullValueStrategy)
		{
			try
			{
				StreamReader br = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read), charSet);
				string fileLine;
				SymbolTable table = addSymbolTable(tableName, columnCategory, columnType, nullValueStrategy);

				while (!ReferenceEquals((fileLine = br.ReadLine()), null))
				{
					table.addSymbol(fileLine.Trim());
				}
				return table;
			}
			catch (FileNotFoundException e)
			{
				throw new SymbolException("The tagset file '" + fileName + "' cannot be found. ", e);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new SymbolException("The char set '" + charSet + "' is not supported. ", e);
			}
			catch (IOException e)
			{
				throw new SymbolException("The tagset file '" + fileName + "' cannot be loaded. ", e);
			}
		}

	//	public String printSymbolTables() throws MaltChainedException  {
	//		return parentSymbolTableHandler.printSymbolTables();
	//	}
	}

}