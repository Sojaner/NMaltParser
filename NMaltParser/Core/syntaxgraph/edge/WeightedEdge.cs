using System.Text;

namespace NMaltParser.Core.SyntaxGraph.Edge
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class WeightedEdge : GraphEdge, Weightable
	{
		private double? weight = Double.NaN;

		public WeightedEdge()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public WeightedEdge(org.maltparser.core.syntaxgraph.node.Node source, org.maltparser.core.syntaxgraph.node.Node target, int type) throws org.maltparser.core.exception.MaltChainedException
		public WeightedEdge(Node.Node source, Node.Node target, int type) : base(source, target, type)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public WeightedEdge(org.maltparser.core.syntaxgraph.node.Node source, org.maltparser.core.syntaxgraph.node.Node target, int type, System.Nullable<double> weight) throws org.maltparser.core.exception.MaltChainedException
		public WeightedEdge(Node.Node source, Node.Node target, int type, double? weight) : base(source, target, type)
		{
			Weight = weight.Value;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
			base.clear();
			weight = Double.NaN;
		}

		public virtual double Weight
		{
			get
			{
				return weight.Value;
			}
			set
			{
				weight = value;
			}
		}


		public virtual int CompareTo(WeightedEdge that)
		{
			if (this == that)
			{
				return 0;
			}
			int comparison = weight.compareTo(that.Weight);
			if (comparison != 0)
			{
				return comparison;
			}

			return base.CompareTo(that);
		}

		public override bool Equals(object obj)
		{
			WeightedEdge e = (WeightedEdge)obj;
			return weight.Equals(e.Weight) && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			int hash = 7;
			hash = 31 * hash + (null == weight ? 0 : weight.GetHashCode());
			return 31 * hash + base.GetHashCode();
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(Weight);
			sb.Append(' ');
			sb.Append(Source.Index);
			sb.Append("->");
			sb.Append(Target.Index);
			sb.Append(' ');
			sb.Append(base.ToString());
			return sb.ToString();
		}
	}

}