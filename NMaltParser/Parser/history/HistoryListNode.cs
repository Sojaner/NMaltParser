using System.Text;

namespace org.maltparser.parser.history
{
    using  action;
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
			previousNode = _previousNode;
			action = _action;
			if (previousNode != null)
			{
				position = previousNode.Position + 1;
			}
			else
			{
				position = 1;
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
				previousNode = value;
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
				action = value;
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
			previousNode = null;
			action = null;
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