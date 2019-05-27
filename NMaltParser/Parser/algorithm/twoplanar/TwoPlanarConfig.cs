using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Parser.Algorithm.TwoPlanar
{
    /// <summary>
	/// @author Carlos Gomez Rodriguez
	/// 
	/// </summary>
	public class TwoPlanarConfig : ParserConfiguration
	{


		//Connectedness enforcing
		/*
		public static final int NO_CONNECTEDNESS = 1;
		public static final int REDUCE_ONLY = 2; //connectedness enforced on reduce only
		public static final int FULL_CONNECTEDNESS = 3; //connectedness enforced on shift and reduce
		*/

		// Root Handling
		public const int NORMAL = 1; //root tokens attached to Root with RightArc
		public const int RELAXED = 2; //root tokens unattached

		//Constraints
		public readonly bool SINGLE_HEAD = true; //single-head constraint
		public bool noCoveredRoots = false; //no-covered-roots constraint
		public bool acyclicity = true; //acyclicity constraint

		//public int connectedness = NO_CONNECTEDNESS; //connectedness constraint

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		public bool reduceAfterSwitch_Renamed = false;


		private readonly Stack<DependencyNode> firstStack;
		private readonly Stack<DependencyNode> secondStack;

		public const bool FIRST_STACK = false;
		public const bool SECOND_STACK = true;

		private bool activeStack;

		private Stack<DependencyNode> input;

		private IDependencyStructure dependencyGraph;

		//root handling: explicitly create links to dummy root or not?
		private int rootHandling;

		//needed to disallow two consecutive switches:
		private int lastAction;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TwoPlanarConfig(String noCoveredRoots, String acyclicity, String reduceAfterSwitch, String rootHandling) throws org.maltparser.core.exception.MaltChainedException
		public TwoPlanarConfig(string noCoveredRoots, string acyclicity, string reduceAfterSwitch, string rootHandling) : base()
		{
			firstStack = new Stack<DependencyNode>();
			secondStack = new Stack<DependencyNode>();
			activeStack = FIRST_STACK;
			input = new Stack<DependencyNode>();
	//		dependencyGraph = new DependencyGraph(symbolTableHandler);
			setRootHandling(rootHandling);
			NoCoveredRoots = Convert.ToBoolean(noCoveredRoots);
			Acyclicity = Convert.ToBoolean(acyclicity);
			ReduceAfterSwitch = Convert.ToBoolean(reduceAfterSwitch);
		}

		public virtual void switchStacks()
		{
			activeStack = !activeStack;
		}

		public virtual bool reduceAfterSwitch()
		{
			return reduceAfterSwitch_Renamed;
		}

		public virtual bool ReduceAfterSwitch
		{
			set
			{
				reduceAfterSwitch_Renamed = value;
			}
		}

		public virtual int LastAction
		{
			set
			{
				lastAction = value;
			}
			get
			{
				return lastAction;
			}
		}


		public virtual bool StackActivityState
		{
			get
			{
				return activeStack;
			}
		}

		private Stack<DependencyNode> FirstStack
		{
			get
			{
				return firstStack;
			}
		}

		private Stack<DependencyNode> SecondStack
		{
			get
			{
				return secondStack;
			}
		}

		public virtual Stack<DependencyNode> ActiveStack
		{
			get
			{
				if (activeStack == FIRST_STACK)
				{
					return FirstStack;
				}
				else
				{
					return SecondStack;
				}
			}
		}

		public virtual Stack<DependencyNode> InactiveStack
		{
			get
			{
				if (activeStack == FIRST_STACK)
				{
					return SecondStack;
				}
				else
				{
					return FirstStack;
				}
			}
		}

		public virtual Stack<DependencyNode> Input
		{
			get
			{
				return input;
			}
		}

		public virtual IDependencyStructure DependencyStructure
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
//ORIGINAL LINE: private org.maltparser.core.syntaxgraph.node.DependencyNode getStackNode(java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> stack, int index) throws org.maltparser.core.exception.MaltChainedException
		private DependencyNode getStackNode(Stack<DependencyNode> stack, int index)
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
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getActiveStackNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode getActiveStackNode(int index)
		{
			return getStackNode(ActiveStack, index);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getInactiveStackNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode getInactiveStackNode(int index)
		{
			return getStackNode(InactiveStack, index);
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
		public override IDependencyStructure DependencyGraph
		{
			set
			{
				dependencyGraph = value;
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
				TwoPlanarConfig planarConfig = (TwoPlanarConfig)parserConfiguration;
				activeStack = planarConfig.activeStack;
				Stack<DependencyNode> sourceActiveStack = planarConfig.ActiveStack;
				Stack<DependencyNode> sourceInactiveStack = planarConfig.InactiveStack;
				Stack<DependencyNode> sourceInput = planarConfig.Input;
				DependencyGraph = planarConfig.DependencyGraph;
				for (int i = 0, n = sourceActiveStack.Count; i < n; i++)
				{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
					ActiveStack.Push(dependencyGraph.GetDependencyNode(sourceActiveStack.get(i).Index));
				}
				for (int i = 0, n = sourceInactiveStack.Count ; i < n ; i++)
				{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
					InactiveStack.Push(dependencyGraph.GetDependencyNode(sourceInactiveStack.get(i).Index));
				}
				for (int i = 0, n = sourceInput.Count; i < n; i++)
				{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
					input.Push(dependencyGraph.GetDependencyNode(sourceInput.get(i).Index));
				}
			}
			else
			{
				ActiveStack.Push(dependencyGraph.DependencyRoot);
				InactiveStack.Push(dependencyGraph.DependencyRoot);
				for (int i = dependencyGraph.HighestTokenIndex; i > 0; i--)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode node = dependencyGraph.getDependencyNode(i);
					DependencyNode node = dependencyGraph.GetDependencyNode(i);
					if (node != null)
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
			ActiveStack.Push(dependencyGraph.DependencyRoot);
			InactiveStack.Push(dependencyGraph.DependencyRoot);
			for (int i = dependencyGraph.HighestTokenIndex; i > 0; i--)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode node = dependencyGraph.getDependencyNode(i);
				DependencyNode node = dependencyGraph.GetDependencyNode(i);
				if (node != null)
				{
					input.Push(node);
				}
			}
		}

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

		//does not make much sense to enforce the no-covered-roots constraint in 2-planar parsing, it won't capture some 2-planar structures
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
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
	//		dependencyGraph.clear();
			ActiveStack.Clear();
			InactiveStack.Clear();
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
			if (GetType() != obj.GetType())
			{
				return false;
			}
			TwoPlanarConfig that = (TwoPlanarConfig)obj;

			if (ActiveStack.Count != that.ActiveStack.Count)
			{
				return false;
			}
			if (InactiveStack.Count != that.InactiveStack.Count)
			{
				return false;
			}
			if (input.Count != that.Input.Count)
			{
				return false;
			}
			if (dependencyGraph.NEdges() != that.DependencyGraph.NEdges())
			{
				return false;
			}
			for (int i = 0; i < ActiveStack.Count; i++)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
				if (ActiveStack.get(i).Index != that.ActiveStack.get(i).Index)
				{
					return false;
				}
			}
			for (int i = 0; i < InactiveStack.Count; i++)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
				if (InactiveStack.get(i).Index != that.InactiveStack.get(i).Index)
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
			sb.Append(ActiveStack.Count);
			sb.Append(", ");
			sb.Append(InactiveStack.Count);
			sb.Append(", ");
			sb.Append(input.Count);
			sb.Append(", ");
			sb.Append(dependencyGraph.NEdges());
			return sb.ToString();
		}
	}

}