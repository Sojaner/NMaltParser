using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Parser.Algorithm.Covington
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class CovingtonConfig : ParserConfiguration
	{
		private readonly List<DependencyNode> input;
		private int right;
		private int left;
		private int leftstop;
		private int rightstop;
		private DependencyStructure dependencyGraph;
		private readonly bool allowRoot;
		private readonly bool allowShift;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CovingtonConfig(boolean cr, boolean cs) throws org.maltparser.core.exception.MaltChainedException
		public CovingtonConfig(bool cr, bool cs) : base()
		{
			input = new List<DependencyNode>();
			allowRoot = cr;
			allowShift = cs;
		}

		public virtual DependencyStructure DependencyStructure
		{
			get
			{
				return dependencyGraph;
			}
		}

		public virtual List<DependencyNode> Input
		{
			get
			{
				return input;
			}
		}

		public override bool TerminalState
		{
			get
			{
				return right > rightstop;
			}
		}

		public virtual int Right
		{
			get
			{
				return right;
			}
			set
			{
				right = value;
			}
		}


		public virtual int Left
		{
			get
			{
				return left;
			}
			set
			{
				left = value;
			}
		}


		public virtual int Leftstop
		{
			get
			{
				return leftstop;
			}
		}

		public virtual int Rightstop
		{
			get
			{
				return rightstop;
			}
		}

		public virtual bool AllowRoot
		{
			get
			{
				return allowRoot;
			}
		}

	//	public void setAllowRoot(boolean allowRoot) {
	//		this.allowRoot = allowRoot;
	//	}

		public virtual bool AllowShift
		{
			get
			{
				return allowShift;
			}
		}

	//	public void setAllowShift(boolean allowShift) {
	//		this.allowShift = allowShift;
	//	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getLeftNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode getLeftNode(int index)
		{
			if (index < 0)
			{
				throw new ParsingException("Left index must be non-negative in feature specification. ");
			}
			if (left - index >= 0)
			{
				return input[left - index];
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getRightNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode getRightNode(int index)
		{
			if (index < 0)
			{
				throw new ParsingException("Right index must be non-negative in feature specification. ");
			}
			if (right + index < input.Count)
			{
				return input[right + index];
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getLeftContextNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode getLeftContextNode(int index)
		{
			if (index < 0)
			{
				throw new ParsingException("LeftContext index must be non-negative in feature specification. ");
			}

			int tmpindex = 0;
			for (int i = left + 1; i < right; i++)
			{
				if (!input[i].hasAncestorInside(left, right))
				{
					if (tmpindex == index)
					{
						return input[i];
					}
					else
					{
						tmpindex++;
					}
				}
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getRightContextNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode getRightContextNode(int index)
		{
			if (index < 0)
			{
				throw new ParsingException("RightContext index must be non-negative in feature specification. ");
			}
			int tmpindex = 0;
			for (int i = right - 1; i > left; i--)
			{
				if (!input[i].hasAncestorInside(left, right))
				{
					if (tmpindex == index)
					{
						return input[i];
					}
					else
					{
						tmpindex++;
					}
				}
			}
			return null;
		}

		public virtual DependencyNode LeftTarget
		{
			get
			{
				return input[left];
			}
		}

		public virtual DependencyNode RightTarget
		{
			get
			{
				return input[right];
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDependencyGraph(org.maltparser.core.syntaxgraph.DependencyStructure source) throws org.maltparser.core.exception.MaltChainedException
		public override DependencyStructure DependencyGraph
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
				CovingtonConfig covingtonConfig = (CovingtonConfig)parserConfiguration;
				List<DependencyNode> sourceInput = covingtonConfig.Input;
				DependencyGraph = covingtonConfig.DependencyGraph;
				for (int i = 0, n = sourceInput.Count; i < n; i++)
				{
					input.Add(dependencyGraph.getDependencyNode(sourceInput[i].Index));
				}
				left = covingtonConfig.Left;
				right = covingtonConfig.Right;
				rightstop = covingtonConfig.Rightstop;
				leftstop = covingtonConfig.Leftstop;
			}
			else
			{
				for (int i = 0, n = dependencyGraph.HighestTokenIndex; i <= n; i++)
				{
					DependencyNode node = dependencyGraph.getDependencyNode(i);
					if (node != null)
					{
						input.Add(node);
					}
				}
				if (allowRoot == true)
				{
					leftstop = 0;
				}
				else
				{
					leftstop = 1;
				}
				rightstop = dependencyGraph.HighestTokenIndex;
				left = leftstop;
				right = left + 1;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize() throws org.maltparser.core.exception.MaltChainedException
		public override void initialize()
		{
			for (int i = 0, n = dependencyGraph.HighestTokenIndex; i <= n; i++)
			{
				DependencyNode node = dependencyGraph.getDependencyNode(i);
				if (node != null)
				{
					input.Add(node);
				}
			}
			if (allowRoot == true)
			{
				leftstop = 0;
			}
			else
			{
				leftstop = 1;
			}
			rightstop = dependencyGraph.HighestTokenIndex;
			left = leftstop;
			right = left + 1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
	//		dependencyGraph.clear();
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
			CovingtonConfig that = (CovingtonConfig)obj;

			if (input.Count != that.Input.Count)
			{
				return false;
			}
			if (dependencyGraph.nEdges() != that.DependencyGraph.nEdges())
			{
				return false;
			}
			for (int i = 0; i < input.Count; i++)
			{
				if (input[i].Index != that.Input[i].Index)
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
			sb.Append(input.Count);
			sb.Append(", ");
			sb.Append(dependencyGraph.nEdges());
			return sb.ToString();
		}

	}

}