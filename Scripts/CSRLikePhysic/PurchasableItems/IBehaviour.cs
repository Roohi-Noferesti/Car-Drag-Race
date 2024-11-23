namespace PurchasableItems
{
	public interface IBehaviour
	{
		bool IsOwned(string value);

		void Apply(string value);

		void Revoke(string value);
	}
}
