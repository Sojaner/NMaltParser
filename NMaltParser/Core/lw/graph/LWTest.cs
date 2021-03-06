﻿using System;
using System.Collections.Generic;
using System.IO;
using NMaltParser.Concurrent.Graph.DataFormat;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.Symbol.Hash;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Utilities;

namespace NMaltParser.Core.LW.Graph
{
    public class LWTest
	{
		private const string IGNORE_COLUMN_SIGN = "_";
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String[] readSentences(java.io.BufferedReader reader) throws java.io.IOException
		public static string[] readSentences(StreamReader reader)
		{
			List<string> tokens = new List<string>();
			string line;
			while (!ReferenceEquals((line = reader.ReadLine()), null))
			{
				if (line.Trim().Length == 0)
				{
					break;
				}
				else
				{
					tokens.Add(line.Trim());
				}

			}
			return tokens.ToArray();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.maltparser.core.syntaxgraph.DependencyStructure getOldDependencyGraph(org.maltparser.concurrent.graph.dataformat.DataFormat dataFormat, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandlers, String[] tokens) throws org.maltparser.core.exception.MaltChainedException
		public static IDependencyStructure getOldDependencyGraph(DataFormat dataFormat, SymbolTableHandler symbolTableHandlers, string[] tokens)
		{
			IDependencyStructure oldGraph = new DependencyGraph(symbolTableHandlers);
			for (int i = 0; i < tokens.Length; i++)
			{
				oldGraph.AddDependencyNode(i + 1);
			}
			for (int i = 0; i < tokens.Length; i++)
			{
				DependencyNode node = oldGraph.GetDependencyNode(i + 1);
				string[] items = tokens[i].Split("\t", true);
				Edge edge = null;
				for (int j = 0; j < items.Length; j++)
				{
					ColumnDescription column = dataFormat.GetColumnDescription(j);

					if (column.Category == ColumnDescription.Input && node != null)
					{
						oldGraph.AddLabel(node, column.Name, items[j]);
					}
					else if (column.Category == ColumnDescription.Head)
					{
						if (column.Category != ColumnDescription.Ignore && !items[j].Equals(IGNORE_COLUMN_SIGN))
						{
							edge = oldGraph.AddDependencyEdge(int.Parse(items[j]), i + 1);
						}
					}
					else if (column.Category == ColumnDescription.DependencyEdgeLabel && edge != null)
					{
						oldGraph.AddLabel(edge, column.Name, items[j]);
					}
				}
			}

			oldGraph.SetDefaultRootEdgeLabel(oldGraph.SymbolTables.getSymbolTable("DEPREL"), "ROOT");
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
				DataFormat dataFormat = DataFormat.ParseDataFormatXmLFile("/appdata/dataformat/conllx.xml");
				reader = new StreamReader(new FileStream(inFile, FileMode.Open, FileAccess.Read), charSet);
				int sentenceCounter = 0;
				while (true)
				{
					string[] goldTokens = readSentences(reader);
					if (goldTokens.Length == 0)
					{
						break;
					}
					sentenceCounter++;
					SymbolTableHandler newTable = new HashSymbolTableHandler();
					IDependencyStructure newGraph = new LWDependencyGraph(dataFormat, newTable, goldTokens, "ROOT");
	//	    		SymbolTableHandler oldTable = new HashSymbolTableHandler();
	//	    		DependencyStructure oldGraph = getOldDependencyGraph(dataFormat, oldTable, goldTokens);
					int newGraphINT;
					int oldGraphINT;
					bool newGraphBOOL;
					bool oldGraphBOOL;
					SortedSet<LWNode> newGraphSortedSet;
					SortedSet<DependencyNode> oldGraphSortedSet;

	//	    		for (int i = 0; i < newGraph.nDependencyNode(); i++) {
	//	    			newGraphINT = newGraph.getDependencyNode(i).getIndex();
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).getIndex();


	//	    			newGraphINT = newGraph.getNode(i).getHeadIndex();
	//	    			newGraphINT = newGraph.getDependencyNode(i).getHead() != null ? newGraph.getDependencyNode(i).getHead().getIndex() : -1;
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).getHead() != null ? oldGraph.getDependencyNode(i).getHead().getIndex() : -1;


	//	    			newGraphINT = newGraph.getDependencyNode(i).getPredecessor() != null ? newGraph.getDependencyNode(i).getPredecessor().getIndex() : -1;
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).getPredecessor() != null ? oldGraph.getDependencyNode(i).getPredecessor().getIndex() : -1;

	//	    			newGraphINT = newGraph.getTokenNode(i).getSuccessor() != null ? newGraph.getTokenNode(i).getSuccessor().getIndex() : -1;
	//	    			oldGraphINT = oldGraph.getTokenNode(i).getSuccessor() != null ? oldGraph.getTokenNode(i).getSuccessor().getIndex() : -1;

	//	    			newGraphINT = newGraph.getDependencyNode(i).getLeftDependentCount();
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).getLeftDependentCount();
	//
	//	    			newGraphINT = newGraph.getDependencyNode(i).getRightDependentCount();
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).getRightDependentCount();

	//	    			newGraphINT = newGraph.getDependencyNode(i).getRightmostDependent() != null ? newGraph.getNode(i).getRightmostDependent().getIndex() : -1;
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).getRightmostDependent() != null ? oldGraph.getDependencyNode(i).getRightmostDependent	().getIndex() : -1;
	//	    			newGraphINT = newGraph.getDependencyNode(i).findComponent().getIndex();
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).findComponent().getIndex();
	//
	//	    			newGraphINT = newGraph.getDependencyNode(i).getRank();
	//	    			oldGraphINT = oldGraph.getDependencyNode(i).getRank();


	//	    			newGraphBOOL = newGraph.getDependencyNode(i).isRoot();
	//	    			oldGraphBOOL = oldGraph.getDependencyNode(i).isRoot();

	//	    			newGraphBOOL = newGraph.getDependencyNode(i).hasRightDependent();
	//	    			oldGraphBOOL = oldGraph.getDependencyNode(i).hasRightDependent();

	//	    			newGraphBOOL = newGraph.getDependencyNode(i).hasHead();
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
	//	    			if (newGraphINT != oldGraphINT) {
	//	    				System.out.println(newGraphINT + "\t" + oldGraphINT);
	//	    			}
	//	    		}


	//	    		System.out.println(oldGraph);
				}
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (LWGraphException e)
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