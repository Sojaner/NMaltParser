using System;
using System.Collections.Generic;

namespace org.maltparser.transform.pseudo
{
    using  core.io.dataformat;
    using  core.symbol;
    using  core.syntaxgraph;
	using  core.syntaxgraph.node;

	/// <summary>
	/// This class contains methods for projectivizing and deprojectivizing
	/// 
	/// @author Jens Nilsson
	/// </summary>
	public class PseudoProjectivity
	{
		internal static int id = 0;

		private enum PseudoProjectiveEncoding
		{
			NONE,
			BASELINE,
			HEAD,
			PATH,
			HEADPATH,
			TRACE
		}

		private enum CoveredRootAttachment
		{
			NONE,
			IGNORE,
			LEFT,
			RIGHT,
			HEAD
		}

		private enum LiftingOrder
		{
			SHORTEST,
			DEEPEST
		}

		private PseudoProjectiveEncoding markingStrategy;
		private CoveredRootAttachment rootAttachment;
		private LiftingOrder liftingOrder;
		private Logger configLogger;

		private SymbolTable deprelSymbolTable;
		private SymbolTable pppathSymbolTable;
		private SymbolTable ppliftedSymbolTable;
		private SymbolTable ppcoveredRootSymbolTable;

		private ColumnDescription deprelColumn;
		private ColumnDescription pppathColumn;
		private ColumnDescription ppliftedColumn;
		private ColumnDescription ppcoveredRootColumn;

		private List<bool> nodeLifted;
		private List<List<DependencyNode>> nodeTrace;
		private List<DependencyNode> headDeprel;
		private List<bool> nodePath;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private List<bool> isCoveredRoot_Renamed;
		private List<int> nodeRelationLength;
		private List<string> synacticHeadDeprel;


		public PseudoProjectivity()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(String markingStrategyString, String coveredRoot, String liftingOrder, org.apache.log4j.Logger configLogger, org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initialize(string markingStrategyString, string coveredRoot, string liftingOrder, Logger configLogger, DataFormatInstance dataFormatInstance, SymbolTableHandler symbolTables)
		{
			nodeLifted = new List<bool>();
			nodeTrace = new List<List<DependencyNode>>();
			headDeprel = new List<DependencyNode>();
			nodePath = new List<bool>();
			isCoveredRoot_Renamed = new List<bool>();
			nodeRelationLength = new List<int>();
			synacticHeadDeprel = new List<string>();

			this.configLogger = configLogger;
			if (markingStrategyString.Equals("none", StringComparison.OrdinalIgnoreCase))
			{
				markingStrategy = PseudoProjectiveEncoding.NONE;
			}
			else if (markingStrategyString.Equals("baseline", StringComparison.OrdinalIgnoreCase))
			{
				markingStrategy = PseudoProjectiveEncoding.BASELINE;
			}
			else if (markingStrategyString.Equals("head", StringComparison.OrdinalIgnoreCase))
			{
				markingStrategy = PseudoProjectiveEncoding.HEAD;
			}
			else if (markingStrategyString.Equals("path", StringComparison.OrdinalIgnoreCase))
			{
				markingStrategy = PseudoProjectiveEncoding.PATH;
			}
			else if (markingStrategyString.Equals("head+path", StringComparison.OrdinalIgnoreCase))
			{
				markingStrategy = PseudoProjectiveEncoding.HEADPATH;
			}
			else if (markingStrategyString.Equals("trace", StringComparison.OrdinalIgnoreCase))
			{
				markingStrategy = PseudoProjectiveEncoding.TRACE;
			}
			deprelColumn = dataFormatInstance.getColumnDescriptionByName("DEPREL");
			deprelSymbolTable = symbolTables.getSymbolTable(deprelColumn.Name);
			if (markingStrategy == PseudoProjectiveEncoding.HEAD || markingStrategy == PseudoProjectiveEncoding.PATH || markingStrategy == PseudoProjectiveEncoding.HEADPATH)
			{
				ppliftedColumn = dataFormatInstance.addInternalColumnDescription(symbolTables, "PPLIFTED", "DEPENDENCY_EDGE_LABEL", "BOOLEAN", "", deprelColumn.NullValueStrategy);
				ppliftedSymbolTable = symbolTables.getSymbolTable(ppliftedColumn.Name);
				if (markingStrategy == PseudoProjectiveEncoding.PATH)
				{
					ppliftedSymbolTable.addSymbol("#true#");
					ppliftedSymbolTable.addSymbol("#false#");
				}
				else
				{
					ppliftedSymbolTable.addSymbol("#false#");
				}
			}

			if (markingStrategy == PseudoProjectiveEncoding.PATH || markingStrategy == PseudoProjectiveEncoding.HEADPATH)
			{
				pppathColumn = dataFormatInstance.addInternalColumnDescription(symbolTables, "PPPATH", "DEPENDENCY_EDGE_LABEL", "BOOLEAN", "", deprelColumn.NullValueStrategy);
				pppathSymbolTable = symbolTables.getSymbolTable(pppathColumn.Name);
				pppathSymbolTable.addSymbol("#true#");
				pppathSymbolTable.addSymbol("#false#");
			}

			if (coveredRoot.Equals("none", StringComparison.OrdinalIgnoreCase))
			{
				rootAttachment = CoveredRootAttachment.NONE;
			}
			else if (coveredRoot.Equals("ignore", StringComparison.OrdinalIgnoreCase))
			{
				rootAttachment = CoveredRootAttachment.IGNORE;
			}
			else if (coveredRoot.Equals("left", StringComparison.OrdinalIgnoreCase))
			{
				rootAttachment = CoveredRootAttachment.LEFT;
			}
			else if (coveredRoot.Equals("right", StringComparison.OrdinalIgnoreCase))
			{
				rootAttachment = CoveredRootAttachment.RIGHT;
			}
			else if (coveredRoot.Equals("head", StringComparison.OrdinalIgnoreCase))
			{
				rootAttachment = CoveredRootAttachment.HEAD;
			}

			if (rootAttachment != CoveredRootAttachment.NONE)
			{
				ppcoveredRootColumn = dataFormatInstance.addInternalColumnDescription(symbolTables, "PPCOVERED", "DEPENDENCY_EDGE_LABEL", "BOOLEAN", "", deprelColumn.NullValueStrategy);
				ppcoveredRootSymbolTable = symbolTables.getSymbolTable(ppcoveredRootColumn.Name);
				ppcoveredRootSymbolTable.addSymbol("#true#");
				ppcoveredRootSymbolTable.addSymbol("#false#");
			}
			if (liftingOrder.Equals("shortest", StringComparison.OrdinalIgnoreCase))
			{
				this.liftingOrder = LiftingOrder.SHORTEST;
			}
			else if (liftingOrder.Equals("deepest", StringComparison.OrdinalIgnoreCase))
			{
				this.liftingOrder = LiftingOrder.DEEPEST;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initProjectivization(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		private void initProjectivization(DependencyStructure pdg)
		{
			nodeLifted.Clear();
			nodeTrace.Clear();
			headDeprel.Clear();
			nodePath.Clear();
			isCoveredRoot_Renamed.Clear();
			nodeRelationLength.Clear();

			foreach (int index in pdg.DependencyIndices)
			{
				nodeLifted.Add(false);
				nodeTrace.Add(new List<DependencyNode>());
				headDeprel.Add(null);
				nodePath.Add(false);
				isCoveredRoot_Renamed.Add(false);
				if (ppliftedSymbolTable != null && index != 0)
				{
					pdg.getDependencyNode(index).HeadEdge.LabelSet.put(ppliftedSymbolTable, ppliftedSymbolTable.getSymbolStringToCode("#false#"));
				}
				if (pppathSymbolTable != null && index != 0)
				{
					pdg.getDependencyNode(index).HeadEdge.LabelSet.put(pppathSymbolTable, pppathSymbolTable.getSymbolStringToCode("#false#"));
				}
				if (ppcoveredRootSymbolTable != null && index != 0)
				{
					pdg.getDependencyNode(index).HeadEdge.LabelSet.put(ppcoveredRootSymbolTable, ppcoveredRootSymbolTable.getSymbolStringToCode("#false#"));
				}
			}
			computeRelationLength(pdg);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void projectivize(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		public virtual void projectivize(DependencyStructure pdg)
		{
			id++;
			if (!pdg.Tree)
			{
				configLogger.info("\n[Warning: Sentence '" + id + "' cannot projectivize, because the dependency graph is not a tree]\n");
				return;
			}
			DependencyNode deepestNonProjectiveNode;
			initProjectivization(pdg);
			if (rootAttachment == CoveredRootAttachment.IGNORE)
			{
				if (markingStrategy != PseudoProjectiveEncoding.NONE)
				{
					while (!pdg.Projective)
					{
						if (liftingOrder == LiftingOrder.DEEPEST)
						{
							deepestNonProjectiveNode = getDeepestNonProjectiveNode(pdg);
						}
						else
						{
							deepestNonProjectiveNode = getShortestNonProjectiveNode(pdg);
						}
						if (!attachCoveredRoots(pdg, deepestNonProjectiveNode))
						{
							nodeLifted[deepestNonProjectiveNode.Index] = true;
							setHeadDeprel(deepestNonProjectiveNode, deepestNonProjectiveNode.Head);
							Path = deepestNonProjectiveNode.Head;
							pdg.moveDependencyEdge(pdg.getDependencyNode(deepestNonProjectiveNode.Head.Head.Index).Index, deepestNonProjectiveNode.Index);
						}
					}
					deattachCoveredRootsForProjectivization(pdg);
				}
			}
			else
			{
				if (rootAttachment != CoveredRootAttachment.NONE)
				{
					foreach (int index in pdg.TokenIndices)
					{
						attachCoveredRoots(pdg, pdg.getTokenNode(index));
					}
				}
				if (markingStrategy != PseudoProjectiveEncoding.NONE)
				{
					while (!pdg.Projective)
					{
						if (liftingOrder == LiftingOrder.DEEPEST)
						{
							deepestNonProjectiveNode = getDeepestNonProjectiveNode(pdg);
						}
						else
						{
							deepestNonProjectiveNode = getShortestNonProjectiveNode(pdg);
						}
						nodeLifted[deepestNonProjectiveNode.Index] = true;
						setHeadDeprel(deepestNonProjectiveNode, deepestNonProjectiveNode.Head);
						Path = deepestNonProjectiveNode.Head;
						pdg.moveDependencyEdge(pdg.getDependencyNode(deepestNonProjectiveNode.Head.Head.Index).Index, deepestNonProjectiveNode.Index);
					}
				}
			}
			// collectTraceStatistics(pdg);
			assignPseudoProjectiveDeprels(pdg);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void mergeArclabels(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		public virtual void mergeArclabels(DependencyStructure pdg)
		{
			assignPseudoProjectiveDeprelsForMerge(pdg);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void splitArclabels(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		public virtual void splitArclabels(DependencyStructure pdg)
		{
			int pathLabelIndex = -1, movedLabelIndex = -1, coveredArcLabelIndex;
			string label;
			initDeprojeciviztion(pdg);
			foreach (int index in pdg.TokenIndices)
			{
				if (pdg.getTokenNode(index).HeadEdge.hasLabel(deprelSymbolTable))
				{
					label = deprelSymbolTable.getSymbolCodeToString(pdg.getTokenNode(index).HeadEdge.getLabelCode(deprelSymbolTable));
					if (!ReferenceEquals(label, null) && (pathLabelIndex = label.IndexOf("%", StringComparison.Ordinal)) != -1)
					{
						label = label.Substring(0, pathLabelIndex);
						setLabel(pdg.getTokenNode(index), label);
						pdg.getTokenNode(index).HeadEdge.addLabel(pppathSymbolTable, pppathSymbolTable.getSymbolStringToCode("#true#"));
					}
					if (!ReferenceEquals(label, null) && (movedLabelIndex = label.IndexOf("|", StringComparison.Ordinal)) != -1 && label.IndexOf("|null", StringComparison.Ordinal) == -1)
					{
						if (movedLabelIndex + 1 < label.Length)
						{
							pdg.getTokenNode(index).HeadEdge.addLabel(ppliftedSymbolTable, ppliftedSymbolTable.getSymbolStringToCode(label.Substring(movedLabelIndex + 1)));
						}
						else
						{
							pdg.getTokenNode(index).HeadEdge.addLabel(ppliftedSymbolTable, ppliftedSymbolTable.getSymbolStringToCode("#true#"));
						}
						label = label.Substring(0, movedLabelIndex);
						setLabel(pdg.getTokenNode(index), label);
					}
				}
			}
			foreach (int index in pdg.TokenIndices)
			{
				if (pdg.getTokenNode(index).HeadEdge.hasLabel(deprelSymbolTable))
				{
					label = deprelSymbolTable.getSymbolCodeToString(pdg.getTokenNode(index).HeadEdge.getLabelCode(deprelSymbolTable));
					if ((coveredArcLabelIndex = label.IndexOf("|null", StringComparison.Ordinal)) != -1)
					{
						label = label.Substring(0, coveredArcLabelIndex);
						setLabel(pdg.getTokenNode(index), label);
						pdg.getTokenNode(index).HeadEdge.addLabel(ppcoveredRootSymbolTable, ppcoveredRootSymbolTable.getSymbolStringToCode("#true#"));
					}
				}
			}
		}

		private void setHeadDeprel(DependencyNode node, DependencyNode parent)
		{
			if (headDeprel[node.Index] == null)
			{
				headDeprel[node.Index] = parent;
			}
			nodeTrace[node.Index] = headDeprel;
		}

		private DependencyNode Path
		{
			set
			{
				nodePath[value.Index] = true;
			}
		}

		private bool isCoveredRoot(DependencyNode node)
		{
			return isCoveredRoot_Renamed[node.Index];
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void deattachCoveredRootsForProjectivization(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		private void deattachCoveredRootsForProjectivization(DependencyStructure pdg)
		{
			foreach (int index in pdg.TokenIndices)
			{
				if (isCoveredRoot(pdg.getTokenNode(index)))
				{
					pdg.moveDependencyEdge(pdg.DependencyRoot.Index, pdg.getTokenNode(index).Index);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean attachCoveredRoots(org.maltparser.core.syntaxgraph.DependencyStructure pdg, org.maltparser.core.syntaxgraph.node.DependencyNode deepest) throws org.maltparser.core.exception.MaltChainedException
		private bool attachCoveredRoots(DependencyStructure pdg, DependencyNode deepest)
		{
			int i;
			bool foundCoveredRoot = false;
			DependencyNode coveredRootHead;
			for (i = Math.Min(deepest.Index, deepest.Head.Index) + 1; i < Math.Max(deepest.Index, deepest.Head.Index); i++)
			{
				int leftMostIndex = pdg.getDependencyNode(i).LeftmostProperDescendantIndex;
				if (leftMostIndex == -1)
				{
					leftMostIndex = i;
				}
				int rightMostIndex = pdg.getDependencyNode(i).RightmostProperDescendantIndex;
				if (rightMostIndex == -1)
				{
					rightMostIndex = i;
				}
				if (!nodeLifted[i] && pdg.getDependencyNode(i).Head.Root && !deepest.Head.Root && Math.Min(deepest.Index, deepest.Head.Index) < leftMostIndex && rightMostIndex < Math.Max(deepest.Index, deepest.Head.Index))
				{
					if (rootAttachment == CoveredRootAttachment.LEFT)
					{
						if (deepest.Head.Index < deepest.Index)
						{
							coveredRootHead = deepest.Head;
						}
						else
						{
							coveredRootHead = deepest;
						}
					}
					else if (rootAttachment == CoveredRootAttachment.RIGHT)
					{
						if (deepest.Index < deepest.Head.Index)
						{
							coveredRootHead = deepest.Head;
						}
						else
						{
							coveredRootHead = deepest;
						}
					}
					else
					{
						coveredRootHead = deepest.Head;
					}
					pdg.moveDependencyEdge(coveredRootHead.Index, pdg.getDependencyNode(i).Index);
					CoveredRoot = pdg.getDependencyNode(i);
					foundCoveredRoot = true;
				}
			}
			return foundCoveredRoot;
		}

		private DependencyNode CoveredRoot
		{
			set
			{
				isCoveredRoot_Renamed[value.Index] = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.core.syntaxgraph.node.DependencyNode getDeepestNonProjectiveNode(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		private DependencyNode getDeepestNonProjectiveNode(DependencyStructure pdg)
		{
			DependencyNode deepestNonProjectiveNode = null;
			foreach (int index in pdg.DependencyIndices)
			{
				if (!pdg.getDependencyNode(index).Projective && (deepestNonProjectiveNode == null || pdg.getDependencyNode(index).DependencyNodeDepth > pdg.getDependencyNode(deepestNonProjectiveNode.Index).DependencyNodeDepth))
				{
					deepestNonProjectiveNode = pdg.getDependencyNode(index);
				}
			}

			return deepestNonProjectiveNode;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.core.syntaxgraph.node.DependencyNode getShortestNonProjectiveNode(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		private DependencyNode getShortestNonProjectiveNode(DependencyStructure pdg)
		{
			DependencyNode shortestNonProjectiveNode = null;
			foreach (int index in pdg.DependencyIndices)
			{
				if (!pdg.getDependencyNode(index).Projective && (shortestNonProjectiveNode == null || nodeRelationLength[index] < nodeRelationLength[shortestNonProjectiveNode.Index]))
				{
	//					|| (nodeRelationLength.get(index) == nodeRelationLength.get(shortestNonProjectiveNode.getIndex())))) {
					shortestNonProjectiveNode = pdg.getDependencyNode(index);
				}
			}
			return shortestNonProjectiveNode;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void computeRelationLength(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		private void computeRelationLength(DependencyStructure pdg)
		{
			nodeRelationLength.Add(0);
			foreach (int index in pdg.TokenIndices)
			{
				nodeRelationLength.Add(Math.Abs(pdg.getDependencyNode(index).Index - pdg.getDependencyNode(index).Head.Index));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void assignPseudoProjectiveDeprels(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		private void assignPseudoProjectiveDeprels(DependencyStructure pdg)
		{
			int newLabelCode;
			foreach (int index in pdg.TokenIndices)
			{
				if (!isCoveredRoot(pdg.getDependencyNode(index)))
				{
					if (markingStrategy == PseudoProjectiveEncoding.HEAD || markingStrategy == PseudoProjectiveEncoding.PATH || markingStrategy == PseudoProjectiveEncoding.HEADPATH)
					{
						if (markingStrategy == PseudoProjectiveEncoding.PATH)
						{
							if (nodeLifted[index])
							{
								newLabelCode = ppliftedSymbolTable.getSymbolStringToCode("#true#");
							}
							else
							{
								newLabelCode = ppliftedSymbolTable.getSymbolStringToCode("#false#");
							}
							pdg.getDependencyNode(index).HeadEdge.addLabel(ppliftedSymbolTable, newLabelCode);
						}
						else
						{
							if (nodeLifted[index])
							{
								newLabelCode = ppliftedSymbolTable.addSymbol(deprelSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(headDeprel[index].Index).HeadEdge.getLabelCode(deprelSymbolTable)));
							}
							else
							{
								newLabelCode = ppliftedSymbolTable.getSymbolStringToCode("#false#");
							}
							pdg.getDependencyNode(index).HeadEdge.addLabel(ppliftedSymbolTable, newLabelCode);
						}
					}

					if (markingStrategy == PseudoProjectiveEncoding.PATH || markingStrategy == PseudoProjectiveEncoding.HEADPATH)
					{
						if (nodePath[index])
						{
							newLabelCode = pppathSymbolTable.getSymbolStringToCode("#true#");
						}
						else
						{
							newLabelCode = pppathSymbolTable.getSymbolStringToCode("#false#");
						}
						pdg.getDependencyNode(index).HeadEdge.addLabel(pppathSymbolTable, newLabelCode);
					}

				}
				else if (!(rootAttachment == CoveredRootAttachment.NONE || rootAttachment == CoveredRootAttachment.IGNORE))
				{
					pdg.getDependencyNode(index).HeadEdge.addLabel(ppcoveredRootSymbolTable, ppcoveredRootSymbolTable.getSymbolStringToCode("#true#"));
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void setLabel(org.maltparser.core.syntaxgraph.node.DependencyNode node, String label) throws org.maltparser.core.exception.MaltChainedException
		private void setLabel(DependencyNode node, string label)
		{
			// node.getLabelCode().clear();
			node.HeadEdge.LabelSet.put(deprelSymbolTable, deprelSymbolTable.addSymbol(label));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void assignPseudoProjectiveDeprelsForMerge(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		private void assignPseudoProjectiveDeprelsForMerge(DependencyStructure pdg)
		{
			List<string> originalDeprel = new List<string>();
			string newLabel;
			originalDeprel.Add(null);
			foreach (int index in pdg.TokenIndices)
			{
				originalDeprel.Add(deprelSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(deprelSymbolTable)));
			}
			foreach (int index in pdg.TokenIndices)
			{
				newLabel = null;
				if (!isCoveredRoot(pdg.getDependencyNode(index)))
				{
					if (markingStrategy == PseudoProjectiveEncoding.HEAD)
					{
						if (nodeLifted[index])
						{
							newLabel = deprelSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(deprelSymbolTable)) + "|"
									+ originalDeprel[headDeprel[index].Index];
							// } else {
							// newLabel = deprelSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).getHeadEdge().getLabelCode(deprelSymbolTable));
						}
					}
					else if (markingStrategy == PseudoProjectiveEncoding.PATH)
					{
						if (nodeLifted[index] && nodePath[index])
						{
							newLabel = deprelSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(deprelSymbolTable)) + "|%";
						}
						else if (nodeLifted[index] && !nodePath[index])
						{
							newLabel = deprelSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(deprelSymbolTable)) + "|";
						}
						else if (!nodeLifted[index] && nodePath[index])
						{
							newLabel = deprelSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(deprelSymbolTable)) + "%";
						}
					}
					else if (markingStrategy == PseudoProjectiveEncoding.HEADPATH)
					{
						if (nodeLifted[index] && nodePath[index])
						{
							newLabel = deprelSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(deprelSymbolTable)) + "|"
									+ originalDeprel[headDeprel[index].Index] + "%";
						}
						else if (nodeLifted[index] && !nodePath[index])
						{
							newLabel = deprelSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(deprelSymbolTable)) + "|"
									+ originalDeprel[headDeprel[index].Index];
						}
						else if (!nodeLifted[index] && nodePath[index])
						{
							newLabel = originalDeprel[pdg.getDependencyNode(index).Index] + "%";
						}
					}
					else if (markingStrategy == PseudoProjectiveEncoding.TRACE)
					{
						if (nodeLifted[index])
						{
							newLabel = deprelSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(deprelSymbolTable)) + "|";
						}
					}
				}
				else if (!(rootAttachment == CoveredRootAttachment.NONE || rootAttachment == CoveredRootAttachment.IGNORE))
				{
					newLabel = deprelSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(deprelSymbolTable)) + "|null";
				}
				if (!ReferenceEquals(newLabel, null))
				{
					setLabel(pdg.getDependencyNode(index), newLabel);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void deprojectivize(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		public virtual void deprojectivize(DependencyStructure pdg)
		{
			initDeprojeciviztion(pdg);

			foreach (int index in pdg.TokenIndices)
			{
				if (pdg.getDependencyNode(index).HeadEdge.hasLabel(deprelSymbolTable))
				{
					if (pdg.getDependencyNode(index).HeadEdge.hasLabel(pppathSymbolTable) && pppathSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(pppathSymbolTable)).Equals("#true#"))
					{
						Path = pdg.getDependencyNode(index);
					}
					if (pdg.getDependencyNode(index).HeadEdge.hasLabel(ppliftedSymbolTable) && !ppliftedSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(ppliftedSymbolTable)).Equals("#false#"))
					{
						nodeLifted[index] = true;
						if (!ppliftedSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(ppliftedSymbolTable)).Equals("#true#"))
						{
							synacticHeadDeprel[index] = ppliftedSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(ppliftedSymbolTable));
						}
					}
				}
			}
			deattachCoveredRootsForDeprojectivization(pdg);
			if (markingStrategy == PseudoProjectiveEncoding.HEAD && needsDeprojectivizeWithHead(pdg))
			{
				deprojectivizeWithHead(pdg, pdg.DependencyRoot);
			}
			else if (markingStrategy == PseudoProjectiveEncoding.PATH)
			{
				deprojectivizeWithPath(pdg, pdg.DependencyRoot);
			}
			else if (markingStrategy == PseudoProjectiveEncoding.HEADPATH)
			{
				deprojectivizeWithHeadAndPath(pdg, pdg.DependencyRoot);
			}
		}

		private void initDeprojeciviztion(DependencyStructure pdg)
		{
			nodeLifted.Clear();
			nodePath.Clear();
			synacticHeadDeprel.Clear();
			foreach (int index in pdg.DependencyIndices)
			{
				nodeLifted.Add(false);
				nodePath.Add(false);
				synacticHeadDeprel.Add(null);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void deattachCoveredRootsForDeprojectivization(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		private void deattachCoveredRootsForDeprojectivization(DependencyStructure pdg)
		{
			foreach (int index in pdg.TokenIndices)
			{
				if (pdg.getDependencyNode(index).HeadEdge.hasLabel(deprelSymbolTable))
				{
					if (pdg.getDependencyNode(index).HeadEdge.hasLabel(ppcoveredRootSymbolTable) && ppcoveredRootSymbolTable.getSymbolCodeToString(pdg.getDependencyNode(index).HeadEdge.getLabelCode(ppcoveredRootSymbolTable)).Equals("#true#"))
					{
						pdg.moveDependencyEdge(pdg.DependencyRoot.Index, pdg.getDependencyNode(index).Index);
					}
				}
			}
		}

		// Check whether there is at least one node in the specified dependency structure that can be lifted.
		// If this is not the case, there is no need to call deprojectivizeWithHead.

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean needsDeprojectivizeWithHead(org.maltparser.core.syntaxgraph.DependencyStructure pdg) throws org.maltparser.core.exception.MaltChainedException
		private bool needsDeprojectivizeWithHead(DependencyStructure pdg)
		{
			foreach (int index in pdg.DependencyIndices)
			{
				if (nodeLifted[index])
				{
					DependencyNode node = pdg.getDependencyNode(index);
					if (breadthFirstSearchSortedByDistanceForHead(pdg, node.Head, node, synacticHeadDeprel[index]) != null)
					{
						return true;
					}
				}
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean deprojectivizeWithHead(org.maltparser.core.syntaxgraph.DependencyStructure pdg, org.maltparser.core.syntaxgraph.node.DependencyNode node) throws org.maltparser.core.exception.MaltChainedException
		private bool deprojectivizeWithHead(DependencyStructure pdg, DependencyNode node)
		{
			bool success = true, childSuccess = false;
			int i, childAttempts = 2;
			DependencyNode child, possibleSyntacticHead;
			string syntacticHeadDeprel;
			if (nodeLifted[node.Index])
			{
				syntacticHeadDeprel = synacticHeadDeprel[node.Index];
				possibleSyntacticHead = breadthFirstSearchSortedByDistanceForHead(pdg, node.Head, node, syntacticHeadDeprel);
				if (possibleSyntacticHead != null)
				{
					pdg.moveDependencyEdge(possibleSyntacticHead.Index, node.Index);
					nodeLifted[node.Index] = false;
				}
				else
				{
					success = false;
				}
			}
			while (!childSuccess && childAttempts > 0)
			{
				childSuccess = true;
				List<DependencyNode> children = new List<DependencyNode>();
				i = 0;
				while ((child = node.getLeftDependent(i)) != null)
				{
					children.Add(child);
					i++;
				}
				i = 0;
				while ((child = node.getRightDependent(i)) != null)
				{
					children.Add(child);
					i++;
				}
				for (i = 0; i < children.Count; i++)
				{
					child = children[i];
					if (!deprojectivizeWithHead(pdg, child))
					{
						childSuccess = false;
					}
				}
				childAttempts--;
			}
			return childSuccess && success;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.core.syntaxgraph.node.DependencyNode breadthFirstSearchSortedByDistanceForHead(org.maltparser.core.syntaxgraph.DependencyStructure dg, org.maltparser.core.syntaxgraph.node.DependencyNode start, org.maltparser.core.syntaxgraph.node.DependencyNode avoid, String syntacticHeadDeprel) throws org.maltparser.core.exception.MaltChainedException
		private DependencyNode breadthFirstSearchSortedByDistanceForHead(DependencyStructure dg, DependencyNode start, DependencyNode avoid, string syntacticHeadDeprel)
		{
			DependencyNode dependent;
			string dependentDeprel;
			List<DependencyNode> nodes = new List<DependencyNode>();
			nodes.AddRange(findAllDependentsVectorSortedByDistanceToPProjNode(dg, start, avoid, false));
			while (nodes.Count > 0)
			{
				dependent = nodes.RemoveAt(0);
				if (dependent.HeadEdge.hasLabel(deprelSymbolTable))
				{
					dependentDeprel = deprelSymbolTable.getSymbolCodeToString(dependent.HeadEdge.getLabelCode(deprelSymbolTable));
					if (dependentDeprel.Equals(syntacticHeadDeprel))
					{
						return dependent;
					}
				}
				nodes.AddRange(findAllDependentsVectorSortedByDistanceToPProjNode(dg, dependent, avoid, false));
			}
			return null;
		}


		private List<DependencyNode> findAllDependentsVectorSortedByDistanceToPProjNode(DependencyStructure dg, DependencyNode governor, DependencyNode avoid, bool percentOnly)
		{
			List<DependencyNode> output = new List<DependencyNode>();
			SortedSet<DependencyNode> dependents = new SortedSet<DependencyNode>();
			dependents.addAll(governor.LeftDependents);
			dependents.addAll(governor.RightDependents);


			DependencyNode[] deps = new DependencyNode[dependents.Count];
			int[] distances = new int[dependents.Count];
			int i = 0;
			foreach (DependencyNode dep in dependents)
			{
				distances[i] = Math.Abs(dep.Index - avoid.Index);
				deps[i] = dep;
				i++;
			}
			if (distances.Length > 1)
			{
				int smallest;
				int n = distances.Length;
				int tmpDist;
				DependencyNode tmpDep;
				for (i = 0; i < n; i++)
				{
					smallest = i;
					for (int j = i; j < n; j++)
					{
						if (distances[j] < distances[smallest])
						{
							smallest = j;
						}
					}
					if (smallest != i)
					{
						tmpDist = distances[smallest];
						distances[smallest] = distances[i];
						distances[i] = tmpDist;
						tmpDep = deps[smallest];
						deps[smallest] = deps[i];
						deps[i] = tmpDep;
					}
				}
			}
			for (i = 0; i < distances.Length;i++)
			{
				if (deps[i] != avoid && (!percentOnly || (percentOnly && nodePath[deps[i].Index])))
				{
					output.Add(deps[i]);
				}
			}
			return output;
		}

		private List<DependencyNode> findAllDependentsVectorSortedByDistanceToPProjNode2(DependencyStructure dg, DependencyNode governor, DependencyNode avoid, bool percentOnly)
		{
			int i, j;
			List<DependencyNode> dependents = new List<DependencyNode>();
			DependencyNode leftChild, rightChild;

			i = governor.LeftDependentCount - 1;
			j = 0;
			leftChild = governor.getLeftDependent(i--);
			rightChild = governor.getRightDependent(j++);

			while (leftChild != null && rightChild != null)
			{
				if (leftChild == avoid)
				{
					leftChild = governor.getLeftDependent(i--);
				}
				else if (rightChild == avoid)
				{
					rightChild = governor.getRightDependent(j++);
				}
				else if (Math.Abs(leftChild.Index - avoid.Index) < Math.Abs(rightChild.Index - avoid.Index))
				{
					if (!percentOnly || (percentOnly && nodePath[leftChild.Index]))
					{
						dependents.Add(leftChild);
					}
					leftChild = governor.getLeftDependent(i--);
				}
				else
				{
					if (!percentOnly || (percentOnly && nodePath[rightChild.Index]))
					{
						dependents.Add(rightChild);
					}
					rightChild = governor.getRightDependent(j++);
				}
			}
			while (leftChild != null)
			{
				if (leftChild != avoid && (!percentOnly || (percentOnly && nodePath[leftChild.Index])))
				{
					dependents.Add(leftChild);
				}
				leftChild = governor.getLeftDependent(i--);
			}
			while (rightChild != null)
			{
				if (rightChild != avoid && (!percentOnly || (percentOnly && nodePath[rightChild.Index])))
				{
					dependents.Add(rightChild);
				}
				rightChild = governor.getRightDependent(j++);
			}
			return dependents;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean deprojectivizeWithPath(org.maltparser.core.syntaxgraph.DependencyStructure pdg, org.maltparser.core.syntaxgraph.node.DependencyNode node) throws org.maltparser.core.exception.MaltChainedException
		private bool deprojectivizeWithPath(DependencyStructure pdg, DependencyNode node)
		{
			bool success = true, childSuccess = false;
			int i, childAttempts = 2;
			DependencyNode child, possibleSyntacticHead;
			if (node.hasHead() && node.HeadEdge.Labeled && nodeLifted[node.Index] && nodePath[node.Index])
			{
				possibleSyntacticHead = breadthFirstSearchSortedByDistanceForPath(pdg, node.Head, node);
				if (possibleSyntacticHead != null)
				{
					pdg.moveDependencyEdge(possibleSyntacticHead.Index, node.Index);
					nodeLifted[node.Index] = false;
				}
				else
				{
					success = false;
				}
			}
			if (node.hasHead() && node.HeadEdge.Labeled && nodeLifted[node.Index])
			{
				possibleSyntacticHead = breadthFirstSearchSortedByDistanceForPath(pdg, node.Head, node);
				if (possibleSyntacticHead != null)
				{
					pdg.moveDependencyEdge(possibleSyntacticHead.Index, node.Index);
					nodeLifted[node.Index] = false;
				}
				else
				{
					success = false;
				}
			}
			while (!childSuccess && childAttempts > 0)
			{
				childSuccess = true;
				List<DependencyNode> children = new List<DependencyNode>();
				i = 0;
				while ((child = node.getLeftDependent(i)) != null)
				{
					children.Add(child);
					i++;
				}
				i = 0;
				while ((child = node.getRightDependent(i)) != null)
				{
					children.Add(child);
					i++;
				}
				for (i = 0; i < children.Count; i++)
				{
					child = children[i];
					if (!deprojectivizeWithPath(pdg, child))
					{
						childSuccess = false;
					}
				}
				childAttempts--;
			}
			return childSuccess && success;
		}

		private DependencyNode breadthFirstSearchSortedByDistanceForPath(DependencyStructure dg, DependencyNode start, DependencyNode avoid)
		{
			DependencyNode dependent;
			List<DependencyNode> nodes = new List<DependencyNode>(), newNodes;
			nodes.AddRange(findAllDependentsVectorSortedByDistanceToPProjNode(dg, start, avoid, true));
			while (nodes.Count > 0)
			{
				dependent = nodes.RemoveAt(0);
				if (((newNodes = findAllDependentsVectorSortedByDistanceToPProjNode(dg, dependent, avoid, true)).Count) == 0)
				{
					return dependent;
				}
				nodes.AddRange(newNodes);
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean deprojectivizeWithHeadAndPath(org.maltparser.core.syntaxgraph.DependencyStructure pdg, org.maltparser.core.syntaxgraph.node.DependencyNode node) throws org.maltparser.core.exception.MaltChainedException
		private bool deprojectivizeWithHeadAndPath(DependencyStructure pdg, DependencyNode node)
		{
			bool success = true, childSuccess = false;
			int i, childAttempts = 2;
			DependencyNode child, possibleSyntacticHead;
			if (node.hasHead() && node.HeadEdge.Labeled && nodeLifted[node.Index] && nodePath[node.Index])
			{
				possibleSyntacticHead = breadthFirstSearchSortedByDistanceForHeadAndPath(pdg, node.Head, node, synacticHeadDeprel[node.Index]);
				if (possibleSyntacticHead != null)
				{
					pdg.moveDependencyEdge(possibleSyntacticHead.Index, node.Index);
					nodeLifted[node.Index] = false;
				}
				else
				{
					success = false;
				}
			}
			if (node.hasHead() && node.HeadEdge.Labeled && nodeLifted[node.Index])
			{
				possibleSyntacticHead = breadthFirstSearchSortedByDistanceForHeadAndPath(pdg, node.Head, node, synacticHeadDeprel[node.Index]);
				if (possibleSyntacticHead != null)
				{
					pdg.moveDependencyEdge(possibleSyntacticHead.Index, node.Index);
					nodeLifted[node.Index] = false;
				}
				else
				{
					success = false;
				}
			}
			while (!childSuccess && childAttempts > 0)
			{
				childSuccess = true;
				List<DependencyNode> children = new List<DependencyNode>();
				i = 0;
				while ((child = node.getLeftDependent(i)) != null)
				{
					children.Add(child);
					i++;
				}
				i = 0;
				while ((child = node.getRightDependent(i)) != null)
				{
					children.Add(child);
					i++;
				}
				for (i = 0; i < children.Count; i++)
				{
					child = children[i];
					if (!deprojectivizeWithHeadAndPath(pdg, child))
					{
						childSuccess = false;
					}
				}
				childAttempts--;
			}
			return childSuccess && success;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.core.syntaxgraph.node.DependencyNode breadthFirstSearchSortedByDistanceForHeadAndPath(org.maltparser.core.syntaxgraph.DependencyStructure dg, org.maltparser.core.syntaxgraph.node.DependencyNode start, org.maltparser.core.syntaxgraph.node.DependencyNode avoid, String syntacticHeadDeprelCode) throws org.maltparser.core.exception.MaltChainedException
		private DependencyNode breadthFirstSearchSortedByDistanceForHeadAndPath(DependencyStructure dg, DependencyNode start, DependencyNode avoid, string syntacticHeadDeprelCode)
		{
			DependencyNode dependent;
			List<DependencyNode> nodes = new List<DependencyNode>(), newNodes = null, secondChance = new List<DependencyNode>();
			nodes.AddRange(findAllDependentsVectorSortedByDistanceToPProjNode(dg, start, avoid, true));
			while (nodes.Count > 0)
			{
				dependent = nodes.RemoveAt(0);
				if (((newNodes = findAllDependentsVectorSortedByDistanceToPProjNode(dg, dependent, avoid, true)).Count) == 0 && deprelSymbolTable.getSymbolCodeToString(dependent.HeadEdge.getLabelCode(deprelSymbolTable)).Equals(syntacticHeadDeprelCode))
				{
					return dependent;
				}
				nodes.AddRange(newNodes);
				if (deprelSymbolTable.getSymbolCodeToString(dependent.HeadEdge.getLabelCode(deprelSymbolTable)).Equals(syntacticHeadDeprelCode) && newNodes.Count != 0)
				{
					secondChance.Add(dependent);
				}
			}
			if (secondChance.Count > 0)
			{
				return secondChance[0];
			}
			return null;
		}
	}

}