using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.syntaxgraph
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.flow;
	using  org.maltparser.core.flow.item;
	using  org.maltparser.core.flow.spec;
	using  org.maltparser.core.options;
	using  org.maltparser.core.symbol;
	using  org.maltparser.core.syntaxgraph.edge;
	using  org.maltparser.core.syntaxgraph.node;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class CopyChartItem : ChartItem
	{
		private string idName;
		private string targetName;
		private string sourceName;
		private string taskName;
		private bool usePartialTree;

		private TokenStructure cachedSource = null;
		private TokenStructure cachedTarget = null;

		public CopyChartItem()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(org.maltparser.core.flow.FlowChartInstance flowChartinstance, org.maltparser.core.flow.spec.ChartItemSpecification chartItemSpecification) throws org.maltparser.core.exception.MaltChainedException
		public override void initialize(FlowChartInstance flowChartinstance, ChartItemSpecification chartItemSpecification)
		{
			base.initialize(flowChartinstance, chartItemSpecification);
			foreach (string key in chartItemSpecification.ChartItemAttributes.Keys)
			{
				if (key.Equals("id"))
				{
					idName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("target"))
				{
					targetName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("source"))
				{
					sourceName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("task"))
				{
					taskName = chartItemSpecification.ChartItemAttributes[key];
				}
			}
			if (string.ReferenceEquals(idName, null))
			{
				idName = getChartElement("copy").Attributes.get("id").DefaultValue;
			}
			else if (string.ReferenceEquals(targetName, null))
			{
				targetName = getChartElement("copy").Attributes.get("target").DefaultValue;
			}
			else if (string.ReferenceEquals(sourceName, null))
			{
				sourceName = getChartElement("copy").Attributes.get("source").DefaultValue;
			}
			else if (string.ReferenceEquals(taskName, null))
			{
				taskName = getChartElement("copy").Attributes.get("task").DefaultValue;
			}
			usePartialTree = OptionManager.instance().getOptionValue(OptionContainerIndex, "singlemalt", "use_partial_tree").ToString().Equals("true");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int preprocess(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int preprocess(int signal)
		{
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int process(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int process(int signal)
		{
			if (taskName.Equals("terminals"))
			{
				if (cachedSource == null)
				{
					cachedSource = (TokenStructure)flowChartinstance.getFlowChartRegistry(typeof(org.maltparser.core.syntaxgraph.TokenStructure), sourceName);
				}
				if (cachedTarget == null)
				{
					cachedTarget = (TokenStructure)flowChartinstance.getFlowChartRegistry(typeof(org.maltparser.core.syntaxgraph.TokenStructure), targetName);
				}
				copyTerminalStructure(cachedSource, cachedTarget);
	//			SystemLogger.logger().info("usePartialTree:" + usePartialTree);
				if (usePartialTree && cachedSource is DependencyStructure && cachedTarget is DependencyStructure)
				{
					copyPartialDependencyStructure((DependencyStructure)cachedSource, (DependencyStructure)cachedTarget);
				}
			}
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int postprocess(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int postprocess(int signal)
		{
			return signal;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public override void terminate()
		{
			cachedSource = null;
			cachedTarget = null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copyTerminalStructure(TokenStructure sourceGraph, TokenStructure targetGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void copyTerminalStructure(TokenStructure sourceGraph, TokenStructure targetGraph)
		{
			targetGraph.clear();
			foreach (int index in sourceGraph.TokenIndices)
			{
				DependencyNode gnode = sourceGraph.getTokenNode(index);
				DependencyNode pnode = targetGraph.addTokenNode(gnode.Index);
				foreach (SymbolTable table in gnode.LabelTypes)
				{
					pnode.addLabel(table, gnode.getLabelSymbol(table));
				}
			}
			if (sourceGraph.hasComments())
			{
				for (int i = 1; i <= sourceGraph.nTokenNode() + 1; i++)
				{
					List<string> commentList = sourceGraph.getComment(i);
					if (commentList != null)
					{
						for (int j = 0; j < commentList.Count;j++)
						{
							targetGraph.addComment(commentList[j], i);
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copyPartialDependencyStructure(DependencyStructure sourceGraph, DependencyStructure targetGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void copyPartialDependencyStructure(DependencyStructure sourceGraph, DependencyStructure targetGraph)
		{
			SymbolTable partHead = cachedSource.SymbolTables.getSymbolTable("PARTHEAD");
			SymbolTable partDeprel = cachedSource.SymbolTables.getSymbolTable("PARTDEPREL");
			if (partHead == null || partDeprel == null)
			{
				return;
			}
			SymbolTable deprel = cachedTarget.SymbolTables.getSymbolTable("DEPREL");
			foreach (int index in sourceGraph.TokenIndices)
			{
				DependencyNode snode = sourceGraph.getTokenNode(index);
				DependencyNode tnode = targetGraph.getTokenNode(index);
				if (snode != null && tnode != null)
				{
					int spartheadindex = int.Parse(snode.getLabelSymbol(partHead));
					string spartdeprel = snode.getLabelSymbol(partDeprel);
					if (spartheadindex > 0)
					{
						Edge tedge = targetGraph.addDependencyEdge(spartheadindex, snode.Index);
						tedge.addLabel(deprel, spartdeprel);
					}
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
			return obj.ToString().Equals(this.ToString());
		}

		public override int GetHashCode()
		{
			return 217 + (null == ToString() ? 0 : ToString().GetHashCode());
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("    copy ");
			sb.Append("id:");
			sb.Append(idName);
			sb.Append(' ');
			sb.Append("task:");
			sb.Append(taskName);
			sb.Append(' ');
			sb.Append("source:");
			sb.Append(sourceName);
			sb.Append(' ');
			sb.Append("target:");
			sb.Append(targetName);
			return sb.ToString();
		}
	}

}