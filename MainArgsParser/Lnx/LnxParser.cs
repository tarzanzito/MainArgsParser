using System;
using System.Collections.Generic;
using Candal.MainArgumentsParser.Base;

namespace Candal.MainArgumentsParser.Lnx
{
    public sealed class LnxParser : Parser
    {
        #region Fields Readonly

        private static readonly string[] FirstInitialChars = { "-", "/" };
        private static readonly string FirstInitialDefault = "-";
        private static readonly string[] SecondInitialChars = { "--" };
        private static readonly string SecondInitialDefault = "--";
        private static readonly string[] EqualChars = { "=", ":" };
        private static readonly string EqualDefault = "=";
        private static readonly string[] SignalChars = { "+", "-" };
        private static readonly string SignalDefault = "+";

        #endregion

        #region Properties
        #endregion

        #region Constructors

        public LnxParser() : base(LnxOptionType.None)
        {
            AddHelpArgument(ComposeKeyArgument("h", 0));
            AddHelpArgument(ComposeKeyArgument("help", 1));
        }

        #endregion

        #region Protected Methods

        protected override string ComposeKeyArgument(string argument, int order)
        {
            // like FirstInitialChars
            if (order == 0)
                return FirstInitialDefault + argument;

            // like SecondInitialChars
            if (order == 1)
                return SecondInitialDefault + argument;

            throw new ParserException("Potencial options can only have two key (short and long) ");
        }

        protected override string SplitArgument(string argument, string previousKey)
        {
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

            //first type: begin with: FirstInitialChars
            string argTemp = argument.Substring(0, FirstInitialDefault.Length);
            int pos = FindIndexOfArray(argTemp, FirstInitialChars);
            bool isFirstType = (pos == 0);

            //second type: begin with: SecondInitialChars
            argTemp = argument.Substring(0, SecondInitialDefault.Length);
            pos = FindIndexOfArray(argTemp, SecondInitialChars);
            bool isSecondType = pos == 0;

            //is an parameter
            if (!isFirstType && !isSecondType) 
                {
                AddFoundParameter(argument);
                return null;
            }

            string key;
            string value;

            //clean key
            if (isSecondType)
                argTemp = argument.Substring(SecondInitialDefault.Length); 
            else
                argTemp = argument.Substring(FirstInitialDefault.Length);

            //has an equals type
            pos = FindIndexOfArray(argTemp, EqualChars);
            if (pos > 0)
            {
                key = argTemp.Substring(0, pos);
                value = argTemp.Substring(pos + EqualDefault.Length, argTemp.Length - pos - EqualDefault.Length);
            }
            else
            {
                //has an signal type
                pos = FindIndexOfArray(argTemp, SignalChars);
                if (pos == (argTemp.Length - SignalDefault.Length))
                {
                    key = argTemp.Substring(0, pos);
                    value = argTemp.Substring(pos, SignalDefault.Length);
                }
                else
                {
                    key = argTemp;
                    value = "";
                }
            }

            //save
            AddFoundArgument(key, value, (isSecondType ? 1 : 0));

            return null;
        }
 
        protected override void ValidateArgumentContent(string key, string value, Enum type)
        {
            //secondInitialForm lenght > 1
            if (key.Substring(0, SecondInitialDefault.Length) == SecondInitialDefault)
            {
                if (key.Length < (SecondInitialDefault.Length + 1))
                    throw new ParserException("Option with base mask [" + SecondInitialDefault + "] must be multi char");
            }
            else
            {
                //firstInitialForm lenght = 1
                if (key.Length > (FirstInitialDefault.Length + 1))
                    throw new ParserException("Option with base  [" + FirstInitialDefault + "] must be single char");
            }

            LnxOptionType lnxType = (LnxOptionType)type;
            int pos;

            switch (lnxType)
            {
                case LnxOptionType.Equal:
                    if (value.Length == 0)
                        throw new ParserException("Option value is empty. : [" + key + "]");
                    break;

                case LnxOptionType.NoneOrEqual:
                    break;

                case LnxOptionType.None:
                    if (value.Length != 0)
                        throw new ParserException("Option cannot have value. : [" + key + "]");
                    break;

                case LnxOptionType.Signal:
                    pos = FindIndexOfArray(value, SignalChars);
                    if (pos != 0)
                        throw new ParserException("Option value must have : [" + StringArrayToString(SignalChars) + "]");
                    break;

                case LnxOptionType.NoneOrSignal:
                    if (value.Length > 0)
                    {
                        pos = FindIndexOfArray(value, SignalChars);
                        if (pos != 0)
                            throw new ParserException("Option value must have : [" + StringArrayToString(SignalChars) + "]");
                    }
                    break;

                default:
                    throw new ParserException("Unexpected ActionType: [" + type.ToString() + "]");
            }
        }

        protected override string ExportPotentialOption(string key, PotentialOption action)
        {
            LnxOptionType lnxType = (LnxOptionType)action.Type;
            string value;

            switch (lnxType)
            {
                case LnxOptionType.Equal:
                case LnxOptionType.NoneOrEqual:
                    value = "=VALUE,";
                    break;
                case LnxOptionType.Signal:
                case LnxOptionType.NoneOrSignal:
                    value = "+,";
                    break;

                default:
                    value = ",";
                    break;
            }

            List<string> list = GetPotentialArgumentKeyList(key);

            return (list[0] + value).PadRight(10) + (list[1] + value).PadRight(30) + action.Descrition;
        }

        #endregion
    }
}
