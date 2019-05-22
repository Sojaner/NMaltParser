using System.Text;

namespace org.maltparser.core.propagation.spec
{
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class PropagationSpec
	{
		public const long serialVersionUID = 1L;
		private readonly string from;
		private readonly string to;
		private readonly string _for; // for
		private readonly string over;

		public PropagationSpec(string from, string to, string _for, string over)
		{
			this.from = from;
			this.to = to;
			this._for = _for;
			this.over = over;
		}

		public virtual string From
		{
			get
			{
				return from;
			}
		}


		public virtual string To
		{
			get
			{
				return to;
			}
		}


		public virtual string For
		{
			get
			{
				return _for;
			}
		}


		public virtual string Over
		{
			get
			{
				return over;
			}
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((string.ReferenceEquals(_for, null)) ? 0 : _for.GetHashCode());
			result = prime * result + ((string.ReferenceEquals(from, null)) ? 0 : from.GetHashCode());
			result = prime * result + ((string.ReferenceEquals(over, null)) ? 0 : over.GetHashCode());
			result = prime * result + ((string.ReferenceEquals(to, null)) ? 0 : to.GetHashCode());
			return result;
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
			PropagationSpec other = (PropagationSpec) obj;
			if (string.ReferenceEquals(_for, null))
			{
				if (!string.ReferenceEquals(other._for, null))
				{
					return false;
				}
			}
			else if (!_for.Equals(other._for))
			{
				return false;
			}
			if (string.ReferenceEquals(from, null))
			{
				if (!string.ReferenceEquals(other.from, null))
				{
					return false;
				}
			}
			else if (!from.Equals(other.from))
			{
				return false;
			}
			if (string.ReferenceEquals(over, null))
			{
				if (!string.ReferenceEquals(other.over, null))
				{
					return false;
				}
			}
			else if (!over.Equals(other.over))
			{
				return false;
			}
			if (string.ReferenceEquals(to, null))
			{
				if (!string.ReferenceEquals(other.to, null))
				{
					return false;
				}
			}
			else if (!to.Equals(other.to))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("FROM: ");
			sb.Append(from);
			sb.Append("\n");
			sb.Append("TO  : ");
			sb.Append(to);
			sb.Append("\n");
			sb.Append("FOR : ");
			sb.Append(_for);
			sb.Append("\n");
			sb.Append("OVER: ");
			sb.Append(over);
			sb.Append("\n");
			return sb.ToString();
		}
	}

}