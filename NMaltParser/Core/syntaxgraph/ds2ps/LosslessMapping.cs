using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Helper;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph.Ds2PS
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class LosslessMapping : Dependency2PhraseStructure
	{
		private string DEPREL = "DEPREL";
		private string PHRASE = "PHRASE";
		private string HEADREL = "HEADREL";
		private string ATTACH = "ATTACH";
		private string CAT = "CAT";
		private string EDGELABEL;
		private readonly char EMPTY_SPINE = '*';
		private readonly string EMPTY_LABEL = "??";
		private readonly char SPINE_ELEMENT_SEPARATOR = '|';
		private readonly char LABEL_ELEMENT_SEPARATOR = '~';
		private readonly char QUESTIONMARK = '?';
		private string optionString;
		private HeadRules.HeadRules headRules;
		private DataFormatInstance dependencyDataFormatInstance;
		private DataFormatInstance phraseStructuretDataFormatInstance;
		private SymbolTableHandler symbolTableHandler;
		private bool lockUpdate = false;
		private int nonTerminalCounter;
		private StringBuilder deprel;
		private StringBuilder headrel;
		private StringBuilder phrase;

		public LosslessMapping(DataFormatInstance dependencyDataFormatInstance, DataFormatInstance phraseStructuretDataFormatInstance, SymbolTableHandler symbolTableHandler)
		{
			this.symbolTableHandler = symbolTableHandler;
			DependencyDataFormatInstance = dependencyDataFormatInstance;
			PhraseStructuretDataFormatInstance = phraseStructuretDataFormatInstance;
			deprel = new StringBuilder();
			headrel = new StringBuilder();
			phrase = new StringBuilder();

			if (phraseStructuretDataFormatInstance.PhraseStructureEdgeLabelColumnDescriptionSet.Count == 1)
			{
				foreach (ColumnDescription column in phraseStructuretDataFormatInstance.PhraseStructureEdgeLabelColumnDescriptionSet)
				{
					EDGELABEL = column.Name;
				}
			}

			clear();
		}

		public virtual void clear()
		{
			nonTerminalCounter = 0;
		}

		public virtual string OptionString
		{
			get
			{
				return optionString;
			}
			set
			{
				optionString = value;
			}
		}


		public virtual DataFormatInstance DependencyDataFormatInstance
		{
			get
			{
				return dependencyDataFormatInstance;
			}
			set
			{
				dependencyDataFormatInstance = value;
			}
		}


		public virtual DataFormatInstance PhraseStructuretDataFormatInstance
		{
			get
			{
				return phraseStructuretDataFormatInstance;
			}
			set
			{
				phraseStructuretDataFormatInstance = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update(org.maltparser.core.syntaxgraph.MappablePhraseStructureGraph graph, org.maltparser.core.syntaxgraph.edge.Edge e, Object arg) throws org.maltparser.core.exception.MaltChainedException
		public virtual void update(MappablePhraseStructureGraph graph, Edge.Edge e, object arg)
		{
			if (lockUpdate == false)
			{
	//			if (e.getType() == Edge.PHRASE_STRUCTURE_EDGE && e.getSource() instanceof NonTerminalNode && lockUpdate == false) { 
	//				if(e.getTarget() instanceof TerminalNode) {
	//					PhraseStructureNode top = (PhraseStructureNode)e.getTarget(); 
	//					while (top.getParent() != null && ((NonTerminalNode)top.getParent()).getLexicalHead() == (PhraseStructureNode)e.getTarget()) {
	//						top = top.getParent();
	//					}
	//					updateDependenyGraph(graph, top);
	//				}
	//				else if (e.getSource().isRoot()) {
	//					updateDependenyGraph(graph, graph.getPhraseStructureRoot());
	//				}
	//			}
				if (e.Type == Edge_Fields.DEPENDENCY_EDGE && e.Source is DependencyNode && e.Target is DependencyNode)
				{
					if (e.Labeled && e.LabelSet.size() == 4)
					{
						updatePhraseStructureGraph(graph, (Edge.Edge)e, false);
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateDependenyGraph(org.maltparser.core.syntaxgraph.MappablePhraseStructureGraph graph, org.maltparser.core.syntaxgraph.node.PhraseStructureNode top) throws org.maltparser.core.exception.MaltChainedException
		public virtual void updateDependenyGraph(MappablePhraseStructureGraph graph, PhraseStructureNode top)
		{
			if (graph.nTokenNode() == 1 && graph.nNonTerminals() == 0)
			{
				// Special case when the root dominates direct a single terminal node
				Edge.Edge e = graph.addDependencyEdge(graph.DependencyRoot, graph.getDependencyNode(1));
				e.addLabel(graph.SymbolTables.getSymbolTable(DEPREL), graph.getDefaultRootEdgeLabelSymbol(graph.SymbolTables.getSymbolTable(DEPREL)));
				e.addLabel(graph.SymbolTables.getSymbolTable(HEADREL), graph.getDefaultRootEdgeLabelSymbol(graph.SymbolTables.getSymbolTable(HEADREL)));
				e.addLabel(graph.SymbolTables.getSymbolTable(PHRASE), "*");
	//			e.addLabel(graph.getSymbolTables().getSymbolTable(PHRASE), graph.getDefaultRootEdgeLabelSymbol(graph.getSymbolTables().getSymbolTable(PHRASE)));
				e.addLabel(graph.SymbolTables.getSymbolTable(ATTACH), graph.getDefaultRootEdgeLabelSymbol(graph.SymbolTables.getSymbolTable(ATTACH)));
			}
			else
			{
				updateDependencyEdges(graph, top);
				updateDependenyLabels(graph);
			}
		}



//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void updateDependencyEdges(org.maltparser.core.syntaxgraph.MappablePhraseStructureGraph graph, org.maltparser.core.syntaxgraph.node.PhraseStructureNode top) throws org.maltparser.core.exception.MaltChainedException
		private void updateDependencyEdges(MappablePhraseStructureGraph graph, PhraseStructureNode top)
		{
			if (top == null)
			{
				return;
			}
			DependencyNode head = null;
			DependencyNode dependent = null;
			if (top is NonTerminalNode)
			{
				foreach (PhraseStructureNode node in ((NonTerminalNode)top).Children)
				{
					if (node is NonTerminalNode)
					{
						updateDependencyEdges(graph,node);
					}
					else
					{
						head = ((NonTerminalNode)top).getLexicalHead(headRules);
						dependent = (DependencyNode)node;
						if (head != null && dependent != null && head != dependent)
						{
							lockUpdate = true;
							if (!dependent.hasHead())
							{
								graph.addDependencyEdge(head, dependent);
							}
							else if (head != dependent.Head)
							{
								graph.moveDependencyEdge(head, dependent);
							}
							lockUpdate = false;
						}
					}
				}
			}

			head = null;
			if (top.Parent != null)
			{
				head = ((NonTerminalNode)top.Parent).getLexicalHead(headRules);
			}
			else if (top.Root)
			{
				head = (DependencyNode)top;
			}

			if (top is NonTerminalNode)
			{
				dependent = ((NonTerminalNode)top).getLexicalHead(headRules);
			}
			else if (!top.Root)
			{
				dependent = (DependencyNode)top;
			}
			if (head != null && dependent != null && head != dependent)
			{
				lockUpdate = true;
				if (!dependent.hasHead())
				{
					graph.addDependencyEdge(head, dependent);
				}
				else if (head != dependent.Head)
				{
					graph.moveDependencyEdge(head, dependent);
				}
				lockUpdate = false;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void updateDependenyLabels(org.maltparser.core.syntaxgraph.MappablePhraseStructureGraph graph) throws org.maltparser.core.exception.MaltChainedException
		private void updateDependenyLabels(MappablePhraseStructureGraph graph)
		{
			foreach (int index in graph.TokenIndices)
			{
				PhraseStructureNode top = (PhraseStructureNode)graph.getTokenNode(index);

				while (top != null && top.Parent != null && graph.getTokenNode(index) == ((NonTerminalNode)top.Parent).getLexicalHead(headRules))
				{
					top = top.Parent;
				}
				lockUpdate = true;
				labelDependencyEdge(graph, graph.getTokenNode(index).HeadEdge, top);
				lockUpdate = false;
			}
		}


	//	private void updateDependenyLabels(MappablePhraseStructureGraph graph, PhraseStructureNode top) throws MaltChainedException {
	//		if (top == null) {
	//			return;
	//		}
	//		DependencyNode head = null;
	//		DependencyNode dependent = null;
	//		if (top instanceof NonTerminalNode) {
	//			for (PhraseStructureNode node : ((NonTerminalNode)top).getChildren()) {
	//				if (node instanceof NonTerminalNode) {
	//					updateDependenyLabels(graph, node);
	//				} else {
	//					head = ((NonTerminalNode)top).getLexicalHead(headRules);
	//					dependent = (DependencyNode)node;
	//					if (head != null && dependent != null && head != dependent) {
	//						lockUpdate = true;
	//						if (dependent.hasHead()) {
	//							Edge e = dependent.getHeadEdge();
	//							labelDependencyEdge(graph, e, node);
	//						}
	//						lockUpdate = false;
	//					}
	//				}
	//			}
	//		}
	//		
	//		dependent = null;
	//		if (top instanceof NonTerminalNode) {
	//			dependent = ((NonTerminalNode)top).getLexicalHead(headRules);
	//		}
	//
	//		if (dependent != null) {
	//			lockUpdate = true;
	//			if (dependent.hasHead()) {
	//				Edge e = dependent.getHeadEdge();
	//				labelDependencyEdge(graph, e, top);
	//			}
	//			lockUpdate = false;
	//		}
	//	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void labelDependencyEdge(org.maltparser.core.syntaxgraph.MappablePhraseStructureGraph graph, org.maltparser.core.syntaxgraph.edge.Edge e, org.maltparser.core.syntaxgraph.node.PhraseStructureNode top) throws org.maltparser.core.exception.MaltChainedException
		private void labelDependencyEdge(MappablePhraseStructureGraph graph, Edge.Edge e, PhraseStructureNode top)
		{
			if (e == null)
			{
				return;
			}
			SymbolTableHandler symbolTables = graph.SymbolTables;
			deprel.Length = 0;
			phrase.Length = 0;
			headrel.Length = 0;

			e.removeLabel(symbolTables.getSymbolTable(DEPREL));
			e.removeLabel(symbolTables.getSymbolTable(HEADREL));
			e.removeLabel(symbolTables.getSymbolTable(PHRASE));
			e.removeLabel(symbolTables.getSymbolTable(ATTACH));

			int i = 0;
			SortedDictionary<string, SymbolTable> edgeLabelSymbolTables = phraseStructuretDataFormatInstance.getPhraseStructureEdgeLabelSymbolTables(symbolTableHandler);
			SortedDictionary<string, SymbolTable> nodeLabelSymbolTables = phraseStructuretDataFormatInstance.getPhraseStructureNodeLabelSymbolTables(symbolTableHandler);
			if (!top.Root)
			{
				foreach (string name in edgeLabelSymbolTables.Keys)
				{
					if (top.hasParentEdgeLabel(symbolTables.getSymbolTable(name)))
					{
						deprel.Append(top.getParentEdgeLabelSymbol(symbolTables.getSymbolTable(name)));
					}
					else
					{
						deprel.Append(EMPTY_LABEL);
					}
					i++;
					if (i < edgeLabelSymbolTables.Count)
					{
						deprel.Append(LABEL_ELEMENT_SEPARATOR);
					}
				}
				if (deprel.Length != 0)
				{
					e.addLabel(symbolTables.getSymbolTable(DEPREL), deprel.ToString());
				}
			}
			else
			{
				string deprelDefaultRootLabel = graph.getDefaultRootEdgeLabelSymbol(symbolTables.getSymbolTable(DEPREL));
				if (!ReferenceEquals(deprelDefaultRootLabel, null))
				{
					e.addLabel(symbolTables.getSymbolTable(DEPREL), deprelDefaultRootLabel);
				}
				else
				{
					e.addLabel(symbolTables.getSymbolTable(DEPREL), EMPTY_LABEL);
				}
			}
			PhraseStructureNode tmp = (PhraseStructureNode)e.Target;
			while (tmp != top && tmp.Parent != null)
			{ // && !tmp.getParent().isRoot()) {
				i = 0;
				foreach (string name in edgeLabelSymbolTables.Keys)
				{
					if (tmp.hasParentEdgeLabel(symbolTables.getSymbolTable(name)))
					{
						headrel.Append(tmp.getParentEdgeLabelSymbol(symbolTables.getSymbolTable(name)));
					}
					else
					{
						headrel.Append(EMPTY_LABEL);
					}
					i++;
					if (i < edgeLabelSymbolTables.Count)
					{
						headrel.Append(LABEL_ELEMENT_SEPARATOR);
					}
				}
				i = 0;
				headrel.Append(SPINE_ELEMENT_SEPARATOR);
				foreach (string name in nodeLabelSymbolTables.Keys)
				{
					if (tmp.Parent.hasLabel(symbolTables.getSymbolTable(name)))
					{
						phrase.Append(tmp.Parent.getLabelSymbol(symbolTables.getSymbolTable(name)));
					}
					else
					{
						if (tmp.Parent.Root)
						{
							string deprelDefaultRootLabel = graph.getDefaultRootEdgeLabelSymbol(symbolTables.getSymbolTable(PHRASE));
							if (!ReferenceEquals(deprelDefaultRootLabel, null))
							{
								phrase.Append(deprelDefaultRootLabel);
							}
							else
							{
								phrase.Append(EMPTY_LABEL);
							}
						}
						else
						{
							phrase.Append(EMPTY_LABEL);
						}
					}
					i++;
					if (i < nodeLabelSymbolTables.Count)
					{
						phrase.Append(LABEL_ELEMENT_SEPARATOR);
					}
				}
				phrase.Append(SPINE_ELEMENT_SEPARATOR);
				tmp = tmp.Parent;
			}
			if (phrase.Length == 0)
			{
				headrel.Append(EMPTY_SPINE);
				phrase.Append(EMPTY_SPINE);
			}
			else
			{
				headrel.Length = headrel.Length - 1;
				phrase.Length = phrase.Length - 1;
			}
			e.addLabel(symbolTables.getSymbolTable(HEADREL), headrel.ToString());
			e.addLabel(symbolTables.getSymbolTable(PHRASE), phrase.ToString());
			int a = 0;
			tmp = (PhraseStructureNode)e.Source;
			while (top.Parent != null && tmp.Parent != null && tmp.Parent != top.Parent)
			{
				a++;
				tmp = tmp.Parent;
			}
			e.addLabel(symbolTables.getSymbolTable(ATTACH), Convert.ToString(a));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void connectUnattachedSpines(org.maltparser.core.syntaxgraph.MappablePhraseStructureGraph graph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void connectUnattachedSpines(MappablePhraseStructureGraph graph)
		{
			connectUnattachedSpines(graph, graph.DependencyRoot);

			if (!graph.PhraseStructureRoot.Labeled)
			{
				graph.PhraseStructureRoot.addLabel(graph.SymbolTables.addSymbolTable(CAT), graph.getDefaultRootEdgeLabelSymbol(graph.SymbolTables.getSymbolTable(PHRASE)));

			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void connectUnattachedSpines(org.maltparser.core.syntaxgraph.MappablePhraseStructureGraph graph, org.maltparser.core.syntaxgraph.node.DependencyNode depNode) throws org.maltparser.core.exception.MaltChainedException
		private void connectUnattachedSpines(MappablePhraseStructureGraph graph, DependencyNode depNode)
		{
			if (!depNode.Root)
			{
				PhraseStructureNode dependentSpine = (PhraseStructureNode)depNode;
				while (dependentSpine.Parent != null)
				{
					dependentSpine = dependentSpine.Parent;
				}
				if (!dependentSpine.Root)
				{
					updatePhraseStructureGraph(graph,depNode.HeadEdge,true);
				}
			}
			for (int i = 0; i < depNode.LeftDependentCount; i++)
			{
				connectUnattachedSpines(graph, depNode.getLeftDependent(i));
			}
			for (int i = depNode.RightDependentCount - 1; i >= 0 ; i--)
			{
				connectUnattachedSpines(graph, depNode.getRightDependent(i));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updatePhraseStructureGraph(org.maltparser.core.syntaxgraph.MappablePhraseStructureGraph graph, org.maltparser.core.syntaxgraph.edge.Edge depEdge, boolean attachHeadSpineToRoot) throws org.maltparser.core.exception.MaltChainedException
		public virtual void updatePhraseStructureGraph(MappablePhraseStructureGraph graph, Edge.Edge depEdge, bool attachHeadSpineToRoot)
		{
			PhraseStructureNode dependentSpine = (PhraseStructureNode)depEdge.Target;

			if (((PhraseStructureNode)depEdge.Target).Parent == null)
			{
				// Restore dependent spine
				string phraseSpineLabel = null;
				string edgeSpineLabel = null;
				int empty_label = 0;

				if (depEdge.hasLabel(graph.SymbolTables.getSymbolTable(PHRASE)))
				{
					phraseSpineLabel = depEdge.getLabelSymbol(graph.SymbolTables.getSymbolTable(PHRASE));
				}
				if (depEdge.hasLabel(graph.SymbolTables.getSymbolTable(HEADREL)))
				{
					edgeSpineLabel = depEdge.getLabelSymbol(graph.SymbolTables.getSymbolTable(HEADREL));
				}
				if (!ReferenceEquals(phraseSpineLabel, null) && phraseSpineLabel.Length > 0 && phraseSpineLabel[0] != EMPTY_SPINE)
				{
					int ps = 0, es = 0, i = 0, j = 0, n = phraseSpineLabel.Length - 1, m = edgeSpineLabel.Length - 1;
					PhraseStructureNode child = (PhraseStructureNode)depEdge.Target;
					while (true)
					{
						while (i <= n && phraseSpineLabel[i] != SPINE_ELEMENT_SEPARATOR)
						{
							if (phraseSpineLabel[i] == QUESTIONMARK)
							{
								empty_label++;
							}
							else
							{
								empty_label = 0;
							}
							i++;
						}
						if (depEdge.Source.Root && i >= n)
						{
							dependentSpine = graph.PhraseStructureRoot;
						}
						else
						{
							dependentSpine = graph.addNonTerminalNode(++nonTerminalCounter);
						}

						if (empty_label != 2 && ps != i)
						{
							dependentSpine.addLabel(graph.SymbolTables.addSymbolTable(CAT), phraseSpineLabel.Substring(ps, i - ps));
						}

						empty_label = 0;
						if (!ReferenceEquals(edgeSpineLabel, null))
						{
							while (j <= m && edgeSpineLabel[j] != SPINE_ELEMENT_SEPARATOR)
							{
								if (edgeSpineLabel[j] == QUESTIONMARK)
								{
									empty_label++;
								}
								else
								{
									empty_label = 0;
								}
								j++;
							}
						}
						lockUpdate = true;
						Edge.Edge e = graph.addPhraseStructureEdge(dependentSpine, child);
						if (empty_label != 2 && es != j && !ReferenceEquals(edgeSpineLabel, null) && e != null)
						{
							e.addLabel(graph.SymbolTables.addSymbolTable(EDGELABEL), edgeSpineLabel.Substring(es, j - es));
						}
						else if (es == j)
						{
							e.addLabel(graph.SymbolTables.addSymbolTable(EDGELABEL), EMPTY_LABEL);
						}

						lockUpdate = false;
						child = dependentSpine;
						if (i >= n)
						{
							break;
						}
						empty_label = 0;
						ps = i = i + 1;
						es = j = j + 1;
					}
				}

				// Recursively attach the dependent spines to target node. 
				DependencyNode target = (DependencyNode)depEdge.Target;
				for (int i = 0; i < target.LeftDependentCount; i++)
				{
					updatePhraseStructureGraph(graph, target.getLeftDependent(i).HeadEdge, attachHeadSpineToRoot);
				}
				for (int i = target.RightDependentCount - 1; i >= 0 ; i--)
				{
					updatePhraseStructureGraph(graph, target.getRightDependent(i).HeadEdge, attachHeadSpineToRoot);
				}
			}
			else
			{
				// If dependent spine already exist, then set dependentSpine to the highest nonterminal
				// of the dependent spine.
				while (dependentSpine.Parent != null && !dependentSpine.Parent.Root)
				{
					dependentSpine = dependentSpine.Parent;
				}
			}


			PhraseStructureNode headSpine = null;
			if (((PhraseStructureNode)depEdge.Source).Parent != null)
			{
				// If head spine exist, then attach dependent spine to the head spine at the attachment level a.
				int a = 0;
				headSpine = ((PhraseStructureNode)depEdge.Source).Parent;
				if (depEdge.hasLabel(graph.SymbolTables.getSymbolTable(ATTACH)))
				{
					try
					{
					a = int.Parse((depEdge.getLabelSymbol(graph.SymbolTables.getSymbolTable(ATTACH))));
					}
					catch (FormatException e)
					{
						throw new MaltChainedException(e.Message);
					}
				}
				for (int i = 0; i < a && headSpine != null; i++)
				{
					headSpine = headSpine.Parent;
				}

				if ((headSpine == null || headSpine == dependentSpine) && attachHeadSpineToRoot)
				{
					headSpine = graph.PhraseStructureRoot;
				}
				if (headSpine != null)
				{
					lockUpdate = true;
					Edge.Edge e = graph.addPhraseStructureEdge(headSpine, dependentSpine);
					if (depEdge.hasLabel(graph.SymbolTables.getSymbolTable(DEPREL)) && !depEdge.getLabelSymbol(graph.SymbolTables.getSymbolTable(DEPREL)).Equals(EMPTY_LABEL) & e != null)
					{
						e.addLabel(graph.SymbolTables.addSymbolTable(EDGELABEL), depEdge.getLabelSymbol(graph.SymbolTables.getSymbolTable(DEPREL)));
					}
					lockUpdate = false;
				}
			}
			else if (depEdge.Source.Root && !depEdge.Labeled)
			{
					headSpine = graph.PhraseStructureRoot;
					lockUpdate = true;
					Edge.Edge e = graph.addPhraseStructureEdge(headSpine, dependentSpine);
					if (depEdge.hasLabel(graph.SymbolTables.getSymbolTable(DEPREL)) && !depEdge.getLabelSymbol(graph.SymbolTables.getSymbolTable(DEPREL)).Equals(EMPTY_LABEL) & e != null)
					{
						e.addLabel(graph.SymbolTables.addSymbolTable(EDGELABEL), depEdge.getLabelSymbol(graph.SymbolTables.getSymbolTable(DEPREL)));
					}
					else
					{
						e.addLabel(graph.SymbolTables.addSymbolTable(EDGELABEL), graph.getDefaultRootEdgeLabelSymbol(graph.SymbolTables.getSymbolTable(DEPREL)));
					}
					lockUpdate = false;
					// Recursively attach the dependent spines to target node. 
					DependencyNode target = (DependencyNode)depEdge.Target;
					for (int i = 0; i < target.LeftDependentCount; i++)
					{
						updatePhraseStructureGraph(graph, target.getLeftDependent(i).HeadEdge, attachHeadSpineToRoot);
					}
					for (int i = target.RightDependentCount - 1; i >= 0 ; i--)
					{
						updatePhraseStructureGraph(graph, target.getRightDependent(i).HeadEdge, attachHeadSpineToRoot);
					}
			}
		}

		public virtual HeadRules.HeadRules getHeadRules()
		{
			return headRules;
		}

		public virtual void setHeadRules(HeadRules.HeadRules headRules)
		{
			this.headRules = headRules;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setHeadRules(String headRulesURL) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setHeadRules(string headRulesURL)
		{
			if (!ReferenceEquals(headRulesURL, null) && headRulesURL.Length > 0 && !headRulesURL.Equals("*"))
			{
				headRules = new HeadRules.HeadRules(SystemLogger.logger(), phraseStructuretDataFormatInstance, symbolTableHandler);
				headRules.parseHeadRules(headRulesURL);
			}
		}
	}

}