using System;
using System.Collections.Generic;

namespace org.maltparser.core.lw.graph
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	using DependencyStructure = org.maltparser.core.syntaxgraph.DependencyStructure;
	using Edge = org.maltparser.core.syntaxgraph.edge.Edge;
	using DependencyNode = org.maltparser.core.syntaxgraph.node.DependencyNode;

	/// <summary>
	/// A lightweight version of pseudo projectivity and this class can only perform deprojectivizing. The class is based on 
	/// the more complex class org.maltparser.transform.pseudo.PseudoProjectivity.
	/// 
	/// 
	/// @author Johan Hall
	/// </summary>
	public sealed class LWDeprojectivizer
	{
		public const int NONE = 0;
		public const int BASELINE = 1;
		public const int HEAD = 1;
		public const int PATH = 1;
		public const int HEADPATH = 1;
		public const int TRACE = 1;

		public LWDeprojectivizer()
		{
		}

		public static int getMarkingStrategyInt(string markingStrategyString)
		{
			if (markingStrategyString.Equals("none", StringComparison.OrdinalIgnoreCase))
			{
				return LWDeprojectivizer.NONE;
			}
			else if (markingStrategyString.Equals("baseline", StringComparison.OrdinalIgnoreCase))
			{
				return LWDeprojectivizer.BASELINE;
			}
			else if (markingStrategyString.Equals("head", StringComparison.OrdinalIgnoreCase))
			{
				return LWDeprojectivizer.HEAD;
			}
			else if (markingStrategyString.Equals("path", StringComparison.OrdinalIgnoreCase))
			{
				return LWDeprojectivizer.PATH;
			}
			else if (markingStrategyString.Equals("head+path", StringComparison.OrdinalIgnoreCase))
			{
				return LWDeprojectivizer.HEADPATH;
			}
			else if (markingStrategyString.Equals("trace", StringComparison.OrdinalIgnoreCase))
			{
				return LWDeprojectivizer.TRACE;
			}
			return LWDeprojectivizer.NONE;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void deprojectivize(org.maltparser.core.syntaxgraph.DependencyStructure pdg, int markingStrategy) throws org.maltparser.core.exception.MaltChainedException
		public void deprojectivize(DependencyStructure pdg, int markingStrategy)
		{
			SymbolTable deprelSymbolTable = pdg.SymbolTables.getSymbolTable("DEPREL");
			SymbolTable ppliftedSymbolTable = pdg.SymbolTables.getSymbolTable("PPLIFTED");
			SymbolTable pppathSymbolTable = pdg.SymbolTables.getSymbolTable("PPPATH");

			bool[] nodeLifted = new bool[pdg.nDependencyNode()];
			Arrays.fill(nodeLifted, false);
			bool[] nodePath = new bool[pdg.nDependencyNode()];
			Arrays.fill(nodePath, false);
			string[] synacticHeadDeprel = new string[pdg.nDependencyNode()];
			Arrays.fill(synacticHeadDeprel, null);

			foreach (int index in pdg.TokenIndices)
			{
				Edge e = pdg.getDependencyNode(index).HeadEdge;
				if (e.hasLabel(deprelSymbolTable))
				{
					if (e.hasLabel(pppathSymbolTable) && pppathSymbolTable.getSymbolCodeToString(e.getLabelCode(pppathSymbolTable)).Equals("#true#"))
					{
						nodePath[pdg.getDependencyNode(index).Index] = true;
					}
					if (e.hasLabel(ppliftedSymbolTable) && !ppliftedSymbolTable.getSymbolCodeToString(e.getLabelCode(ppliftedSymbolTable)).Equals("#false#"))
					{
						nodeLifted[index] = true;

						if (!ppliftedSymbolTable.getSymbolCodeToString(e.getLabelCode(ppliftedSymbolTable)).Equals("#true#"))
						{
							synacticHeadDeprel[index] = ppliftedSymbolTable.getSymbolCodeToString(e.getLabelCode(ppliftedSymbolTable));
						}
					}
				}
			}
			deattachCoveredRootsForDeprojectivization(pdg, deprelSymbolTable);
			if (markingStrategy == LWDeprojectivizer.HEAD && needsDeprojectivizeWithHead(pdg, nodeLifted, nodePath, synacticHeadDeprel, deprelSymbolTable))
			{
				deprojectivizeWithHead(pdg, pdg.DependencyRoot, nodeLifted, nodePath, synacticHeadDeprel, deprelSymbolTable);
			}
			else if (markingStrategy == LWDeprojectivizer.PATH)
			{
				deprojectivizeWithPath(pdg, pdg.DependencyRoot, nodeLifted, nodePath);
			}
			else if (markingStrategy == LWDeprojectivizer.HEADPATH)
			{
				deprojectivizeWithHeadAndPath(pdg, pdg.DependencyRoot, nodeLifted, nodePath, synacticHeadDeprel, deprelSymbolTable);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void deattachCoveredRootsForDeprojectivization(org.maltparser.core.syntaxgraph.DependencyStructure pdg, org.maltparser.core.symbol.SymbolTable deprelSymbolTable) throws org.maltparser.core.exception.MaltChainedException
		private void deattachCoveredRootsForDeprojectivization(DependencyStructure pdg, SymbolTable deprelSymbolTable)
		{
			SymbolTable ppcoveredRootSymbolTable = pdg.SymbolTables.getSymbolTable("PPCOVERED");
			foreach (int index in pdg.TokenIndices)
			{
				Edge e = pdg.getDependencyNode(index).HeadEdge;
				if (e.hasLabel(deprelSymbolTable))
				{
					if (e.hasLabel(ppcoveredRootSymbolTable) && ppcoveredRootSymbolTable.getSymbolCodeToString(e.getLabelCode(ppcoveredRootSymbolTable)).Equals("#true#"))
					{
						pdg.moveDependencyEdge(pdg.DependencyRoot.Index, pdg.getDependencyNode(index).Index);
					}
				}
			}
		}

		// Check whether there is at least one node in the specified dependency structure that can be lifted.
		// If this is not the case, there is no need to call deprojectivizeWithHead.

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean needsDeprojectivizeWithHead(org.maltparser.core.syntaxgraph.DependencyStructure pdg, boolean[] nodeLifted, boolean[] nodePath, String[] synacticHeadDeprel, org.maltparser.core.symbol.SymbolTable deprelSymbolTable) throws org.maltparser.core.exception.MaltChainedException
		private bool needsDeprojectivizeWithHead(DependencyStructure pdg, bool[] nodeLifted, bool[] nodePath, string[] synacticHeadDeprel, SymbolTable deprelSymbolTable)
		{
			foreach (int index in pdg.DependencyIndices)
			{
				if (nodeLifted[index])
				{
					DependencyNode node = pdg.getDependencyNode(index);
					if (breadthFirstSearchSortedByDistanceForHead(pdg, node.Head, node, synacticHeadDeprel[index], nodePath, deprelSymbolTable) != null)
					{
						return true;
					}
				}
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean deprojectivizeWithHead(org.maltparser.core.syntaxgraph.DependencyStructure pdg, org.maltparser.core.syntaxgraph.node.DependencyNode node, boolean[] nodeLifted, boolean[] nodePath, String[] synacticHeadDeprel, org.maltparser.core.symbol.SymbolTable deprelSymbolTable) throws org.maltparser.core.exception.MaltChainedException
		private bool deprojectivizeWithHead(DependencyStructure pdg, DependencyNode node, bool[] nodeLifted, bool[] nodePath, string[] synacticHeadDeprel, SymbolTable deprelSymbolTable)
		{
			bool success = true, childSuccess = false;
			int i, childAttempts = 2;
			DependencyNode possibleSyntacticHead;
			string syntacticHeadDeprel;
			if (nodeLifted[node.Index])
			{
				syntacticHeadDeprel = synacticHeadDeprel[node.Index];
				possibleSyntacticHead = breadthFirstSearchSortedByDistanceForHead(pdg, node.Head, node, syntacticHeadDeprel, nodePath, deprelSymbolTable);
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

				IList<DependencyNode> children = node.ListOfDependents;
				for (i = 0; i < children.Count; i++)
				{
					if (!deprojectivizeWithHead(pdg, children[i], nodeLifted, nodePath, synacticHeadDeprel, deprelSymbolTable))
					{
						childSuccess = false;
					}
				}
				childAttempts--;
			}
			return childSuccess && success;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.core.syntaxgraph.node.DependencyNode breadthFirstSearchSortedByDistanceForHead(org.maltparser.core.syntaxgraph.DependencyStructure dg, org.maltparser.core.syntaxgraph.node.DependencyNode start, org.maltparser.core.syntaxgraph.node.DependencyNode avoid, String syntacticHeadDeprel, boolean[] nodePath, org.maltparser.core.symbol.SymbolTable deprelSymbolTable) throws org.maltparser.core.exception.MaltChainedException
		private DependencyNode breadthFirstSearchSortedByDistanceForHead(DependencyStructure dg, DependencyNode start, DependencyNode avoid, string syntacticHeadDeprel, bool[] nodePath, SymbolTable deprelSymbolTable)
		{
			DependencyNode dependent;
			string dependentDeprel;
			IList<DependencyNode> nodes = new List<DependencyNode>();
			((IList<DependencyNode>)nodes).AddRange(findAllDependentsVectorSortedByDistanceToPProjNode(dg, start, avoid, false, nodePath));
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
				((IList<DependencyNode>)nodes).AddRange(findAllDependentsVectorSortedByDistanceToPProjNode(dg, dependent, avoid, false, nodePath));
			}
			return null;
		}


		private IList<DependencyNode> findAllDependentsVectorSortedByDistanceToPProjNode(DependencyStructure dg, DependencyNode governor, DependencyNode avoid, bool percentOnly, bool[] nodePath)
		{
			IList<DependencyNode> output = new List<DependencyNode>();
			IList<DependencyNode> dependents = governor.ListOfDependents;
	//		SortedSet<DependencyNode> dependents = new TreeSet<DependencyNode>();
	//		dependents.addAll(governor.getLeftDependents());
	//		dependents.addAll(governor.getRightDependents());


			DependencyNode[] deps = new DependencyNode[dependents.Count];
			int[] distances = new int[dependents.Count];
			for (int i = 0; i < dependents.Count; i++)
			{
				distances[i] = Math.Abs(dependents[i].Index - avoid.Index);
				deps[i] = dependents[i];
			}
			if (distances.Length > 1)
			{
				int smallest;
				int n = distances.Length;
				int tmpDist;
				DependencyNode tmpDep;
				for (int i = 0; i < n; i++)
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
			for (int i = 0; i < distances.Length;i++)
			{
				if (deps[i] != avoid && (!percentOnly || (percentOnly && nodePath[deps[i].Index])))
				{
					output.Add(deps[i]);
				}
			}
			return output;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean deprojectivizeWithPath(org.maltparser.core.syntaxgraph.DependencyStructure pdg, org.maltparser.core.syntaxgraph.node.DependencyNode node, boolean[] nodeLifted, boolean[] nodePath) throws org.maltparser.core.exception.MaltChainedException
		private bool deprojectivizeWithPath(DependencyStructure pdg, DependencyNode node, bool[] nodeLifted, bool[] nodePath)
		{
			bool success = true, childSuccess = false;
			int i, childAttempts = 2;
			DependencyNode possibleSyntacticHead;
			if (node.hasHead() && node.HeadEdge.Labeled && nodeLifted[node.Index] && nodePath[node.Index])
			{
				possibleSyntacticHead = breadthFirstSearchSortedByDistanceForPath(pdg, node.Head, node, nodePath);
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
				possibleSyntacticHead = breadthFirstSearchSortedByDistanceForPath(pdg, node.Head, node, nodePath);
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
				IList<DependencyNode> children = node.ListOfDependents;
				for (i = 0; i < children.Count; i++)
				{
					if (!deprojectivizeWithPath(pdg, children[i], nodeLifted, nodePath))
					{
						childSuccess = false;
					}
				}
				childAttempts--;
			}
			return childSuccess && success;
		}

		private DependencyNode breadthFirstSearchSortedByDistanceForPath(DependencyStructure dg, DependencyNode start, DependencyNode avoid, bool[] nodePath)
		{
			DependencyNode dependent;
			IList<DependencyNode> nodes = new List<DependencyNode>(), newNodes;
			((IList<DependencyNode>)nodes).AddRange(findAllDependentsVectorSortedByDistanceToPProjNode(dg, start, avoid, true, nodePath));
			while (nodes.Count > 0)
			{
				dependent = nodes.RemoveAt(0);
				if (((newNodes = findAllDependentsVectorSortedByDistanceToPProjNode(dg, dependent, avoid, true, nodePath)).Count) == 0)
				{
					return dependent;
				}
				((IList<DependencyNode>)nodes).AddRange(newNodes);
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean deprojectivizeWithHeadAndPath(org.maltparser.core.syntaxgraph.DependencyStructure pdg, org.maltparser.core.syntaxgraph.node.DependencyNode node, boolean[] nodeLifted, boolean[] nodePath, String[] synacticHeadDeprel, org.maltparser.core.symbol.SymbolTable deprelSymbolTable) throws org.maltparser.core.exception.MaltChainedException
		private bool deprojectivizeWithHeadAndPath(DependencyStructure pdg, DependencyNode node, bool[] nodeLifted, bool[] nodePath, string[] synacticHeadDeprel, SymbolTable deprelSymbolTable)
		{
			bool success = true, childSuccess = false;
			int i, childAttempts = 2;
			DependencyNode possibleSyntacticHead;
			if (node.hasHead() && node.HeadEdge.Labeled && nodeLifted[node.Index] && nodePath[node.Index])
			{
				possibleSyntacticHead = breadthFirstSearchSortedByDistanceForHeadAndPath(pdg, node.Head, node, synacticHeadDeprel[node.Index], nodePath, deprelSymbolTable);
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
				possibleSyntacticHead = breadthFirstSearchSortedByDistanceForHeadAndPath(pdg, node.Head, node, synacticHeadDeprel[node.Index], nodePath, deprelSymbolTable);
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
				IList<DependencyNode> children = node.ListOfDependents;
				for (i = 0; i < children.Count; i++)
				{
					if (!deprojectivizeWithHeadAndPath(pdg, children[i], nodeLifted, nodePath, synacticHeadDeprel, deprelSymbolTable))
					{
						childSuccess = false;
					}
				}
				childAttempts--;
			}
			return childSuccess && success;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.core.syntaxgraph.node.DependencyNode breadthFirstSearchSortedByDistanceForHeadAndPath(org.maltparser.core.syntaxgraph.DependencyStructure dg, org.maltparser.core.syntaxgraph.node.DependencyNode start, org.maltparser.core.syntaxgraph.node.DependencyNode avoid, String syntacticHeadDeprelCode, boolean[] nodePath, org.maltparser.core.symbol.SymbolTable deprelSymbolTable) throws org.maltparser.core.exception.MaltChainedException
		private DependencyNode breadthFirstSearchSortedByDistanceForHeadAndPath(DependencyStructure dg, DependencyNode start, DependencyNode avoid, string syntacticHeadDeprelCode, bool[] nodePath, SymbolTable deprelSymbolTable)
		{
			DependencyNode dependent;
			IList<DependencyNode> nodes = new List<DependencyNode>(), newNodes = null, secondChance = new List<DependencyNode>();
			((IList<DependencyNode>)nodes).AddRange(findAllDependentsVectorSortedByDistanceToPProjNode(dg, start, avoid, true, nodePath));
			while (nodes.Count > 0)
			{
				dependent = nodes.RemoveAt(0);
				if (((newNodes = findAllDependentsVectorSortedByDistanceToPProjNode(dg, dependent, avoid, true, nodePath)).Count) == 0 && deprelSymbolTable.getSymbolCodeToString(dependent.HeadEdge.getLabelCode(deprelSymbolTable)).Equals(syntacticHeadDeprelCode))
				{
					return dependent;
				}
				((IList<DependencyNode>)nodes).AddRange(newNodes);
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