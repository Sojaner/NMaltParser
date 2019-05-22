using System.Collections.Generic;

namespace NMaltParser.ML.Lib
{


	/// <summary>
	/// The feature list is sorted according to the compareTo of the node.
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class FeatureList
	{
		private const long serialVersionUID = 7526471155622776147L;
		private readonly List<MaltFeatureNode> list;

		/// <summary>
		/// Creates a feature list of MaltFeatureNode objects
		/// </summary>
		public FeatureList()
		{
			list = new List<MaltFeatureNode>();
		}

		/// <summary>
		/// Creates a feature list of MaltFeatureNode objects
		/// </summary>
		public FeatureList(int size)
		{
			list = new List<MaltFeatureNode>(size);
		}

		/// <summary>
		/// Adds a MaltFeatureNode object to the feature list. The object will be added in the sorted feature list based on 
		/// the compareTo() in MaltFeatureNode.
		/// </summary>
		/// <param name="x"> a MaltFeatureNode object </param>
		public virtual void add(MaltFeatureNode x)
		{
			if (list.Count == 0 || list[list.Count - 1].CompareTo(x) <= 0)
			{
				list.Add(x);
			}
			else
			{
				int low = 0;
				int high = list.Count - 1;
				int mid;
				MaltFeatureNode y;
				while (low <= high)
				{
					mid = (low + high) / 2;
					y = list[mid];
					if (y.CompareTo(x) < 0)
					{
						low = mid + 1;
					}
					else if (y.CompareTo(x) > 0)
					{
						high = mid - 1;
					}
					else
					{
						break;
					}
				}
				list.Insert(low,x);
			}
		}

		/// <summary>
		/// Adds an index/value pair to the feature list.
		/// </summary>
		/// <param name="index"> a binarized feature index </param>
		/// <param name="value"> a value  </param>
		public virtual void add(int index, double value)
		{
			add(new MaltFeatureNode(index,value));
		}

		/// <param name="i"> the position in the feature list </param>
		/// <returns> a MaltFeatureNode object located on the position <i>i</i> </returns>
		public virtual MaltFeatureNode get(int i)
		{
			if (i < 0 || i >= list.Count)
			{
				return null;
			}
			return list[i];
		}

		/// <summary>
		/// Clears the feature list
		/// </summary>
		public virtual void clear()
		{
			list.Clear();
		}

		/// <returns> the size of the feature list </returns>
		public virtual int size()
		{
			return list.Count;
		}

		public virtual MaltFeatureNode[] toArray()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MaltFeatureNode[] nodes = new MaltFeatureNode[list.size()];
			MaltFeatureNode[] nodes = new MaltFeatureNode[list.Count];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = nodes.length;
			int len = nodes.Length;

			for (int i = 0; i < len; i++)
			{
				nodes[i] = list[i];

			}
			return nodes;
		}
	}

}