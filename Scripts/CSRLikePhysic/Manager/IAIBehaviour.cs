public interface IAIBehaviour
{
	void Begin();

	IAIBehaviour Update(out DriverInputs zDriverInputs);

	IAIBehaviour ForceNextState();
}
