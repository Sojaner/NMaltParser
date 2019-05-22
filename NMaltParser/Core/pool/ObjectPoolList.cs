using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.pool
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;

	public abstract class ObjectPoolList<T> : ObjectPool<T>
	{
		private readonly List<T> objectList;
		private int currentSize;

		public ObjectPoolList() : this(int.MaxValue)
		{
		}

		public ObjectPoolList(int keepThreshold) : base(keepThreshold)
		{
			objectList = new List<T>();
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
				T t = default(T);
				if (currentSize >= objectList.Count)
				{
					t = create();
					objectList.Add(t);
					currentSize++;
				}
				else
				{
					t = objectList[currentSize];
					currentSize++;
				}
				return t;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void checkIn(T o) throws org.maltparser.core.exception.MaltChainedException
		public override void checkIn(T o)
		{
			lock (this)
			{
				resetObject(o);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void checkInAll() throws org.maltparser.core.exception.MaltChainedException
		public override void checkInAll()
		{
			lock (this)
			{
				for (int i = currentSize-1; i >= 0 && i < objectList.Count; i--)
				{
					resetObject(objectList[i]);
					if (currentSize >= keepThreshold)
					{
						objectList.RemoveAt(i);
					}
				}
				currentSize = 0;
			}
		}

		public virtual int CurrentSize
		{
			get
			{
				return currentSize;
			}
			set
			{
				this.currentSize = value;
			}
		}


		public virtual int size()
		{
			return objectList.Count;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < currentSize; i++)
			{
				sb.Append(objectList[i]);
				sb.Append(", ");
			}
			return sb.ToString();
		}
	}

}