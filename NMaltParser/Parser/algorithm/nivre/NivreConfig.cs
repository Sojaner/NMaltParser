using System.Collections.Generic;
using System.Text;

namespace org.maltparser.parser.algorithm.nivre
{
    using  core.syntaxgraph;
	using  core.syntaxgraph.node;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class NivreConfig : ParserConfiguration
	{
		private readonly Stack<DependencyNode> stack;
		private readonly Stack<DependencyNode> input;
		private DependencyStructure dependencyGraph;
		private readonly bool allowRoot;
		private readonly bool allowReduce;
		private readonly bool enforceTree;

		private bool end; //Added

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public NivreConfig(boolean allowRoot, boolean allowReduce, boolean enforceTree) throws org.maltparser.core.exception.MaltChainedException
		public NivreConfig(bool allowRoot, bool allowReduce, bool enforceTree) : base()
		{
			stack = new Stack<DependencyNode>();
			input = new Stack<DependencyNode>();
			this.allowRoot = allowRoot;
			this.allowReduce = allowReduce;
			this.enforceTree = enforceTree;
			end = false; // Added
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
				if (EnforceTree)
				{
					return input.Count == 0 && stack.Count == 1;
				}
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
				dependencyGraph = value;
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NivreConfig nivreConfig = (NivreConfig)parserConfiguration;
				NivreConfig nivreConfig = (NivreConfig)parserConfiguration;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> sourceStack = nivreConfig.getStack();
				Stack<DependencyNode> sourceStack = nivreConfig.Stack;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> sourceInput = nivreConfig.getInput();
				Stack<DependencyNode> sourceInput = nivreConfig.Input;
				DependencyGraph = nivreConfig.DependencyGraph;
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
					{ // added !node.hasHead()
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
				{ // added !node.hasHead()
					input.Push(node);
				}
			}
		}

		public virtual bool End
		{
			set
			{
				end = value;
			}
			get
			{
				return end;
			}
		}


		public virtual bool AllowRoot
		{
			get
			{
				return allowRoot;
			}
		}

		public virtual bool AllowReduce
		{
			get
			{
					return allowReduce;
			}
		}

		public virtual bool EnforceTree
		{
			get
			{
				return enforceTree;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
			stack.Clear();
			input.Clear();
			historyNode = null;
			end = false; // Added
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
			NivreConfig that = (NivreConfig)obj;

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
			sb.Append(", ");
			sb.Append(allowRoot);
			sb.Append(", ");
			sb.Append(allowReduce);
			sb.Append(", ");
			sb.Append(enforceTree);
			return sb.ToString();
		}
	}

}