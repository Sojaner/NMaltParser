using System.Collections.Generic;
using System.Text;

namespace org.maltparser.parser.history
{

	using  org.maltparser.core.exception;
	using  org.maltparser.parser.history.action;
	using  org.maltparser.parser.history.action;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class HistoryTreeNode : HistoryNode
	{
		private GuideUserAction action;
		private HistoryTreeNode parent;
		private int depth;
		private List<HistoryTreeNode> children;

		public HistoryTreeNode(HistoryNode previousNode, GuideUserAction action)
		{
			PreviousNode = parent;
			Action = action;
			children = new List<HistoryTreeNode>();
		}

		public virtual GuideUserAction Action
		{
			get
			{
				return action;
			}
			set
			{
				this.action = value;
			}
		}


		public virtual HistoryNode PreviousNode
		{
			get
			{
				return parent;
			}
			set
			{
				if (value is HistoryTreeNode)
				{
					this.parent = (HistoryTreeNode)value;
					parent.addChild(this);
					Depth = parent.Depth + 1;
				}
			}
		}


		public virtual int Depth
		{
			get
			{
				return depth;
			}
			set
			{
				this.depth = value;
			}
		}


		public virtual void addChild(HistoryTreeNode child)
		{
			children.Add(child);
		}

		public virtual void removeChild(HistoryTreeNode child)
		{
			children.Remove(child);
		}

		public virtual HistoryTreeNode getChild(ActionDecision childDecision)
		{
			foreach (HistoryTreeNode c in children)
			{
				if (c.Action.Equals(childDecision))
				{
					return c;
				}
			}
			return null;
		}

		public virtual int Position
		{
			get
			{
				return depth;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public virtual void clear()
		{
			if (parent != null)
			{
				parent.removeChild(this);
			}
			Action = null;
			PreviousNode = null;
			children.Clear();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i <= depth; i++)
			{
				sb.Append("  ");
			}
			sb.Append(action);
			sb.Append('\n');
			for (int i = 0; i < children.Count; i++)
			{
				sb.Append(children[i]);
			}
			return sb.ToString();
		}

	}

}