using System;

namespace Objectives.Impl
{
	public class SampleObjective : AbstractObjective
	{
		public string EditableField;

		[SerializeInProfile]
		private string SavedField;

		[Command]
		public void OnSomethingOrOther()
		{
		}
	}
}
