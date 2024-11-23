using LitJson;
using System;

public class MetricsDictionary
{
	public delegate void Populate(MetricsDictionary dict);

	public JsonDict JSON
	{
		get;
		private set;
	}

	public MetricsDictionary(string kingdom, string phylum)
	{
		this.JSON = new JsonDict();
		this.Set<string>("kingdom", kingdom);
		this.Set<string>("phylum", phylum);
	}

	public MetricsDictionary(string kingdom, string phylum, string _class, string family, string genus, string value)
	{
		this.JSON = new JsonDict();
		this.Set<string>("kingdom", kingdom);
		this.Set<string>("phylum", phylum);
		if (_class != string.Empty)
		{
			this.Set<string>("class", _class);
		}
		if (family != string.Empty)
		{
			this.Set<string>("family", family);
		}
		if (genus != string.Empty)
		{
			this.Set<string>("genus", genus);
		}
		if (value != string.Empty)
		{
			this.Set<string>("value", value);
		}
	}

	public void Set<T>(string key, T value)
	{
		if (value != null)
		{
			this.JSON.Set(key, value.ToString());
		}
	}

	public override string ToString()
	{
		return this.JSON.ToString();
	}
}
