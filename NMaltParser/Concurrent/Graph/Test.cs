using System;
using System.Collections.Generic;
using System.IO;

namespace org.maltparser.concurrent.graph
{

	using  org.maltparser.concurrent.graph.dataformat;
	using  org.maltparser.concurrent.graph.dataformat;
	using  org.maltparser.core.exception;
	using  org.maltparser.core.symbol.hash;
	using  org.maltparser.core.syntaxgraph;
	using  org.maltparser.core.syntaxgraph.edge;
	using  org.maltparser.core.syntaxgraph.node;

	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class Test
	{
		private const string IGNORE_COLUMN_SIGN = "_";
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.maltparser.core.syntaxgraph.DependencyStructure getOldDependencyGraph(org.maltparser.concurrent.graph.dataformat.DataFormat dataFormat, String[] tokens) throws org.maltparser.core.exception.MaltChainedException
		public static DependencyStructure getOldDependencyGraph(DataFormat dataFormat, string[] tokens)
		{
			DependencyStructure oldGraph = new org.maltparser.core.syntaxgraph.DependencyGraph(new HashSymbolTableHandler());
			for (int i = 0; i < tokens.Length; i++)
			{
				oldGraph.addDependencyNode(i + 1);
			}
			for (int i = 0; i < tokens.Length; i++)
			{
				DependencyNode node = oldGraph.getDependencyNode(i + 1);
				string[] items = tokens[i].Split("\t", true);
				Edge edge = null;
				for (int j = 0; j < items.Length; j++)
				{
					ColumnDescription column = dataFormat.getColumnDescription(j);

					if (column.Category == ColumnDescription.INPUT && node != null)
					{
						oldGraph.addLabel(node, column.Name, items[j]);
					}
					else if (column.Category == ColumnDescription.HEAD)
					{
						if (column.Category != ColumnDescription.IGNORE && !items[j].Equals(IGNORE_COLUMN_SIGN))
						{
							edge = oldGraph.addDependencyEdge(int.Parse(items[j]), i + 1);
						}
					}
					else if (column.Category == ColumnDescription.DEPENDENCY_EDGE_LABEL && edge != null)
					{
						oldGraph.addLabel(edge, column.Name, items[j]);
					}
				}
			}

			oldGraph.setDefaultRootEdgeLabel(oldGraph.SymbolTables.getSymbolTable("DEPREL"), "ROOT");
			return oldGraph;
		}

		public static void Main(string[] args)
		{
			long startTime = DateTimeHelper.CurrentUnixTimeMillis();
			string inFile = args[0];
			string charSet = "UTF-8";

			StreamReader reader = null;

			try
			{
				DataFormat dataFormat = DataFormat.parseDataFormatXMLfile("/appdata/dataformat/conllx.xml");
				reader = new StreamReader(new FileStream(inFile, FileMode.Open, FileAccess.Read), charSet);
				int sentenceCounter = 0;
				while (true)
				{
					string[] goldTokens = ConcurrentUtils.readSentence(reader);
					if (goldTokens.Length == 0)
					{
						break;
					}
					sentenceCounter++;
					ConcurrentDependencyGraph newGraph = new ConcurrentDependencyGraph(dataFormat, goldTokens);
					DependencyStructure oldGraph = getOldDependencyGraph(dataFormat, goldTokens);
					int newGraphINT;
					int oldGraphINT;
					bool newGraphBOOL;
					bool oldGraphBOOL;
					SortedSet<ConcurrentDependencyNode> newGraphSortedSet;
					SortedSet<DependencyNode> oldGraphSortedSet;

					for (int i = 0; i < newGraph.nDependencyNodes(); i++)
					{
	//	    			newGraphINT = newGraph.getNode(i).getIndex();
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).getIndex();

	//	    			newGraphINT = newGraph.getNode(i).getHeadIndex();
	//	    			newGraphINT = newGraph.getNode(i).getHead() != null ? newGraph.getNode(i).getHead().getIndex() : -1;
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).getHead() != null ? oldGraph.getDependencyNode(i).getHead().getIndex() : -1;


	//	    			newGraphINT = newGraph.getNode(i).getPredecessor() != null ? newGraph.getNode(i).getPredecessor().getIndex() : -1;
	//	    			oldGraphINT = oldGraph.getTokenNode(i).getPredecessor() != null ? oldGraph.getTokenNode(i).getPredecessor().getIndex() : -1;

	//	    			newGraphINT = newGraph.getNode(i).getSuccessor() != null ? newGraph.getNode(i).getSuccessor().getIndex() : -1;
	//	    			oldGraphINT = oldGraph.getTokenNode(i).getSuccessor() != null ? oldGraph.getTokenNode(i).getSuccessor().getIndex() : -1;

	//	    			newGraphINT = newGraph.getNode(i).getLeftDependentCount();
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).getLeftDependentCount();
	//
	//	    			newGraphINT = newGraph.getNode(i).getRightDependentCount();
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).getRightDependentCount();

	//	    			newGraphINT = newGraph.getNode(i).getRightmostDependent() != null ? newGraph.getNode(i).getRightmostDependent().getIndex() : -1;
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).getRightmostDependent() != null ? oldGraph.getDependencyNode(i).getRightmostDependent	().getIndex() : -1;
						newGraphINT = newGraph.getDependencyNode(i).findComponent().Index;
						oldGraphINT = oldGraph.getDependencyNode(i).findComponent().Index;

						newGraphINT = newGraph.getDependencyNode(i).Rank;
						oldGraphINT = oldGraph.getDependencyNode(i).Rank;
						if (newGraphINT != oldGraphINT)
						{
							Console.WriteLine(newGraphINT + "\t" + oldGraphINT);
						}

	//	    			newGraphBOOL = newGraph.getNode(i).isRoot();
	//	    			oldGraphBOOL = oldGraph.getDependencyNode(i).isRoot();

	//	    			newGraphBOOL = newGraph.getNode(i).hasRightDependent();
	//	    			oldGraphBOOL = oldGraph.getDependencyNode(i).hasRightDependent();

	//	    			newGraphBOOL = newGraph.getNode(i).hasHead();
	//	    			oldGraphBOOL = oldGraph.getDependencyNode(i).hasHead();
	//	    	    	if (newGraphBOOL != oldGraphBOOL) {
	//	    	    		System.out.println(newGraphBOOL + "\t" + oldGraphBOOL);
	//	    	    	}

	//		    		newGraphSortedSet = newGraph.getNode(i).getRightDependents();
	//		    		oldGraphSortedSet = oldGraph.getDependencyNode(i).getLeftDependents();
	//		    		if (newGraphSortedSet.size() != oldGraphSortedSet.size()) {
	//		    			System.out.println(newGraphSortedSet + "\t" + oldGraphSortedSet);
	//		    		} else {
	//		    			Iterator<DependencyNode> it = oldGraphSortedSet.iterator();
	//		    			for (Node n : newGraphSortedSet) {
	//		    				DependencyNode o = it.next();
	//		    				if (n.getIndex() != o.getIndex()) {
	//		    					System.out.println(n.getIndex() + "\t" + o.getIndex());
	//		    				}
	//		    			}
	//		    		}

					}


	//	    		System.out.println(oldGraph);
				}
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (ConcurrentGraphException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (MaltChainedException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				if (reader != null)
				{
					try
					{
						reader.Close();
					}
					catch (IOException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
			}
			long elapsed = DateTimeHelper.CurrentUnixTimeMillis() - startTime;
			Console.WriteLine("Finished init basic   : " + (new Formatter()).format("%02d:%02d:%02d", elapsed / 3600000, elapsed % 3600000 / 60000, elapsed % 60000 / 1000) + " (" + elapsed + " ms)");
		}


	}

}