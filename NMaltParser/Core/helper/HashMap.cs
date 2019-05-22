using System;
using System.Collections.Generic;
using System.Text;

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
	/// A memory-efficient hash map.
	/// </summary>
	/// @param <K> the key type </param>
	/// @param <V> the value type </param>
	[Serializable]
	public class HashMap<K, V> : IDictionary<K, V>
	{
		private const long serialVersionUID = 7526471155622776147L;
	  /// <summary>
	  /// In the interest of memory-savings, we start with the smallest feasible
	  /// power-of-two table size that can hold three items without rehashing. If we
	  /// started with a size of 2, we'd have to expand as soon as the second item
	  /// was added.
	  /// </summary>
	  private const int INITIAL_TABLE_SIZE = 4;

	  private class EntryIterator : IEnumerator<Entry<K, V>>
	  {
		  internal bool InstanceFieldsInitialized = false;

		  internal virtual void InitializeInstanceFields()
		  {
			  advanceToItem();
		  }

		  private readonly HashMap<K, V> outerInstance;

		  public EntryIterator(HashMap<K, V> outerInstance)
		  {
			  this.outerInstance = outerInstance;

			  if (!InstanceFieldsInitialized)
			  {
				  InitializeInstanceFields();
				  InstanceFieldsInitialized = true;
			  }
		  }

		internal int index = 0;
		internal int last = -1;

		public virtual bool hasNext()
		{
		  return index < outerInstance.keys.Length;
		}

		public virtual Entry<K, V> next()
		{
		  if (!hasNext())
		  {
			throw new NoSuchElementException();
		  }
		  last = index;
		  Entry<K, V> toReturn = new HashEntry(outerInstance, index++);
		  advanceToItem();
		  return toReturn;
		}

		public virtual void remove()
		{
		  if (last < 0)
		  {
			throw new System.InvalidOperationException();
		  }
		  outerInstance.internalRemove(last);
		  if (outerInstance.keys[last] != null)
		  {
			index = last;
		  }
		  last = -1;
		}

		internal virtual void advanceToItem()
		{
		  for (; index < outerInstance.keys.Length; ++index)
		  {
			if (outerInstance.keys[index] != null)
			{
			  return;
			}
		  }
		}
	  }

	  private class EntrySet : AbstractSet<Entry<K, V>>
	  {
		  private readonly HashMap<K, V> outerInstance;

		  public EntrySet(HashMap<K, V> outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		public override bool add(Entry<K, V> entry)
		{
		  bool result = !outerInstance.ContainsKey(entry.Key);
		  outerInstance[entry.Key] = entry.Value;
		  return result;
		}

		public override bool addAll<T1>(ICollection<T1> c) where T1 : Entry<K, V>
		{
		  outerInstance.ensureSizeFor(size() + c.Count);
		  return base.addAll(c);
		}

		public override void clear()
		{
		  outerInstance.Clear();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public boolean contains(Object o)
		public override bool contains(object o)
		{
		  if (!(o is Entry))
		  {
			return false;
		  }
		  Entry<K, V> entry = (Entry<K, V>) o;
		  V value = outerInstance[entry.Key];
		  return outerInstance.valueEquals(value, entry.Value);
		}

		public override int GetHashCode()
		{
		  return outerInstance.GetHashCode();
		}

		public override IEnumerator<KeyValuePair<K, V>> iterator()
		{
		  return new EntryIterator(outerInstance);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public boolean remove(Object o)
		public override bool remove(object o)
		{
		  if (!(o is Entry))
		  {
			return false;
		  }
		  Entry<K, V> entry = (Entry<K, V>) o;
		  int index = outerInstance.findKey(entry.Key);
		  if (index >= 0 && outerInstance.valueEquals(outerInstance.values_Renamed[index], entry.Value))
		  {
			outerInstance.internalRemove(index);
			return true;
		  }
		  return false;
		}

		public override bool removeAll<T1>(ICollection<T1> c)
		{
		  bool didRemove = false;
		  foreach (object o in c)
		  {
			didRemove |= remove(o);
		  }
		  return didRemove;
		}

		public override int size()
		{
		  return outerInstance.size_Renamed;
		}
	  }

	  private class HashEntry : Entry<K, V>
	  {
		  private readonly HashMap<K, V> outerInstance;

		internal readonly int index;

		public HashEntry(HashMap<K, V> outerInstance, int index)
		{
			this.outerInstance = outerInstance;
		  this.index = index;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public boolean equals(Object o)
		public override bool Equals(object o)
		{
		  if (!(o is Entry))
		  {
			return false;
		  }
		  Entry<K, V> entry = (Entry<K, V>) o;
		  return outerInstance.keyEquals(Key, entry.Key) && outerInstance.valueEquals(Value, entry.Value);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public K getKey()
		public virtual K Key
		{
			get
			{
			  return (K) unmaskNullKey(outerInstance.keys[index]);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V getValue()
		public virtual V Value
		{
			get
			{
			  return (V) outerInstance.values_Renamed[index];
			}
		}

		public override int GetHashCode()
		{
		  return outerInstance.keyHashCode(Key) ^ outerInstance.valueHashCode(Value);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V setValue(V value)
		public virtual V setValue(V value)
		{
		  V previous = (V) outerInstance.values_Renamed[index];
		  outerInstance.values_Renamed[index] = value;
		  return previous;
		}

		public override string ToString()
		{
		  return Key + "=" + Value;
		}
	  }

	  private class KeyIterator : IEnumerator<K>
	  {
		  internal bool InstanceFieldsInitialized = false;

		  internal virtual void InitializeInstanceFields()
		  {
			  advanceToItem();
		  }

		  private readonly HashMap<K, V> outerInstance;

		  public KeyIterator(HashMap<K, V> outerInstance)
		  {
			  this.outerInstance = outerInstance;

			  if (!InstanceFieldsInitialized)
			  {
				  InitializeInstanceFields();
				  InstanceFieldsInitialized = true;
			  }
		  }

		internal int index = 0;
		internal int last = -1;

		public virtual bool hasNext()
		{
		  return index < outerInstance.keys.Length;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public K next()
		public virtual K next()
		{
		  if (!hasNext())
		  {
			throw new NoSuchElementException();
		  }
		  last = index;
		  object toReturn = unmaskNullKey(outerInstance.keys[index++]);
		  advanceToItem();
		  return (K) toReturn;
		}

		public virtual void remove()
		{
		  if (last < 0)
		  {
			throw new System.InvalidOperationException();
		  }
		  outerInstance.internalRemove(last);
		  if (outerInstance.keys[last] != null)
		  {
			index = last;
		  }
		  last = -1;
		}

		internal virtual void advanceToItem()
		{
		  for (; index < outerInstance.keys.Length; ++index)
		  {
			if (outerInstance.keys[index] != null)
			{
			  return;
			}
		  }
		}
	  }

	  private class KeySet : AbstractSet<K>
	  {
		  private readonly HashMap<K, V> outerInstance;

		  public KeySet(HashMap<K, V> outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		public override void clear()
		{
		  outerInstance.Clear();
		}

		public override bool contains(object o)
		{
		  return outerInstance.ContainsKey(o);
		}

		public override int GetHashCode()
		{
		  int result = 0;
		  for (int i = 0; i < outerInstance.keys.Length; ++i)
		  {
			object key = outerInstance.keys[i];
			if (key != null)
			{
			  result += outerInstance.keyHashCode(unmaskNullKey(key));
			}
		  }
		  return result;
		}

		public override IEnumerator<K> iterator()
		{
		  return new KeyIterator(outerInstance);
		}

		public override bool remove(object o)
		{
		  int index = outerInstance.findKey(o);
		  if (index >= 0)
		  {
			outerInstance.internalRemove(index);
			return true;
		  }
		  return false;
		}

		public override bool removeAll<T1>(ICollection<T1> c)
		{
		  bool didRemove = false;
		  foreach (object o in c)
		  {
			didRemove |= remove(o);
		  }
		  return didRemove;
		}

		public override int size()
		{
		  return outerInstance.size_Renamed;
		}
	  }

	  private class ValueIterator : IEnumerator<V>
	  {
		  internal bool InstanceFieldsInitialized = false;

		  internal virtual void InitializeInstanceFields()
		  {
			  advanceToItem();
		  }

		  private readonly HashMap<K, V> outerInstance;

		  public ValueIterator(HashMap<K, V> outerInstance)
		  {
			  this.outerInstance = outerInstance;

			  if (!InstanceFieldsInitialized)
			  {
				  InitializeInstanceFields();
				  InstanceFieldsInitialized = true;
			  }
		  }

		internal int index = 0;
		internal int last = -1;

		public virtual bool hasNext()
		{
		  return index < outerInstance.keys.Length;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V next()
		public virtual V next()
		{
		  if (!hasNext())
		  {
			throw new NoSuchElementException();
		  }
		  last = index;
		  object toReturn = outerInstance.values_Renamed[index++];
		  advanceToItem();
		  return (V) toReturn;
		}

		public virtual void remove()
		{
		  if (last < 0)
		  {
			throw new System.InvalidOperationException();
		  }
		  outerInstance.internalRemove(last);
		  if (outerInstance.keys[last] != null)
		  {
			index = last;
		  }
		  last = -1;
		}

		internal virtual void advanceToItem()
		{
		  for (; index < outerInstance.keys.Length; ++index)
		  {
			if (outerInstance.keys[index] != null)
			{
			  return;
			}
		  }
		}
	  }

	  private class Values : AbstractCollection<V>
	  {
		  private readonly HashMap<K, V> outerInstance;

		  public Values(HashMap<K, V> outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		public override void clear()
		{
		  outerInstance.Clear();
		}

		public override bool contains(object o)
		{
		  return outerInstance.ContainsValue(o);
		}

		public override int GetHashCode()
		{
		  int result = 0;
		  for (int i = 0; i < outerInstance.keys.Length; ++i)
		  {
			if (outerInstance.keys[i] != null)
			{
			  result += outerInstance.valueHashCode(outerInstance.values_Renamed[i]);
			}
		  }
		  return result;
		}

		public override IEnumerator<V> iterator()
		{
		  return new ValueIterator(outerInstance);
		}

		public override bool remove(object o)
		{
		  if (o == null)
		  {
			for (int i = 0; i < outerInstance.keys.Length; ++i)
			{
			  if (outerInstance.keys[i] != null && outerInstance.values_Renamed[i] == null)
			  {
				outerInstance.internalRemove(i);
				return true;
			  }
			}
		  }
		  else
		  {
			for (int i = 0; i < outerInstance.keys.Length; ++i)
			{
			  if (outerInstance.valueEquals(outerInstance.values_Renamed[i], o))
			  {
				outerInstance.internalRemove(i);
				return true;
			  }
			}
		  }
		  return false;
		}

		public override bool removeAll<T1>(ICollection<T1> c)
		{
		  bool didRemove = false;
		  foreach (object o in c)
		  {
			didRemove |= remove(o);
		  }
		  return didRemove;
		}

		public override int size()
		{
		  return outerInstance.size_Renamed;
		}
	  }

	  private static readonly object NULL_KEY = new SerializableAnonymousInnerClass();

	  private class SerializableAnonymousInnerClass : Serializable
	  {
		  internal object readResolve()
		  {
			return NULL_KEY;
		  }
	  }

	  internal static object maskNullKey(object k)
	  {
		return (k == null) ? NULL_KEY : k;
	  }

	  internal static object unmaskNullKey(object k)
	  {
		return (k == NULL_KEY) ? null : k;
	  }

	  /// <summary>
	  /// Backing store for all the keys; transient due to custom serialization.
	  /// Default access to avoid synthetic accessors from inner classes.
	  /// </summary>
	  [NonSerialized]
	  internal object[] keys;

	  /// <summary>
	  /// Number of pairs in this set; transient due to custom serialization. Default
	  /// access to avoid synthetic accessors from inner classes.
	  /// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  [NonSerialized]
	  internal int size_Renamed = 0;

	  /// <summary>
	  /// Backing store for all the values; transient due to custom serialization.
	  /// Default access to avoid synthetic accessors from inner classes.
	  /// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  [NonSerialized]
	  internal object[] values_Renamed;

	  public HashMap()
	  {
		initTable(INITIAL_TABLE_SIZE);
	  }

	  public HashMap<T1>(IDictionary<T1> m) where T1 : K
	  {
		int newCapacity = INITIAL_TABLE_SIZE;
		int expectedSize = m.Count;
		while (newCapacity * 3 < expectedSize * 4)
		{
		  newCapacity <<= 1;
		}

		initTable(newCapacity);
		internalPutAll(m);
	  }

	  public virtual void Clear()
	  {
		initTable(INITIAL_TABLE_SIZE);
		size_Renamed = 0;
	  }

	  public virtual bool ContainsKey(object key)
	  {
		return findKey(key) >= 0;
	  }

	  public virtual bool containsValue(object value)
	  {
		if (value == null)
		{
		  for (int i = 0; i < keys.Length; ++i)
		  {
			if (keys[i] != null && values_Renamed[i] == null)
			{
			  return true;
			}
		  }
		}
		else
		{
		  foreach (object existing in values_Renamed)
		  {
			if (valueEquals(existing, value))
			{
			  return true;
			}
		  }
		}
		return false;
	  }

	  public virtual ISet<Entry<K, V>> entrySet()
	  {
		return new EntrySet(this);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public boolean equals(Object o)
	  public override bool Equals(object o)
	  {
		if (!(o is System.Collections.IDictionary))
		{
		  return false;
		}
		IDictionary<K, V> other = (IDictionary<K, V>) o;
		return entrySet().SetEquals(other.SetOfKeyValuePairs());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V get(Object key)
	  public virtual V get(object key)
	  {
		int index = findKey(key);
		return (index < 0) ? default(V) : (V) values_Renamed[index];
	  }

	  public override int GetHashCode()
	  {
		int result = 0;
		for (int i = 0; i < keys.Length; ++i)
		{
		  object key = keys[i];
		  if (key != null)
		  {
			result += keyHashCode(unmaskNullKey(key)) ^ valueHashCode(values_Renamed[i]);
		  }
		}
		return result;
	  }

	  public virtual bool Empty
	  {
		  get
		  {
			return size_Renamed == 0;
		  }
	  }

	  public virtual ISet<K> keySet()
	  {
		return new KeySet(this);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V put(K key, V value)
	  public virtual V put(K key, V value)
	  {
		ensureSizeFor(size_Renamed + 1);
		int index = findKeyOrEmpty(key);
		if (keys[index] == null)
		{
		  ++size_Renamed;
		  keys[index] = maskNullKey(key);
		  values_Renamed[index] = value;
		  return default(V);
		}
		else
		{
		  object previousValue = values_Renamed[index];
		  values_Renamed[index] = value;
		  return (V) previousValue;
		}
	  }

	  public virtual void putAll<T1>(IDictionary<T1> m) where T1 : K
	  {
		ensureSizeFor(size_Renamed + m.Count);
		internalPutAll(m);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V remove(Object key)
	  public virtual V remove(object key)
	  {
		int index = findKey(key);
		if (index < 0)
		{
		  return default(V);
		}
		object previousValue = values_Renamed[index];
		internalRemove(index);
		return (V) previousValue;
	  }

	  public virtual int Count
	  {
		  get
		  {
			return size_Renamed;
		  }
	  }

	  public override string ToString()
	  {
		if (size_Renamed == 0)
		{
		  return "{}";
		}
		StringBuilder buf = new StringBuilder(32 * size());
		buf.Append('{');

		bool needComma = false;
		for (int i = 0; i < keys.Length; ++i)
		{
		  object key = keys[i];
		  if (key != null)
		  {
			if (needComma)
			{
			  buf.Append(',').Append(' ');
			}
			key = unmaskNullKey(key);
			object value = values_Renamed[i];
			buf.Append(key == this ? "(this Map)" : key).Append('=').Append(value == this ? "(this Map)" : value);
			needComma = true;
		  }
		}
		buf.Append('}');
		return buf.ToString();
	  }

	  public virtual ICollection<V> Values
	  {
		  get
		  {
			return new Values(this);
		  }
	  }

	  /// <summary>
	  /// Adapted from org.apache.commons.collections.map.AbstractHashedMap.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void doReadObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  protected internal virtual void doReadObject(ObjectInputStream @in)
	  {
		int capacity = @in.readInt();
		initTable(capacity);
		int items = @in.readInt();
		for (int i = 0; i < items; i++)
		{
		  object key = @in.readObject();
		  object value = @in.readObject();
		  put((K) key, (V) value);
		}
	  }

	  /// <summary>
	  /// Adapted from org.apache.commons.collections.map.AbstractHashedMap.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void doWriteObject(java.io.ObjectOutputStream out) throws java.io.IOException
	  protected internal virtual void doWriteObject(ObjectOutputStream @out)
	  {
		@out.writeInt(keys.Length);
		@out.writeInt(size_Renamed);
		for (int i = 0; i < keys.Length; ++i)
		{
		  object key = keys[i];
		  if (key != null)
		  {
			@out.writeObject(unmaskNullKey(key));
			@out.writeObject(values_Renamed[i]);
		  }
		}
	  }

	  /// <summary>
	  /// Returns whether two keys are equal for the purposes of this set.
	  /// </summary>
	  protected internal virtual bool keyEquals(object a, object b)
	  {
		return (a == null) ? (b == null) : a.Equals(b);
	  }

	  /// <summary>
	  /// Returns the hashCode for a key.
	  /// </summary>
	  protected internal virtual int keyHashCode(object k)
	  {
		return (k == null) ? 0 : k.GetHashCode();
	  }

	  /// <summary>
	  /// Returns whether two values are equal for the purposes of this set.
	  /// </summary>
	  protected internal virtual bool valueEquals(object a, object b)
	  {
		return (a == null) ? (b == null) : a.Equals(b);
	  }

	  /// <summary>
	  /// Returns the hashCode for a value.
	  /// </summary>
	  protected internal virtual int valueHashCode(object v)
	  {
		return (v == null) ? 0 : v.GetHashCode();
	  }

	  /// <summary>
	  /// Ensures the map is large enough to contain the specified number of entries.
	  /// Default access to avoid synthetic accessors from inner classes.
	  /// </summary>
	  internal virtual void ensureSizeFor(int expectedSize)
	  {
		if (keys.Length * 3 >= expectedSize * 4)
		{
		  return;
		}

		int newCapacity = keys.Length << 1;
		while (newCapacity * 3 < expectedSize * 4)
		{
		  newCapacity <<= 1;
		}

		object[] oldKeys = keys;
		object[] oldValues = values_Renamed;
		initTable(newCapacity);
		for (int i = 0; i < oldKeys.Length; ++i)
		{
		  object k = oldKeys[i];
		  if (k != null)
		  {
			int newIndex = getKeyIndex(unmaskNullKey(k));
			while (keys[newIndex] != null)
			{
			  if (++newIndex == keys.Length)
			  {
				newIndex = 0;
			  }
			}
			keys[newIndex] = k;
			values_Renamed[newIndex] = oldValues[i];
		  }
		}
	  }

	  /// <summary>
	  /// Returns the index in the key table at which a particular key resides, or -1
	  /// if the key is not in the table. Default access to avoid synthetic accessors
	  /// from inner classes.
	  /// </summary>
	  internal virtual int findKey(object k)
	  {
		int index = getKeyIndex(k);
		while (true)
		{
		  object existing = keys[index];
		  if (existing == null)
		  {
			return -1;
		  }
		  if (keyEquals(k, unmaskNullKey(existing)))
		  {
			return index;
		  }
		  if (++index == keys.Length)
		  {
			index = 0;
		  }
		}
	  }

	  /// <summary>
	  /// Returns the index in the key table at which a particular key resides, or
	  /// the index of an empty slot in the table where this key should be inserted
	  /// if it is not already in the table. Default access to avoid synthetic
	  /// accessors from inner classes.
	  /// </summary>
	  internal virtual int findKeyOrEmpty(object k)
	  {
		int index = getKeyIndex(k);
		while (true)
		{
		  object existing = keys[index];
		  if (existing == null)
		  {
			return index;
		  }
		  if (keyEquals(k, unmaskNullKey(existing)))
		  {
			return index;
		  }
		  if (++index == keys.Length)
		  {
			index = 0;
		  }
		}
	  }

	  /// <summary>
	  /// Removes the entry at the specified index, and performs internal management
	  /// to make sure we don't wind up with a hole in the table. Default access to
	  /// avoid synthetic accessors from inner classes.
	  /// </summary>
	  internal virtual void internalRemove(int index)
	  {
		keys[index] = null;
		values_Renamed[index] = null;
		--size_Renamed;
		plugHole(index);
	  }

	  private int getKeyIndex(object k)
	  {
		int h = keyHashCode(k);
		// Copied from Apache's AbstractHashedMap; prevents power-of-two collisions.
		h += ~(h << 9);
		h ^= ((int)((uint)h >> 14));
		h += (h << 4);
		h ^= ((int)((uint)h >> 10));
		// Power of two trick.
		return h & (keys.Length - 1);
	  }

	  private void initTable(int capacity)
	  {
		keys = new object[capacity];
		values_Renamed = new object[capacity];
	  }

	  private void internalPutAll<T1>(IDictionary<T1> m) where T1 : K
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (Entry<? extends K, ? extends V> entry : m.entrySet())
		foreach (Entry<K, ? extends V> entry in m.SetOfKeyValuePairs())
		{
		  K key = entry.Key;
		  V value = entry.Value;
		  int index = findKeyOrEmpty(key);
		  if (keys[index] == null)
		  {
			++size_Renamed;
			keys[index] = maskNullKey(key);
			values_Renamed[index] = value;
		  }
		  else
		  {
			values_Renamed[index] = value;
		  }
		}
	  }

	  /// <summary>
	  /// Tricky, we left a hole in the map, which we have to fill. The only way to
	  /// do this is to search forwards through the map shuffling back values that
	  /// match this index until we hit a null.
	  /// </summary>
	  private void plugHole(int hole)
	  {
		int index = hole + 1;
		if (index == keys.Length)
		{
		  index = 0;
		}
		while (keys[index] != null)
		{
		  int targetIndex = getKeyIndex(unmaskNullKey(keys[index]));
		  if (hole < index)
		  {
			/*
			 * "Normal" case, the index is past the hole and the "bad range" is from
			 * hole (exclusive) to index (inclusive).
			 */
			if (!(hole < targetIndex && targetIndex <= index))
			{
			  // Plug it!
			  keys[hole] = keys[index];
			  values_Renamed[hole] = values_Renamed[index];
			  keys[index] = null;
			  values_Renamed[index] = null;
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
			  keys[hole] = keys[index];
			  values_Renamed[hole] = values_Renamed[index];
			  keys[index] = null;
			  values_Renamed[index] = null;
			  hole = index;
			}
		  }
		  if (++index == keys.Length)
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