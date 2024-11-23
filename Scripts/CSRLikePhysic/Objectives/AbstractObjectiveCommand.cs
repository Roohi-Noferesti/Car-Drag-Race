using System;
using System.Reflection;

namespace Objectives
{
	public abstract class AbstractObjectiveCommand
	{
		private readonly string _methodName;

		private readonly object[] _parameters;

		protected AbstractObjectiveCommand(string methodName, object[] parameters)
		{
			this._methodName = methodName;
			this._parameters = parameters;
		}

		protected AbstractObjectiveCommand(string methodName) : this(methodName, new object[0])
		{
		}

		protected AbstractObjectiveCommand(object[] parameters)
		{
			this._methodName = base.GetType().Name;
			this._parameters = parameters;
		}

		protected AbstractObjectiveCommand()
		{
			this._methodName = base.GetType().Name;
			this._parameters = new object[0];
		}

		public void ExecuteOn(AbstractObjective objective)
		{
            MethodInfo method = CommandUtils.GetMethod(objective, this._methodName);
            if (method != null)
            {
                method.Invoke(objective, this._parameters);

                //Automatically will be send by ObjectManager
                //UpdateInServer(objective);
            }
		}
	}
}
