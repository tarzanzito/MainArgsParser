
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Candal.MainArgumentsParser.Base
{
    public sealed class DataViewController
    {
        private readonly Parser _parent;

        public DataViewController(Parser parent)
        {
            _parent = parent;
        }

        public Dictionary<string, string> FoundArguments
        {
            get
            {
                object obj = CallProperty("FoundArguments");
                return obj as Dictionary<string, string>;
            }
        }

        public List<string> FoundParameters
        {
            get
            {
                object obj = CallProperty("FoundParameters");
                return obj as List<string>;
            }
        }

        public Dictionary<string, string> potentialArguments
        {
            get
            {
                object obj = CallProperty("potentialArguments");
                return obj as Dictionary<string, string>;
            }
        }
        public Dictionary<string, PotentialOption> PotentialActions
        {
            get
            {
                object obj = CallProperty("PotentialActions");
                return obj as Dictionary<string, PotentialOption>;
            }
        }

        private object CallProperty(string propertyName)
        {
            try
            {
                Type type = _parent.GetType().BaseType;
                PropertyInfo methodInfo = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);

                if (methodInfo == null)
                    throw new ParserException("Cannot find [" + propertyName + "] property.");

                object retObject = methodInfo.GetValue(_parent, null);
                return retObject;

            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
