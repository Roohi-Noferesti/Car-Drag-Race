using System;

public interface IWebRequest
{
	string Url
	{
		get;
	}

	bool IsDone
	{
		get;
	}

	string Error
	{
		get;
	}

	int Status
	{
		get;
	}

	string ContentType
	{
		get;
	}

	string Content
	{
		get;
	}

	void SetRequestHash(string requestHash);

	string GetRequestHash();

	void SetHeader(string name, string data);

	void SetTimeout(float timeout);

	void AddFormData(string name, string data);

	void Send();

	void Release();
}
