using System;

public class JsonException : ApplicationException
{
	public JsonException(Exception jsonException) : base(jsonException.Message, jsonException.InnerException)
	{
	}
}
