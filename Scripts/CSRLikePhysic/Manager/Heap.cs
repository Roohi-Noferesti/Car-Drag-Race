using System;
using System.Collections.Generic;

public class Heap<T> where T : IComparable
{
	private readonly List<T> list;

	private int size;

	public List<T> List
	{
		get
		{
			return this.list;
		}
	}

	public int Size
	{
		get
		{
			return this.size;
		}
		private set
		{
			this.size = value;
		}
	}

	public Heap()
	{
		this.list = new List<T>();
		this.size = 0;
	}

	public Heap(List<T> aList, int count = 0)
	{
		this.list = aList;
		this.size = ((count <= 0) ? aList.Count : count);
		this.MakeHeap();
	}

	public T Remove()
	{
		if (this.size == 0)
		{
			throw new InvalidOperationException("Heap is empty.");
		}
		T result = this.list[0];
		this.Swap(0, this.size - 1);
		this.size--;
		this.HeapDown(0);
		return result;
	}

	public T Peek()
	{
		if (this.size == 0)
		{
			throw new InvalidOperationException("Heap is empty.");
		}
		return this.list[0];
	}

	public void Insert(T t)
	{
		this.list.Add(t);
		this.size++;
		this.HeapUp(this.size - 1);
	}

	public override string ToString()
	{
		string text = string.Format("[Heap: Size={0}]\n", this.size);
		text += "{ ";
		int num = 0;
		for (int i = 0; i < this.size; i++)
		{
			string arg_46_0 = text;
			T t = this.list[i];
			text = arg_46_0 + t.ToString();
			if (num < this.size - 1)
			{
				text += ", ";
			}
			num++;
		}
		return text + " }\n";
	}

	public static void HeapSort<U>(List<U> list, Action<U> action) where U : IComparable
	{
		Heap<U> heap = new Heap<U>(list, list.Count);
		while (heap.Size > 0)
		{
			action(heap.Remove());
		}
	}

	private void HeapDown(int index)
	{
		while (this.HasLeftChild(index))
		{
			int num = this.LeftChild(index);
			if (this.HasRightChild(index))
			{
				T t = this.list[num];
				if (t.CompareTo(this.list[this.RightChild(index)]) > 0)
				{
					num = this.RightChild(index);
				}
			}
			T t2 = this.list[index];
			if (t2.CompareTo(this.list[num]) <= 0)
			{
				break;
			}
			this.Swap(index, num);
			index = num;
		}
	}

	private void HeapUp(int index)
	{
		while (this.HasParent(index))
		{
			T t = this.list[this.Parent(index)];
			if (t.CompareTo(this.list[index]) <= 0)
			{
				break;
			}
			int num = this.Parent(index);
			this.Swap(index, num);
			index = num;
		}
	}

	private void MakeHeap()
	{
		for (int i = this.Parent(this.size - 1); i >= 0; i--)
		{
			this.HeapDown(i);
		}
	}

	private int LeftChild(int index)
	{
		return index * 2 + 1;
	}

	private int RightChild(int index)
	{
		return index * 2 + 2;
	}

	private int Parent(int index)
	{
		return (index - 1) / 2;
	}

	private bool HasParent(int index)
	{
		return index >= 1;
	}

	private bool HasLeftChild(int index)
	{
		return this.LeftChild(index) < this.size;
	}

	private bool HasRightChild(int index)
	{
		return this.RightChild(index) < this.size;
	}

	private void Swap(int a, int b)
	{
		T value = this.list[a];
		this.list[a] = this.list[b];
		this.list[b] = value;
	}
}
