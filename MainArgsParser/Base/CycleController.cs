
using System;
using System.Reflection;

namespace Candal.MainArgumentsParser.Base
{
    public sealed class CycleController
    {
        private readonly Parser _parent;

        public CycleController(Parser parent)
        {
            _parent = parent;
        }

        public void BlockOption(string name)
        {
            Call("BlockOption", name);
        }

        public void Exit()
        {
            Call("Exit", null);
        }

        public void ExitAndHelp()
        {
            Call("ExitAndHelp", null);
        }

        public void Break()
        {
            Call("Break", null);
        }

        private void Call(string methodName, string param)
        {
            try
            {
                Type type = _parent.GetType().BaseType;
                MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

                if (methodInfo == null)
                    throw new ParserException("Cannot find [" + methodName + "] method.");

                object[] parametersArray = null;
                if (param.Length > 0)
                    parametersArray = new object[] { param };

                methodInfo.Invoke(_parent, parametersArray);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private bool IsValidSubclass(object parent)
        {
            Type genericType = typeof(Parser);
            Type checkType = parent.GetType();

            while (checkType != null && checkType != typeof(object))
            {
                Type currentType = checkType.IsGenericType ? checkType.GetGenericTypeDefinition() : checkType;
                if (genericType == currentType)
                {
                    return true;
                }
                checkType = checkType.BaseType;
            }

            return false;
        }
    }
}
