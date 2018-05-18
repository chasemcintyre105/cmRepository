using System;
using System.Reflection;
using System.Collections.Generic;

namespace RuleEngine {

	public abstract class TransitionHandler<Command, State> where Command : struct, IConvertible
												   where State : struct, IConvertible  {

        public abstract void Init();

        private Dictionary<Command, MethodInfo> methods;
		protected Engine E;

		public TransitionHandler(Engine E) {
			this.E = E;

			methods = new Dictionary<Command, MethodInfo>();
			foreach (Command command in Enum.GetValues(typeof(Command))) {

				string MethodName = "Handle_" + command.ToString();

				MethodInfo method = this.GetType().GetMethod(MethodName);

				if (method == null)
					ThrowTheError(command);

				ParameterInfo[] parameters =  method.GetParameters();

				if (parameters.Length != 1)
					ThrowTheError(command);

				if (parameters[0].ParameterType != typeof(Dictionary<string, object>))
					ThrowTheError(command);

				methods.Add(command, method);

			}
		}

		public void ThrowTheError(Command command) {
			throw new Exception("TransitionHandler<" + typeof(Command).Name + ", " + typeof(State).Name + "> is missing the handler method 'Handle_" + command.ToString() + "(Dictionary<string, object> properties)'");
		}

		internal void HandleTransition(Command command, Dictionary<string, object> properties) {
			methods[command].Invoke(this, new object[] { properties });
		}

        protected T GetProperty<T>(Dictionary<string, object> properties, string propertyName) {
            Assert.NotNull("Properties dictionary is not defined and therefore no properties could be extracted.", properties);

            Assert.True("Does not have property: " + propertyName, properties.ContainsKey(propertyName));

            object propertyObject = properties[propertyName];

            if (propertyObject == null)
                throw new Exception("Property '" + propertyName + "' has been set but is null");

            Assert.True("Property object " + propertyName + " (" + typeof(T).Name + ") is not the requested type: " + propertyObject.GetType().Name, TypeFunctions.IsSameOrSubclassOf(propertyObject.GetType(), typeof(T)));

            return (T) propertyObject;

        }

	}

}

