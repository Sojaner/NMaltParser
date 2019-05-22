using System.Text;

namespace org.maltparser.core.pool
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using HashSet = org.maltparser.core.helper.HashSet;


	public abstract class ObjectPoolSet<T> : ObjectPool<T>
	{
		private readonly HashSet<T> available;
		private readonly HashSet<T> inuse;

		public ObjectPoolSet() : this(int.MaxValue)
		{
		}

		public ObjectPoolSet(int keepThreshold) : base(keepThreshold)
		{
			available = new HashSet<T>();
			inuse = new HashSet<T>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract T create() throws org.maltparser.core.exception.MaltChainedException;
		protected internal override abstract T create();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void resetObject(T o) throws org.maltparser.core.exception.MaltChainedException;
		public override abstract void resetObject(T o);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized T checkOut() throws org.maltparser.core.exception.MaltChainedException
		public override T checkOut()
		{
			lock (this)
			{
				if (available.Empty)
				{
					T t = create();
					inuse.add(t);
					return t;
				}
				else
				{
					foreach (T t in available)
					{
						inuse.add(t);
						available.remove(t);
						return t;
					}
				}
				return default(T);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void checkIn(T t) throws org.maltparser.core.exception.MaltChainedException
		public override void checkIn(T t)
		{
			lock (this)
			{
				resetObject(t);
				inuse.remove(t);
				if (available.size() < keepThreshold)
				{
					available.add(t);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void checkInAll() throws org.maltparser.core.exception.MaltChainedException
		public override void checkInAll()
		{
			lock (this)
			{
				foreach (T t in inuse)
				{
					resetObject(t);
					if (available.size() < keepThreshold)
					{
						available.add(t);
					}
				}
				inuse.clear();
			}
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			foreach (T t in inuse)
			{
				sb.Append(t);
				sb.Append(", ");
			}
			return sb.ToString();
		}
	}

}