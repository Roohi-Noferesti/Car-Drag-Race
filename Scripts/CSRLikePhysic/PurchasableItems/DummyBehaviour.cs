namespace PurchasableItems
{
	public class DummyBehaviour : IBehaviour
	{
		public bool IsOwned(string value)
		{
			return false;
		}

		public void Apply(string value)
		{
		}

		public void Revoke(string value)
		{
		}
	}
}
