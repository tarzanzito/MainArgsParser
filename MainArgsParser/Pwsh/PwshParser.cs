using System;
using System.Collections.Generic;
using Candal.MainArgumentsParser.Base;

namespace Candal.MainArgumentsParser.Pwsh
{
    public sealed class PwshParser : Parser
    {
        #region Fields Readonly

        private static readonly string[] InitialChars = { "-" };
        private static readonly string InitialDefault = "-";

        #endregion

        #region Properties
        #endregion

        #region Constructors

        public PwshParser() : base(PwshOptionType.ArgumentNo)
        {
            AddHelpArgument(ComposeKeyArgument("h", 0));
            AddHelpArgument(ComposeKeyArgument("help", 1));
        }

        #endregion

        #region Protected Override Methods 

        protected override string ComposeKeyArgument(string argument, int order)
        {
            return InitialDefault + argument;
        }

        protected override string SplitArgument(string argument, string previousKey)
        {
            //then is an parameter of argument
            if (previousKey != null)
            {
                AddFoundArgument(previousKey, argument, 0);
                return null;
            }

            //is too short then is an parameter
            if (argument.Length == 1)
            {
                AddFoundParameter(argument);
                return null;
            }

            //is help too
            if (argument.Substring(0, 2) == "/?")
            { 
                AddFoundArgument(HelpBaseOption, null, 1);
                return null;
            }

            //get first argument  type
            string temp = argument.Substring(0, InitialDefault.Length);
            bool isInitialChar = (FindIndexOfArray(temp, InitialChars) == 0);

            //is an parameter
            if (!isInitialChar)
            {
               AddFoundParameter(argument);
                return null;
            }

            //get the corresponding action
            string optionKey = GetPotentialArgumentByKey(argument);
            PotentialOption optionAction = GetPotentialActionByKey(optionKey);
            PwshOptionType pwshOType = (PwshOptionType)optionAction.Type;

            //if arg need param then process arg in the next item
            string argKey = argument.Substring(InitialDefault.Length);
            if (pwshOType == PwshOptionType.ArgumentYes)
                return argKey;

            //save
            AddFoundArgument(argKey, null, 0);

            return null;
        }

        protected override void ValidateArgumentContent(string key, string value, Enum type)
        {
            PwshOptionType pwshType = (PwshOptionType)type;

            switch (pwshType)
            {
                case PwshOptionType.ArgumentNo:
                    if (value != null)
                        throw new ParserException("Option [" + key + "] do not need argument.");
                    break;

                case PwshOptionType.ArgumentYes:
                    if (value == null || value.Length == 0)
                        throw new ParserException("Option [" + key + "] need argument.");
                    break;
                default:
                    throw new ParserException("Unexpected ActionType: " + type.ToString());
            }
        }

        protected override string ExportPotentialOption(string key, PotentialOption action)
        {
            PwshOptionType pwshType = (PwshOptionType)action.Type;
            string value;

            switch (pwshType)
            {
                case PwshOptionType.ArgumentYes:
                    value = " <VALUE>,";
                    break;

                case PwshOptionType.ArgumentNo:
                    value = ",";
                    break;

                default:
                    value = ",";
                    break;
            }

            List<string> list = GetPotentialArgumentKeyList(key);

            return (list[0] + value).PadRight(30) + "-" + (list[1] + value).PadRight(30) + action.Descrition;
        }

        #endregion
    }
}
