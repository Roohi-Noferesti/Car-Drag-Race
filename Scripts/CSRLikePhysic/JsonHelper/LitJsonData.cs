public class LitJsonData : JsonData
{
	private LitJson.JsonData data;

	public LitJsonData(LitJson.JsonData newData)
	{
		this.data = newData;
	}

	public string Serialize()
	{
		return this.data.ToJson();
	}
}
