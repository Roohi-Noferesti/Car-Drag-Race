using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable
{
	private readonly Heap<T> heap;

	public int Size
	{
		get
		{
			return this.heap.Size;
		}
	}

	public PriorityQueue()
	{
		this.heap = new Heap<T>();
	}

	public List<T> GetList()
	{
		return this.heap.List;
	}

	public bool IsEmpty()
	{
		return this.heap.Size == 0;
	}

	public T Top()
	{
		return this.heap.Peek();
	}

	public T Pop()
	{
		return this.heap.Remove();
	}

	public void Push(T t)
	{
		this.heap.Insert(t);
	}
}
