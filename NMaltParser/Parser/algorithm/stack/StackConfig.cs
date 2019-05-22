using System.Collections.Generic;
using System.Text;

namespace org.maltparser.parser.algorithm.stack
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using DependencyStructure = org.maltparser.core.syntaxgraph.DependencyStructure;
	using DependencyNode = org.maltparser.core.syntaxgraph.node.DependencyNode;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class StackConfig : ParserConfiguration
	{
		private readonly Stack<DependencyNode> stack;
		private readonly Stack<DependencyNode> input;
		private DependencyStructure dependencyGraph;
		private int lookahead;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public StackConfig() throws org.maltparser.core.exception.MaltChainedException
		public StackConfig() : base()
		{
			stack = new Stack<DependencyNode>();
			input = new Stack<DependencyNode>();
	//		dependencyGraph = new DependencyGraph(symbolTableHandler);
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
				return input.Count == 0 && stack.Count == 1;
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
//ORIGINAL LINE: private org.maltparser.core.syntaxgraph.node.DependencyNode getBufferNode(int index) throws org.maltparser.core.exception.MaltChainedException
		private DependencyNode getBufferNode(int index)
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
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getLookaheadNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode getLookaheadNode(int index)
		{
			return getBufferNode(lookahead + index);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getInputNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode getInputNode(int index)
		{
			if (index < lookahead)
			{
				return getBufferNode(index);
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

		public virtual void lookaheadIncrement()
		{
			lookahead++;
		}

		public virtual void lookaheadDecrement()
		{
			if (lookahead > 0)
			{
				lookahead--;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(org.maltparser.parser.ParserConfiguration parserConfiguration) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initialize(ParserConfiguration parserConfiguration)
		{
			if (parserConfiguration != null)
			{
				StackConfig config = (StackConfig)parserConfiguration;
				Stack<DependencyNode> sourceStack = config.Stack;
				Stack<DependencyNode> sourceInput = config.Input;
				DependencyGraph = config.DependencyGraph;
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
			StackConfig that = (StackConfig)obj;

			if (lookahead != that.lookahead)
			{
				return false;
			}
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
	//		dependencyGraph.clear();
			stack.Clear();
			input.Clear();
			historyNode = null;
			lookahead = 0;
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