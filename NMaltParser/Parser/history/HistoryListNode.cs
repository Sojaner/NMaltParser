using System.Text;

namespace org.maltparser.parser.history
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using GuideUserAction = org.maltparser.parser.history.action.GuideUserAction;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class HistoryListNode : HistoryNode
	{
		private HistoryNode previousNode;
		private GuideUserAction action;
		private int position;

		public HistoryListNode(HistoryNode _previousNode, GuideUserAction _action)
		{
			this.previousNode = _previousNode;
			this.action = _action;
			if (previousNode != null)
			{
				this.position = previousNode.Position + 1;
			}
			else
			{
				this.position = 1;
			}
		}

		public virtual HistoryNode PreviousNode
		{
			get
			{
				return previousNode;
			}
			set
			{
				this.previousNode = value;
			}
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



		public virtual int Position
		{
			get
			{
				return position;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public virtual void clear()
		{
			this.previousNode = null;
			this.action = null;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(action);
			return sb.ToString();
		}

	}

}