using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.core.syntaxgraph.headrules
{

	using Logger = org.apache.log4j.Logger;
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using HashMap = org.maltparser.core.helper.HashMap;
	using URLFinder = org.maltparser.core.helper.URLFinder;
	using DataFormatInstance = org.maltparser.core.io.dataformat.DataFormatInstance;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;
	using NonTerminalNode = org.maltparser.core.syntaxgraph.node.NonTerminalNode;
	using PhraseStructureNode = org.maltparser.core.syntaxgraph.node.PhraseStructureNode;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	[Serializable]
	public class HeadRules : Dictionary<string, HeadRule>
	{
		public const long serialVersionUID = 8045568022124826323L;
		protected internal Logger logger;
		protected internal string name;
		private readonly SymbolTableHandler symbolTableHandler;
		private readonly DataFormatInstance dataFormatInstance;
		protected internal SymbolTable nonTerminalSymbolTable; // TODO more complex
		protected internal SymbolTable edgelabelSymbolTable; // TODO more complex

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public HeadRules(org.apache.log4j.Logger logger, org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public HeadRules(Logger logger, DataFormatInstance dataFormatInstance, SymbolTableHandler symbolTableHandler)
		{
			Logger = logger;
			this.dataFormatInstance = dataFormatInstance;
			this.symbolTableHandler = symbolTableHandler;
			nonTerminalSymbolTable = symbolTableHandler.addSymbolTable("CAT");
			edgelabelSymbolTable = symbolTableHandler.addSymbolTable("LABEL");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parseHeadRules(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual void parseHeadRules(string fileName)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.helper.URLFinder f = new org.maltparser.core.helper.URLFinder();
			URLFinder f = new URLFinder();
			parseHeadRules(f.findURL(fileName));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parseHeadRules(java.net.URL url) throws org.maltparser.core.exception.MaltChainedException
		public virtual void parseHeadRules(URL url)
		{
			StreamReader br = null;
			try
			{
				br = new StreamReader(url.openStream());
			}
			catch (IOException e)
			{
				throw new HeadRuleException("Could not read the head rules from file '" + url.ToString() + "'. ", e);
			}
			if (logger.InfoEnabled)
			{
				logger.debug("Loading the head rule specification '" + url.ToString() + "' ...\n");
			}
			string fileLine;
			while (true)
			{
				try
				{
					fileLine = br.ReadLine();
				}
				catch (IOException e)
				{
					throw new HeadRuleException("Could not read the head rules from file '" + url.ToString() + "'. ", e);
				}
				if (string.ReferenceEquals(fileLine, null))
				{
					break;
				}
				if (fileLine.Length <= 1 && fileLine.Trim().Substring(0, 2).Trim().Equals("--"))
				{
					continue;
				}
				int index = fileLine.IndexOf('\t');
				if (index == -1)
				{
					throw new HeadRuleException("The specification of the head rule is not correct '" + fileLine + "'. ");
				}

				HeadRule rule = new HeadRule(this, fileLine);
				put(fileLine.Substring(0,index), rule);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode getHeadChild(org.maltparser.core.syntaxgraph.node.NonTerminalNode nt) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode getHeadChild(NonTerminalNode nt)
		{
			HeadRule rule = null;
			if (nt.hasLabel(nonTerminalSymbolTable))
			{
				rule = this[nonTerminalSymbolTable.Name + ":" + nt.getLabelSymbol(nonTerminalSymbolTable)];
			}
			if (rule == null && nt.hasParentEdgeLabel(edgelabelSymbolTable))
			{
				rule = this[edgelabelSymbolTable.Name + ":" + nt.getParentEdgeLabelSymbol(edgelabelSymbolTable)];
			}

			if (rule != null)
			{
				return rule.getHeadChild(nt);
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Direction getDefaultDirection(org.maltparser.core.syntaxgraph.node.NonTerminalNode nt) throws org.maltparser.core.exception.MaltChainedException
		public virtual Direction getDefaultDirection(NonTerminalNode nt)
		{
			HeadRule rule = null;
			if (nt.hasLabel(nonTerminalSymbolTable))
			{
				rule = this[nonTerminalSymbolTable.Name + ":" + nt.getLabelSymbol(nonTerminalSymbolTable)];
			}
			if (rule == null && nt.hasParentEdgeLabel(edgelabelSymbolTable))
			{
				rule = this[edgelabelSymbolTable.Name + ":" + nt.getParentEdgeLabelSymbol(edgelabelSymbolTable)];
			}

			if (rule != null)
			{
				return rule.DefaultDirection;
			}
			return Direction.LEFT;
		}

		public virtual Logger Logger
		{
			get
			{
				return logger;
			}
			set
			{
				this.logger = value;
			}
		}


		public virtual DataFormatInstance DataFormatInstance
		{
			get
			{
				return dataFormatInstance;
			}
		}

		public virtual SymbolTableHandler SymbolTableHandler
		{
			get
			{
				return symbolTableHandler;
			}
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			foreach (HeadRule rule in this.Values)
			{
				sb.Append(rule);
				sb.Append('\n');
			}
			return sb.ToString();
		}
	}

}