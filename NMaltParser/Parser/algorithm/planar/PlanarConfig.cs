using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.parser.algorithm.planar
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using DependencyStructure = org.maltparser.core.syntaxgraph.DependencyStructure;
	using DependencyNode = org.maltparser.core.syntaxgraph.node.DependencyNode;
	/// <summary>
	/// @author Carlos Gomez Rodriguez
	/// 
	/// </summary>
	public class PlanarConfig : ParserConfiguration
	{
		// Root Handling
		/*
		public static final int STRICT = 1; //root tokens unattached, Reduce not permissible
		public static final int RELAXED = 2; //root tokens unattached, Reduce permissible
		public static final int NORMAL = 3; //root tokens attached to Root with RightArc
		*/

		//Connectedness enforcing
		public const int NO_CONNECTEDNESS = 1;
		public const int REDUCE_ONLY = 2; //connectedness enforced on reduce only
		public const int FULL_CONNECTEDNESS = 3; //connectedness enforced on shift and reduce

		// Root Handling
		public const int NORMAL = 1; //root tokens attached to Root with RightArc
		public const int RELAXED = 2; //root tokens unattached

		//Constraints
		public readonly bool SINGLE_HEAD = true; //single-head constraint
		public bool noCoveredRoots = false; //no-covered-roots constraint
		public bool acyclicity = true; //acyclicity constraint
		public int connectedness = NO_CONNECTEDNESS; //connectedness constraint


		private readonly Stack<DependencyNode> stack;
		private readonly Stack<DependencyNode> input;
		private DependencyStructure dependencyGraph;


		//root handling: explicitly create links to dummy root or not?
		private int rootHandling;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PlanarConfig(String noCoveredRoots, String acyclicity, String connectedness, String rootHandling) throws org.maltparser.core.exception.MaltChainedException
		public PlanarConfig(string noCoveredRoots, string acyclicity, string connectedness, string rootHandling) : base()
		{
			stack = new Stack<DependencyNode>();
			input = new Stack<DependencyNode>();
			setRootHandling(rootHandling);
			NoCoveredRoots = Convert.ToBoolean(noCoveredRoots);
			Acyclicity = Convert.ToBoolean(acyclicity);
			Connectedness = connectedness;
		}

		public virtual Stack<DependencyNode> Stack
		{
			get
			{
				return stack;
			}
		}

		public virtual Stack<DependencyNode> Input
		{
			get
			{
				return input;
			}
		}

		public virtual DependencyStructure DependencyStructure
		{
			get
			{
				return dependencyGraph;
			}
		}

		public override bool TerminalState
		{
			get
			{
				return input.Count == 0;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getStackNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode getStackNode(int index)
		{
			if (index < 0)
			{
				throw new ParsingException("Stack index must be non-negative in feature specification. ");
			}
			if (stack.Count - index > 0)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
				return stack.get(stack.Count - 1 - index);
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getInputNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode getInputNode(int index)
		{
			if (index < 0)
			{
				throw new ParsingException("Input index must be non-negative in feature specification. ");
			}
			if (input.Count - index > 0)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
				return input.get(input.Count - 1 - index);
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDependencyGraph(org.maltparser.core.syntaxgraph.DependencyStructure source) throws org.maltparser.core.exception.MaltChainedException
		public override DependencyStructure DependencyGraph
		{
			set
			{
				this.dependencyGraph = value;
		//		dependencyGraph.clear();
		//		for (int index : value.getTokenIndices()) {
		//			DependencyNode gnode = value.getDependencyNode(index);
		//			DependencyNode pnode = dependencyGraph.addDependencyNode(gnode.getIndex());
		//			for (SymbolTable table : gnode.getLabelTypes()) {
		//				pnode.addLabel(table, gnode.getLabelSymbol(table));
		//			}
		//			
		//			if (gnode.hasHead()) {
		//				Edge s = gnode.getHeadEdge();
		//				Edge t = dependencyGraph.addDependencyEdge(s.getSource().getIndex(), s.getTarget().getIndex());
		//				
		//				for (SymbolTable table : s.getLabelTypes()) {
		//					t.addLabel(table, s.getLabelSymbol(table));
		//				}
		//			}
		//		}
		//		for (SymbolTable table : value.getDefaultRootEdgeLabels().keySet()) {
		//			dependencyGraph.setDefaultRootEdgeLabel(table, value.getDefaultRootEdgeLabelSymbol(table));
		//		}
			}
			get
			{
				return dependencyGraph;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(org.maltparser.parser.ParserConfiguration parserConfiguration) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initialize(ParserConfiguration parserConfiguration)
		{
			if (parserConfiguration != null)
			{
				PlanarConfig planarConfig = (PlanarConfig)parserConfiguration;
				Stack<DependencyNode> sourceStack = planarConfig.Stack;
				Stack<DependencyNode> sourceInput = planarConfig.Input;
				DependencyGraph = planarConfig.DependencyGraph;
				for (int i = 0, n = sourceStack.Count; i < n; i++)
				{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
					stack.Push(dependencyGraph.getDependencyNode(sourceStack.get(i).Index));
				}
				for (int i = 0, n = sourceInput.Count; i < n; i++)
				{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
					input.Push(dependencyGraph.getDependencyNode(sourceInput.get(i).Index));
				}
			}
			else
			{
				stack.Push(dependencyGraph.DependencyRoot);
				for (int i = dependencyGraph.HighestTokenIndex; i > 0; i--)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode node = dependencyGraph.getDependencyNode(i);
					DependencyNode node = dependencyGraph.getDependencyNode(i);
					if (node != null && !node.hasHead())
					{
						input.Push(node);
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize() throws org.maltparser.core.exception.MaltChainedException
		public override void initialize()
		{
			stack.Push(dependencyGraph.DependencyRoot);
			for (int i = dependencyGraph.HighestTokenIndex; i > 0; i--)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode node = dependencyGraph.getDependencyNode(i);
				DependencyNode node = dependencyGraph.getDependencyNode(i);
				if (node != null && !node.hasHead())
				{
					input.Push(node);
				}
			}
		}
		/*
		public int getRootHandling() {
			return rootHandling;
		}
		*/

		public virtual bool requiresSingleHead()
		{
			return SINGLE_HEAD;
		}

		public virtual bool requiresNoCoveredRoots()
		{
			return noCoveredRoots;
		}

		public virtual bool requiresAcyclicity()
		{
			return acyclicity;
		}

		public virtual bool requiresConnectednessCheckOnReduce()
		{
			return connectedness != NO_CONNECTEDNESS;
		}

		public virtual bool requiresConnectednessCheckOnShift()
		{
			return connectedness == FULL_CONNECTEDNESS;
		}

		public virtual bool NoCoveredRoots
		{
			set
			{
				noCoveredRoots = value;
			}
		}

		public virtual bool Acyclicity
		{
			set
			{
				acyclicity = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setConnectedness(String conn) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual string Connectedness
		{
			set
			{
				if (value.Equals("none", StringComparison.OrdinalIgnoreCase))
				{
					connectedness = NO_CONNECTEDNESS;
				}
				else if (value.Equals("reduceonly", StringComparison.OrdinalIgnoreCase))
				{
					connectedness = REDUCE_ONLY;
				}
				else if (value.Equals("full", StringComparison.OrdinalIgnoreCase))
				{
					connectedness = FULL_CONNECTEDNESS;
				}
				else
				{
					throw new ParsingException("The connectedness constraint option '" + value + "' is unknown");
				}
			}
		}

		/*
		public void setRootHandling(int rootHandling) {
			this.rootHandling = rootHandling;
		}
		
		protected void setRootHandling(String rh) throws MaltChainedException {
			if (rh.equalsIgnoreCase("strict")) {
				rootHandling = STRICT;
			} else if (rh.equalsIgnoreCase("relaxed")) {
				rootHandling = RELAXED;
			} else if (rh.equalsIgnoreCase("normal")) {
				rootHandling = NORMAL;
			} else {
				throw new ParsingException("The root handling '"+rh+"' is unknown");
			}
		}
		*/

		public virtual int getRootHandling()
		{
			return rootHandling;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setRootHandling(String rh) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void setRootHandling(string rh)
		{
			if (rh.Equals("relaxed", StringComparison.OrdinalIgnoreCase))
			{
				rootHandling = RELAXED;
			}
			else if (rh.Equals("normal", StringComparison.OrdinalIgnoreCase))
			{
				rootHandling = NORMAL;
			}
			else
			{
				throw new ParsingException("The root handling '" + rh + "' is unknown");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
	//		dependencyGraph.clear();
			stack.Clear();
			input.Clear();
			historyNode = null;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			PlanarConfig that = (PlanarConfig)obj;

			if (stack.Count != that.Stack.Count)
			{
				return false;
			}
			if (input.Count != that.Input.Count)
			{
				return false;
			}
			if (dependencyGraph.nEdges() != that.DependencyGraph.nEdges())
			{
				return false;
			}
			for (int i = 0; i < stack.Count; i++)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
				if (stack.get(i).Index != that.Stack.get(i).Index)
				{
					return false;
				}
			}
			for (int i = 0; i < input.Count; i++)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
				if (input.get(i).Index != that.Input.get(i).Index)
				{
					return false;
				}
			}
			return dependencyGraph.Edges.Equals(that.DependencyGraph.Edges);
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(stack.Count);
			sb.Append(", ");
			sb.Append(input.Count);
			sb.Append(", ");
			sb.Append(dependencyGraph.nEdges());
			return sb.ToString();
		}
	}

}