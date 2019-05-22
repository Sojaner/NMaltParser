using System;
using System.Collections.Generic;

namespace org.maltparser.parser.algorithm.twoplanar
{
    using  core.syntaxgraph;
	using  core.syntaxgraph.edge;
	using  core.syntaxgraph.node;
	using  history;
	using  history.action;
	/// <summary>
	/// @author Carlos Gomez Rodriguez
	/// 
	/// </summary>
	public class TwoPlanarArcEagerOracle : Oracle
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TwoPlanarArcEagerOracle(org.maltparser.parser.DependencyParserConfig manager, org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public TwoPlanarArcEagerOracle(DependencyParserConfig manager, GuideUserHistory history) : base(manager, history)
		{
			GuideName = "Two-Planar";
		}

		/// <summary>
		/// Give this map an edge in the gold standard data, and it will tell you whether, given the links already created by the oracle, it is possible 
		/// to create the corresponding edge in one of the planes, in any plane, or in none at all (the latter will happen in non-2-planar structures). 
		/// </summary>
		private const int ANY_PLANE = 0;
		private const int FIRST_PLANE = 1;
		private const int SECOND_PLANE = 2;
		private const int NO_PLANE = 3;
		private IDictionary<Edge, int> linksToPlanes = new IdentityHashMap<Edge, int>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction predict(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction predict(DependencyStructure gold, ParserConfiguration config)
		{
			TwoPlanarConfig planarConfig = (TwoPlanarConfig)config;
			DependencyStructure dg = planarConfig.DependencyGraph;
			DependencyNode activeStackPeek = planarConfig.ActiveStack.Peek();
			DependencyNode inactiveStackPeek = planarConfig.InactiveStack.Peek();
			int activeStackPeekIndex = activeStackPeek.Index;
			int inactiveStackPeekIndex = inactiveStackPeek.Index;
			int inputPeekIndex = planarConfig.Input.Peek().Index;

			//System.out.println("Initting crossings");

			if (crossingsGraph == null)
			{
				initCrossingsGraph(gold);
			}

			//System.out.println("Crossings initted");

			if (!activeStackPeek.Root && gold.getTokenNode(activeStackPeekIndex).Head.Index == inputPeekIndex && !checkIfArcExists(dg, inputPeekIndex, activeStackPeekIndex))
			{
				if (planarConfig.StackActivityState == TwoPlanarConfig.FIRST_STACK)
				{
					propagatePlaneConstraint(gold.getTokenNode(activeStackPeekIndex).HeadEdge, FIRST_PLANE);
				}
				else
				{
					propagatePlaneConstraint(gold.getTokenNode(activeStackPeekIndex).HeadEdge, SECOND_PLANE);
				}
				//System.out.println("From " + inputPeekIndex + " to " + activeStackPeekIndex);
				return updateActionContainers(TwoPlanar.LEFTARC, gold.getTokenNode(activeStackPeekIndex).HeadEdge.LabelSet);
			}

			else if (gold.getTokenNode(inputPeekIndex).Head.Index == activeStackPeekIndex && !checkIfArcExists(dg, activeStackPeekIndex, inputPeekIndex))
			{
				if (planarConfig.StackActivityState == TwoPlanarConfig.FIRST_STACK)
				{
					propagatePlaneConstraint(gold.getTokenNode(inputPeekIndex).HeadEdge, FIRST_PLANE);
				}
				else
				{
					propagatePlaneConstraint(gold.getTokenNode(inputPeekIndex).HeadEdge, SECOND_PLANE);
				}
				//System.out.println("From " + activeStackPeekIndex + " to " + inputPeekIndex);
				return updateActionContainers(TwoPlanar.RIGHTARC, gold.getTokenNode(inputPeekIndex).HeadEdge.LabelSet);
			}

			else if (!inactiveStackPeek.Root && gold.getTokenNode(inactiveStackPeekIndex).Head.Index == inputPeekIndex && !checkIfArcExists(dg, inputPeekIndex, inactiveStackPeekIndex))
			{
				//need to create link, but on the other plane!!
				//TODO is this if branch really necessary? i.e. will this happen? (later branches already switch)
				//System.out.println("Switch one");
				return updateActionContainers(TwoPlanar.SWITCH, null);
			}

			else if (gold.getTokenNode(inputPeekIndex).Head.Index == inactiveStackPeekIndex && !checkIfArcExists(dg, inactiveStackPeekIndex, inputPeekIndex))
			{
				//need to create link, but on the other plane!!
				//TODO is this if branch really necessary? i.e. will this happen? (later branches already switch)
				//System.out.println("Switch two");
				return updateActionContainers(TwoPlanar.SWITCH, null);
			}

			else if (getFirstPendingLinkOnActivePlane(planarConfig,gold) != null)
			{
				//System.out.println("Reduce one");
				return updateActionContainers(TwoPlanar.REDUCE, null);
			}

			else if (getFirstPendingLinkOnInactivePlane(planarConfig,gold) != null)
			{
				//System.out.println("Switch for reducing");
				return updateActionContainers(TwoPlanar.SWITCH, null);
			}

			//TODO: double reduce somehow? (check if reduced node is not covered by links of the other plane, or something like that).

			else
			{
				//System.out.println("Shift");
				return updateActionContainers(TwoPlanar.SHIFT, null);
			}

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean checkIfArcExists(org.maltparser.core.syntaxgraph.DependencyStructure dg, int index1, int index2) throws org.maltparser.core.exception.MaltChainedException
		private bool checkIfArcExists(DependencyStructure dg, int index1, int index2)
		{
			return dg.getTokenNode(index2).hasHead() && dg.getTokenNode(index2).Head.Index == index1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public override void finalizeSentence(DependencyStructure dependencyGraph)
		{
			crossingsGraph = null;
			linksToPlanes.Clear();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public override void terminate()
		{
		}



		private static bool cross(Edge e1, Edge e2)
		{
			int xSource = e1.Source.Index;
			int xTarget = e1.Target.Index;
			int ySource = e2.Source.Index;
			int yTarget = e2.Target.Index;
			int xMin = Math.Min(xSource,xTarget);
			int xMax = Math.Max(xSource,xTarget);
			int yMin = Math.Min(ySource,yTarget);
			int yMax = Math.Max(ySource,yTarget);
			//System.out.println(xMin+":"+xMax+":"+yMin+":"+yMax);
			return (xMin < yMin && yMin < xMax && xMax < yMax) || (yMin < xMin && xMin < yMax && yMax < xMax);
		}

		private IDictionary<Edge, IList<Edge>> crossingsGraph = null;

		private void initCrossingsGraph(DependencyStructure dg)
		{
			crossingsGraph = new IdentityHashMap<Edge, IList<Edge>>();
			SortedSet<Edge> edges = dg.Edges;
			//System.out.println(edges.size());
			//System.out.println(dg.nEdges());
			for (IEnumerator<Edge> iterator1 = edges.GetEnumerator(); iterator1.MoveNext();)
			{
				Edge edge1 = iterator1.Current;
				for (IEnumerator<Edge> iterator2 = edges.GetEnumerator(); iterator2.MoveNext();)
				{
					Edge edge2 = iterator2.Current;
					if (edge1.Source.Index < edge2.Source.Index && cross(edge1,edge2))
					{
						//System.out.println("Crossing!");
						IList<Edge> crossingEdge1 = crossingsGraph[edge1];
						if (crossingEdge1 == null)
						{
							crossingEdge1 = new LinkedList<Edge>();
							crossingsGraph[edge1] = crossingEdge1;
						}
						crossingEdge1.Add(edge2);
						IList<Edge> crossingEdge2 = crossingsGraph[edge2];
						if (crossingEdge2 == null)
						{
							crossingEdge2 = new LinkedList<Edge>();
							crossingsGraph[edge2] = crossingEdge2;
						}
						crossingEdge2.Add(edge1);
					}
				}
			}
		}

		private IList<Edge> getCrossingEdges(Edge e)
		{
			return crossingsGraph[e];
		}

		private void setPlaneConstraint(Edge e, int requiredPlane)
		{
			linksToPlanes[e] = requiredPlane;
		}

		private int getPlaneConstraint(Edge e)
		{
			int? constr = linksToPlanes[e];
			if (constr == null)
			{
				setPlaneConstraint(e,ANY_PLANE);
				return ANY_PLANE;
			}
			else
			{
				return constr.Value;
			}
		}

		private void propagatePlaneConstraint(Edge e, int requiredPlane)
		{
			setPlaneConstraint(e,requiredPlane);
			if (requiredPlane == FIRST_PLANE || requiredPlane == SECOND_PLANE)
			{
				IList<Edge> crossingEdges = getCrossingEdges(e);
				if (crossingEdges != null)
				{
					for (IEnumerator<Edge> iterator = crossingEdges.GetEnumerator(); iterator.MoveNext();)
					{
						Edge crossingEdge = iterator.Current;
						assert(requiredPlane == FIRST_PLANE || requiredPlane == SECOND_PLANE);
						int crossingEdgeConstraint = getPlaneConstraint(crossingEdge);
						if (crossingEdgeConstraint == ANY_PLANE)
						{
							if (requiredPlane == FIRST_PLANE)
							{
								propagatePlaneConstraint(crossingEdge,SECOND_PLANE);
							}
							else if (requiredPlane == SECOND_PLANE)
							{
								propagatePlaneConstraint(crossingEdge,FIRST_PLANE);
							}
						}
						else if (crossingEdgeConstraint == NO_PLANE)
						{
							;
						}
						else if (crossingEdgeConstraint == FIRST_PLANE)
						{
							if (requiredPlane == FIRST_PLANE)
							{
								propagatePlaneConstraint(crossingEdge,NO_PLANE);
							}
						}
						else if (crossingEdgeConstraint == SECOND_PLANE)
						{
							if (requiredPlane == SECOND_PLANE)
							{
								propagatePlaneConstraint(crossingEdge,NO_PLANE);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Decides in which plane link e should be created.
		/// </summary>
		private int getLinkDecision(Edge e, TwoPlanarConfig config)
		{
			int constraint = getPlaneConstraint(e);
			if (constraint == ANY_PLANE)
			{
				//choose active plane
				if (config.StackActivityState == TwoPlanarConfig.FIRST_STACK)
				{
					return FIRST_PLANE;
				}
				else
				{
					return SECOND_PLANE;
				}
			}
			else
			{
				return constraint;
			}
		}


		/// <summary>
		/// Gets the shortest pending link between (to or from) the input node and a node to the left of the top of the active stack,
		/// such that the link can be established on the active plane.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.core.syntaxgraph.edge.Edge getFirstPendingLinkOnActivePlane(TwoPlanarConfig config, org.maltparser.core.syntaxgraph.DependencyStructure gold) throws org.maltparser.core.exception.MaltChainedException
		private Edge getFirstPendingLinkOnActivePlane(TwoPlanarConfig config, DependencyStructure gold)
		{
			return getFirstPendingLinkOnPlane(config, gold, config.StackActivityState == TwoPlanarConfig.FIRST_STACK ? FIRST_PLANE : SECOND_PLANE, config.ActiveStack.Peek().Index);
		}

		/// <summary>
		/// Gets the shortest pending link between (to or from) the input node and a node to the left of the top of the inactive stack, 
		/// such that the link can be established on the inactive plane.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.core.syntaxgraph.edge.Edge getFirstPendingLinkOnInactivePlane(TwoPlanarConfig config, org.maltparser.core.syntaxgraph.DependencyStructure gold) throws org.maltparser.core.exception.MaltChainedException
		private Edge getFirstPendingLinkOnInactivePlane(TwoPlanarConfig config, DependencyStructure gold)
		{
			return getFirstPendingLinkOnPlane(config, gold, config.StackActivityState == TwoPlanarConfig.FIRST_STACK ? SECOND_PLANE : FIRST_PLANE, config.InactiveStack.Peek().Index);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.core.syntaxgraph.edge.Edge getFirstPendingLinkOnAnyPlane(TwoPlanarConfig config, org.maltparser.core.syntaxgraph.DependencyStructure gold) throws org.maltparser.core.exception.MaltChainedException
		private Edge getFirstPendingLinkOnAnyPlane(TwoPlanarConfig config, DependencyStructure gold)
		{
			Edge e1 = getFirstPendingLinkOnActivePlane(config, gold);
			Edge e2 = getFirstPendingLinkOnInactivePlane(config, gold);
			int left1 = Math.Min(e1.Source.Index, e1.Target.Index);
			int left2 = Math.Min(e2.Source.Index, e2.Target.Index);
			if (left1 > left2)
			{
				return e1;
			}
			else
			{
				return e2;
			}
		}

		/// <summary>
		/// Gets the shortest pending link between (to or from) the input node and a node to the left of rightmostLimit, such that the link
		/// can be established on the given plane.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.core.syntaxgraph.edge.Edge getFirstPendingLinkOnPlane(TwoPlanarConfig config, org.maltparser.core.syntaxgraph.DependencyStructure gold, int plane, int rightmostLimit) throws org.maltparser.core.exception.MaltChainedException
		private Edge getFirstPendingLinkOnPlane(TwoPlanarConfig config, DependencyStructure gold, int plane, int rightmostLimit)
		{
			TwoPlanarConfig planarConfig = (TwoPlanarConfig)config;
			//DependencyStructure dg = planarConfig.getDependencyGraph(); -> no need, if rightmostLimit is well chosen, due to algorithm invariants
			int inputPeekIndex = planarConfig.Input.Peek().Index;

			Edge current = null;
			int maxIndex;
			if (planarConfig.getRootHandling() == TwoPlanarConfig.NORMAL)
			{
				maxIndex = -1; //count links from dummy root
			}
			else
			{
				maxIndex = 0; //do not count links from dummy root
			}

			if (gold.getTokenNode(inputPeekIndex).hasLeftDependent() && gold.getTokenNode(inputPeekIndex).LeftmostDependent.Index < rightmostLimit)
			{
				SortedSet<DependencyNode> dependents = gold.getTokenNode(inputPeekIndex).LeftDependents;
				for (IEnumerator<DependencyNode> iterator = dependents.GetEnumerator(); iterator.MoveNext();)
				{
					DependencyNode dependent = (DependencyNode) iterator.Current;
					if (dependent.Index > maxIndex && dependent.Index < rightmostLimit && getLinkDecision(dependent.HeadEdge,config) == plane)
					{
						maxIndex = dependent.Index;
						current = dependent.HeadEdge;
					}
				}
			}

			//at this point, current is the first left-pointing link, but we have to check right-pointing link as well

			//System.out.println("in" + inputPeekIndex + " rl" + rightmostLimit);
			if (gold.getTokenNode(inputPeekIndex).Head.Index < rightmostLimit)
			{
				//System.out.println(":");
				if (gold.getTokenNode(inputPeekIndex).Head.Index > maxIndex && getLinkDecision(gold.getTokenNode(inputPeekIndex).HeadEdge,config) == plane)
				{
					//System.out.println("::");
					current = gold.getTokenNode(inputPeekIndex).HeadEdge;
				}
			}

			return current;


		}


	}

}