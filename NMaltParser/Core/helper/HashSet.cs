using System;
using System.Collections.Generic;

namespace org.maltparser.core.helper
{
	/*
	 * Copyright 2009 Google Inc.
	 * 
	 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
	 * use this file except in compliance with the License. You may obtain a copy of
	 * the License at
	 * 
	 * http://www.apache.org/licenses/LICENSE-2.0
	 * 
	 * Unless required by applicable law or agreed to in writing, software
	 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
	 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
	 * License for the specific language governing permissions and limitations under
	 * the License.
	 */

	/// <summary>
	/// A memory-efficient hash set.
	/// </summary>
	/// @param <E> the element type </param>
	[Serializable]
	public class HashSet<E> : AbstractSet<E>
	{
	  private const long serialVersionUID = 7526471155622776147L;
	  private class SetIterator : IEnumerator<E>
	  {
		  private readonly HashSet<E> outerInstance;

		internal int index = 0;
		internal int last = -1;

		public SetIterator(HashSet<E> outerInstance)
		{
			this.outerInstance = outerInstance;
		  advanceToItem();
		}

		public virtual bool hasNext()
		{
		  return index < outerInstance.table.Length;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E next()
		public virtual E next()
		{
		  if (!hasNext())
		  {
			throw new NoSuchElementException();
		  }
		  last = index;
		  E toReturn = (E) unmaskNull(outerInstance.table[index++]);
		  advanceToItem();
		  return toReturn;
		}

		public virtual void remove()
		{
		  if (last < 0)
		  {
			throw new InvalidOperationException();
		  }
		  outerInstance.internalRemove(last);
		  if (outerInstance.table[last] != null)
		  {
			index = last;
		  }
		  last = -1;
		}

		internal virtual void advanceToItem()
		{
		  for (; index < outerInstance.table.Length; ++index)
		  {
			if (outerInstance.table[index] != null)
			{
			  return;
			}
		  }
		}
	  }

	  /// <summary>
	  /// In the interest of memory-savings, we start with the smallest feasible
	  /// power-of-two table size that can hold three items without rehashing. If we
	  /// started with a size of 2, we'd have to expand as soon as the second item
	  /// was added.
	  /// </summary>
	  private const int INITIAL_TABLE_SIZE = 4;

	  private static readonly object NULL_ITEM = new SerializableAnonymousInnerClass();

	  private class SerializableAnonymousInnerClass : Serializable
	  {
		  internal object readResolve()
		  {
			return NULL_ITEM;
		  }
	  }

	  internal static object maskNull(object o)
	  {
		return (o == null) ? NULL_ITEM : o;
	  }

	  internal static object unmaskNull(object o)
	  {
		return (o == NULL_ITEM) ? null : o;
	  }

	  /// <summary>
	  /// Number of objects in this set; transient due to custom serialization.
	  /// Default access to avoid synthetic accessors from inner classes.
	  /// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  [NonSerialized]
	  internal int size_Renamed = 0;

	  /// <summary>
	  /// Backing store for all the objects; transient due to custom serialization.
	  /// Default access to avoid synthetic accessors from inner classes.
	  /// </summary>
	  [NonSerialized]
	  internal object[] table;

	  public HashSet()
	  {
		table = new object[INITIAL_TABLE_SIZE];
	  }

	  public HashSet<T1>(ICollection<T1> c) where T1 : E
	  {
		int newCapacity = INITIAL_TABLE_SIZE;
		int expectedSize = c.Count;
		while (newCapacity * 3 < expectedSize * 4)
		{
		  newCapacity <<= 1;
		}

		table = new object[newCapacity];
		base.addAll(c);
	  }

	  public override bool add(E e)
	  {
		ensureSizeFor(size_Renamed + 1);
		int index = findOrEmpty(e);
		if (table[index] == null)
		{
		  ++size_Renamed;
		  table[index] = maskNull(e);
		  return true;
		}
		return false;
	  }

	  public override bool addAll<T1>(ICollection<T1> c) where T1 : E
	  {
		ensureSizeFor(size_Renamed + c.Count);
		return base.addAll(c);
	  }

	  public override void clear()
	  {
		table = new object[INITIAL_TABLE_SIZE];
		size_Renamed = 0;
	  }

	  public override bool contains(object o)
	  {
		return find(o) >= 0;
	  }

	  public override IEnumerator<E> iterator()
	  {
		return new SetIterator(this);
	  }

	  public override bool remove(object o)
	  {
		int index = find(o);
		if (index < 0)
		{
		  return false;
		}
		internalRemove(index);
		return true;
	  }

	  public override int size()
	  {
		return size_Renamed;
	  }

	  public override object[] toArray()
	  {
		return toArray(new object[size_Renamed]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <T> T[] toArray(T[] a)
	  public override T[] toArray<T>(T[] a)
	  {
		if (a.Length < size_Renamed)
		{
		  a = (T[]) Array.CreateInstance(a.GetType().GetElementType(), size_Renamed);
		}
		int index = 0;
		for (int i = 0; i < table.Length; ++i)
		{
		  object e = table[i];
		  if (e != null)
		  {
			a[index++] = (T) unmaskNull(e);
		  }
		}
		while (index < a.Length)
		{
		  a[index++] = null;
		}
		return a;
	  }

	  /// <summary>
	  /// Adapted from org.apache.commons.collections.map.AbstractHashedMap.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void doReadObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  protected internal virtual void doReadObject(ObjectInputStream @in)
	  {
		table = new object[@in.readInt()];
		int items = @in.readInt();
		for (int i = 0; i < items; i++)
		{
		  add((E) @in.readObject());
		}
	  }

	  /// <summary>
	  /// Adapted from org.apache.commons.collections.map.AbstractHashedMap.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void doWriteObject(java.io.ObjectOutputStream out) throws java.io.IOException
	  protected internal virtual void doWriteObject(ObjectOutputStream @out)
	  {
		@out.writeInt(table.Length);
		@out.writeInt(size_Renamed);
		for (int i = 0; i < table.Length; ++i)
		{
		  object e = table[i];
		  if (e != null)
		  {
			@out.writeObject(unmaskNull(e));
		  }
		}
	  }

	  /// <summary>
	  /// Returns whether two items are equal for the purposes of this set.
	  /// </summary>
	  protected internal virtual bool itemEquals(object a, object b)
	  {
		return (a == null) ? (b == null) : a.Equals(b);
	  }

	  /// <summary>
	  /// Return the hashCode for an item.
	  /// </summary>
	  protected internal virtual int itemHashCode(object o)
	  {
		return (o == null) ? 0 : o.GetHashCode();
	  }

	  /// <summary>
	  /// Works just like <seealso cref="#addAll(Collection)"/>, but for arrays. Used to avoid
	  /// having to synthesize a collection in <seealso cref="Sets"/>.
	  /// </summary>
	  internal virtual void addAll(E[] elements)
	  {
		ensureSizeFor(size_Renamed + elements.Length);
		foreach (E e in elements)
		{
		  int index = findOrEmpty(e);
		  if (table[index] == null)
		  {
			++size_Renamed;
			table[index] = maskNull(e);
		  }
		}
	  }

	  /// <summary>
	  /// Removes the item at the specified index, and performs internal management
	  /// to make sure we don't wind up with a hole in the table. Default access to
	  /// avoid synthetic accessors from inner classes.
	  /// </summary>
	  internal virtual void internalRemove(int index)
	  {
		table[index] = null;
		--size_Renamed;
		plugHole(index);
	  }

	  /// <summary>
	  /// Ensures the set is large enough to contain the specified number of entries.
	  /// </summary>
	  private void ensureSizeFor(int expectedSize)
	  {
		if (table.Length * 3 >= expectedSize * 4)
		{
		  return;
		}

		int newCapacity = table.Length << 1;
		while (newCapacity * 3 < expectedSize * 4)
		{
		  newCapacity <<= 1;
		}

		object[] oldTable = table;
		table = new object[newCapacity];
		foreach (object o in oldTable)
		{
		  if (o != null)
		  {
			int newIndex = getIndex(unmaskNull(o));
			while (table[newIndex] != null)
			{
			  if (++newIndex == table.Length)
			  {
				newIndex = 0;
			  }
			}
			table[newIndex] = o;
		  }
		}
	  }

	  /// <summary>
	  /// Returns the index in the table at which a particular item resides, or -1 if
	  /// the item is not in the table.
	  /// </summary>
	  private int find(object o)
	  {
		int index = getIndex(o);
		while (true)
		{
		  object existing = table[index];
		  if (existing == null)
		  {
			return -1;
		  }
		  if (itemEquals(o, unmaskNull(existing)))
		  {
			return index;
		  }
		  if (++index == table.Length)
		  {
			index = 0;
		  }
		}
	  }

	  /// <summary>
	  /// Returns the index in the table at which a particular item resides, or the
	  /// index of an empty slot in the table where this item should be inserted if
	  /// it is not already in the table.
	  /// </summary>
	  private int findOrEmpty(object o)
	  {
		int index = getIndex(o);
		while (true)
		{
		  object existing = table[index];
		  if (existing == null)
		  {
			return index;
		  }
		  if (itemEquals(o, unmaskNull(existing)))
		  {
			return index;
		  }
		  if (++index == table.Length)
		  {
			index = 0;
		  }
		}
	  }

	  private int getIndex(object o)
	  {
		int h = itemHashCode(o);
		// Copied from Apache's AbstractHashedMap; prevents power-of-two collisions.
		h += ~(h << 9);
		h ^= ((int)((uint)h >> 14));
		h += (h << 4);
		h ^= ((int)((uint)h >> 10));
		// Power of two trick.
		return h & (table.Length - 1);
	  }

	  /// <summary>
	  /// Tricky, we left a hole in the map, which we have to fill. The only way to
	  /// do this is to search forwards through the map shuffling back values that
	  /// match this index until we hit a null.
	  /// </summary>
	  private void plugHole(int hole)
	  {
		int index = hole + 1;
		if (index == table.Length)
		{
		  index = 0;
		}
		while (table[index] != null)
		{
		  int targetIndex = getIndex(unmaskNull(table[index]));
		  if (hole < index)
		  {
			/*
			 * "Normal" case, the index is past the hole and the "bad range" is from
			 * hole (exclusive) to index (inclusive).
			 */
			if (!(hole < targetIndex && targetIndex <= index))
			{
			  // Plug it!
			  table[hole] = table[index];
			  table[index] = null;
			  hole = index;
			}
		  }
		  else
		  {
			/*
			 * "Wrapped" case, the index is before the hole (we've wrapped) and the
			 * "good range" is from index (exclusive) to hole (inclusive).
			 */
			if (index < targetIndex && targetIndex <= hole)
			{
			  // Plug it!
			  table[hole] = table[index];
			  table[index] = null;
			  hole = index;
			}
		  }
		  if (++index == table.Length)
		  {
			index = 0;
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
	  private void readObject(ObjectInputStream @in)
	  {
		@in.defaultReadObject();
		doReadObject(@in);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException
	  private void writeObject(ObjectOutputStream @out)
	  {
		@out.defaultWriteObject();
		doWriteObject(@out);
	  }
	}

}