﻿using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Pool;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class PhraseStructureGraph : Sentence, PhraseStructure
	{
		protected internal readonly ObjectPoolList<Edge.Edge> edgePool;
		protected internal readonly SortedSet<Edge.Edge> graphEdges;
		protected internal readonly SortedDictionary<int, NonTerminal> nonTerminalNodes;
		protected internal readonly ObjectPoolList<NonTerminal> nonTerminalPool;
		protected internal readonly Root root;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PhraseStructureGraph(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public PhraseStructureGraph(SymbolTableHandler symbolTables) : base(symbolTables)
		{

			root = new Root();
			root.BelongsToGraph = this;

			graphEdges = new SortedSet<Edge.Edge>();
			edgePool = new ObjectPoolListAnonymousInnerClass(this);

			nonTerminalNodes = new SortedDictionary<int, NonTerminal>();
			nonTerminalPool = new ObjectPoolListAnonymousInnerClass2(this);
		}

		private class ObjectPoolListAnonymousInnerClass : ObjectPoolList<Edge.Edge>
		{
			private readonly PhraseStructureGraph outerInstance;

			public ObjectPoolListAnonymousInnerClass(PhraseStructureGraph outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			protected internal override Edge.Edge create()
			{
				return new GraphEdge();
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resetObject(org.maltparser.core.syntaxgraph.edge.Edge o) throws org.maltparser.core.exception.MaltChainedException
			public void resetObject(Edge.Edge o)
			{
				o.clear();
			}
		}

		private class ObjectPoolListAnonymousInnerClass2 : ObjectPoolList<NonTerminal>
		{
			private readonly PhraseStructureGraph outerInstance;

			public ObjectPoolListAnonymousInnerClass2(PhraseStructureGraph outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.maltparser.core.syntaxgraph.node.NonTerminal create() throws org.maltparser.core.exception.MaltChainedException
			protected internal override NonTerminal create()
			{
				return new NonTerminal();
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resetObject(org.maltparser.core.syntaxgraph.node.NonTerminal o) throws org.maltparser.core.exception.MaltChainedException
			public void resetObject(NonTerminal o)
			{
				o.clear();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addTerminalNode() throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode addTerminalNode()
		{
			return AddTokenNode();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addTerminalNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode addTerminalNode(int index)
		{
			return AddTokenNode(index);
		}

		public virtual PhraseStructureNode getTerminalNode(int index)
		{
			return GetTokenNode(index);
		}

		public virtual int nTerminalNode()
		{
			return NTokenNode();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addNonTerminalNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode addNonTerminalNode(int index)
		{
			NonTerminal node = nonTerminalPool.checkOut();
			node.Index = index;
			node.BelongsToGraph = this;
			nonTerminalNodes[index] = node;
			return node;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addNonTerminalNode() throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode addNonTerminalNode()
		{
			int index = HighestNonTerminalIndex;
			if (index > 0)
			{
				return addNonTerminalNode(index + 1);
			}
			return addNonTerminalNode(1);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode getNonTerminalNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode getNonTerminalNode(int index)
		{
			return nonTerminalNodes[index];
		}

		public virtual int HighestNonTerminalIndex
		{
			get
			{
				try
				{
					return nonTerminalNodes.lastKey();
				}
				catch (NoSuchElementException)
				{
					return 0;
				}
			}
		}

		public virtual ISet<int> NonTerminalIndices
		{
			get
			{
				return new SortedSet<int>(nonTerminalNodes.Keys);
			}
		}

		public virtual bool hasNonTerminals()
		{
			return nonTerminalNodes.Count > 0;
		}

		public virtual int nNonTerminals()
		{
			return nonTerminalNodes.Count;
		}

		public virtual PhraseStructureNode PhraseStructureRoot
		{
			get
			{
				return root;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge addPhraseStructureEdge(org.maltparser.core.syntaxgraph.node.PhraseStructureNode parent, org.maltparser.core.syntaxgraph.node.PhraseStructureNode child) throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge.Edge addPhraseStructureEdge(PhraseStructureNode parent, PhraseStructureNode child)
		{
			if (parent == null || child == null)
			{
				throw new MaltChainedException("Parent or child node is missing.");
			}
			else if (parent is NonTerminalNode && !child.Root)
			{
				Edge.Edge e = edgePool.checkOut();
				e.BelongsToGraph = this;
				e.setEdge((Node.Node)parent, (Node.Node)child, Edge_Fields.PHRASE_STRUCTURE_EDGE);
				graphEdges.Add(e);
				return e;
			}
			else
			{
				throw new MaltChainedException("Parent or child node is not of correct node type.");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removePhraseStructureEdge(org.maltparser.core.syntaxgraph.node.PhraseStructureNode parent, org.maltparser.core.syntaxgraph.node.PhraseStructureNode child) throws org.maltparser.core.exception.MaltChainedException
		public virtual void removePhraseStructureEdge(PhraseStructureNode parent, PhraseStructureNode child)
		{
			if (parent == null || child == null)
			{
				throw new MaltChainedException("Parent or child node is missing.");
			}
			else if (parent is NonTerminalNode && !child.Root)
			{
				foreach (Edge.Edge e in graphEdges)
				{
					if (e.Source == parent && e.Target == child)
					{
						e.clear();
						graphEdges.remove(e);
						if (e is GraphEdge)
						{
							edgePool.checkIn(e);
						}
					}
				}
			}
			else
			{
				throw new SyntaxGraphException("Head node is not a root node or a terminal node.");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge addSecondaryEdge(org.maltparser.core.syntaxgraph.node.ComparableNode source, org.maltparser.core.syntaxgraph.node.ComparableNode target) throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge.Edge addSecondaryEdge(ComparableNode source, ComparableNode target)
		{
			if (source == null || target == null)
			{
				throw new SyntaxGraphException("Head or dependent node is missing.");
			}
			else if (!target.Root)
			{
				Edge.Edge e = edgePool.checkOut();
				e.BelongsToGraph = this;
				e.setEdge((Node.Node)source, (Node.Node)target, Edge_Fields.SECONDARY_EDGE);
				graphEdges.Add(e);
				return e;
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeSecondaryEdge(org.maltparser.core.syntaxgraph.node.ComparableNode source, org.maltparser.core.syntaxgraph.node.ComparableNode target) throws org.maltparser.core.exception.MaltChainedException
		public virtual void removeSecondaryEdge(ComparableNode source, ComparableNode target)
		{
			if (source == null || target == null)
			{
				throw new SyntaxGraphException("Head or dependent node is missing.");
			}
			else if (!target.Root)
			{
				IEnumerator<Edge.Edge> ie = ((Node.Node)target).IncomingEdgeIterator;
				while (ie.MoveNext())
				{
					Edge.Edge e = ie.Current;
					if (e.Source == source)
					{
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
						ie.remove();
						graphEdges.remove(e);
						edgePool.checkIn(e);
					}
				}
			}
		}

		public virtual int nEdges()
		{
			return graphEdges.Count;
		}

		public virtual SortedSet<Edge.Edge> Edges
		{
			get
			{
				return graphEdges;
			}
		}

		public virtual bool Continuous
		{
			get
			{
				foreach (int index in nonTerminalNodes.Keys)
				{
					NonTerminalNode node = nonTerminalNodes[index];
					if (!node.Continuous)
					{
						return false;
					}
				}
				return true;
			}
		}

		public virtual bool ContinuousExcludeTerminalsAttachToRoot
		{
			get
			{
				foreach (int index in nonTerminalNodes.Keys)
				{
					NonTerminalNode node = nonTerminalNodes[index];
					if (!node.ContinuousExcludeTerminalsAttachToRoot)
					{
						return false;
					}
				}
				return true;
			}
		}

	//	public void makeContinuous() throws MaltChainedException {
	//		if (root != null) {
	//			root.reArrangeChildrenAccordingToLeftAndRightProperDesendant();
	//		}
	//	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void Clear()
		{
			edgePool.checkInAll();
			graphEdges.Clear();
			root.clear();
			root.BelongsToGraph = this;
			nonTerminalPool.checkInAll();
			nonTerminalNodes.Clear();
			base.Clear();
		}

		public virtual string toStringTerminalNode(TokenNode node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode depnode = node;
			DependencyNode depnode = node;

			sb.Append(node.ToString().Trim());
			if (depnode.hasHead())
			{
				sb.Append('\t');
				try
				{
					sb.Append(depnode.Head.Index);
					sb.Append('\t');
					sb.Append(depnode.HeadEdge.ToString());
				}
				catch (MaltChainedException e)
				{
					Console.Error.WriteLine(e);
				}
			}
			sb.Append('\n');

			return sb.ToString();
		}

		public virtual string toStringNonTerminalNode(NonTerminalNode node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();

			sb.Append(node.ToString().Trim());
			sb.Append('\n');
			IEnumerator<Edge.Edge> ie = ((Node.Node)node).OutgoingEdgeIterator;
			while (ie.MoveNext())
			{
				Edge.Edge e = ie.Current;
				if (e.Target is TokenNode)
				{
					sb.Append("   T");
					sb.Append(e.Target.Index);
				}
				if (e.Target is NonTerminalNode)
				{
					sb.Append("   N");
					sb.Append(e.Target.Index);
				}
				sb.Append('\t');
				sb.Append(e.ToString());
				sb.Append('\n');
			}
			return sb.ToString();
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			foreach (int index in terminalNodes.Keys)
			{
				sb.Append(toStringTerminalNode(terminalNodes[index]));
			}
			sb.Append('\n');
			sb.Append(toStringNonTerminalNode((NonTerminalNode)PhraseStructureRoot));
			foreach (int index in nonTerminalNodes.Keys)
			{
				sb.Append(toStringNonTerminalNode(nonTerminalNodes[index]));
			}

			return sb.ToString();
		}
	}

}