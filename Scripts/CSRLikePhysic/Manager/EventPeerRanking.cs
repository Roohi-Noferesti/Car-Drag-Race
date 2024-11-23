using System;

[Serializable]
public class EventPeerRanking
{
	private string _dname;

	public int uid;

	public int rp;

	public int rank;

	public string dname
	{
		get
		{
			return NameValidater.GetDisplayableName(this._dname, NameValidater.CreateIdUsername(this.uid));
		}
		set
		{
			this._dname = value;
		}
	}
}
