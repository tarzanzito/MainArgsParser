using System;
using System.Collections.Generic;
using System.Linq;

namespace Candal.MainArgumentsParser.Base
{

    public abstract class Parser 
    {
        #region Fields Readonly

        protected static readonly string HelpBaseOption = "help";
        private static readonly string OptionSeparator = "|";
        private static readonly char[] IniEndChars = { '\'', '"' };
        private static readonly short IniEndLenght= 1;

        //key: id, value: action
        private readonly Dictionary<string, PotentialOption> _potentialActionList; 
        //key:splited argument, value: id
        private readonly Dictionary<string, string> _potentialArgumentList;
        //parameter
        private readonly Dictionary<string, string> _potentialParameterList;
        //key:splited argument, value:?
        private readonly Dictionary<string, string> _foundArgumentList;
        //argument: parameter
        private readonly List<string> _foundParameterlist;
        //controllers
        private readonly CycleController _cycleController;
        private readonly DataViewController _dataViewController;
        private List<string> _helpArgumentList;

        #endregion

        #region Fields

        private OperationState _operationState;
        private string _helpAppDescription;
        private string _helpFooterDescription;
        private OutputMessageDelegate _outputMessage;
        private DataViewDelegate _validateFoundParameters;
        private DataViewDelegate _validateFoundArguments;
        private bool _resolveExceptions;

        #endregion

        #region Properties
        #endregion

        #region Constructors

        protected Parser(System.Enum enumType)
        {
            _operationState = OperationState.Preparing;

            _cycleController = new CycleController(this);
            _dataViewController = new DataViewController(this);

            _potentialActionList = new Dictionary<string, PotentialOption>();
            _potentialArgumentList = new Dictionary<string, string>();
            _foundArgumentList = new Dictionary<string, string>();
            _potentialParameterList = new Dictionary<string, string>();
            _foundParameterlist = new List<string>();
            _foundParameterlist = new List<string>();
            _helpArgumentList = new List<string>();
            _resolveExceptions = true;

            _helpArgumentList.Add("/?");
            AddOption("h|" + HelpBaseOption, enumType, "show help message and exit", (sender, parm) => ShowHelp());
        }

        #endregion

        #region Public Methods 

        public Parser AddOption(string option, System.Enum type, string description, PotentialActionDelegate action)
        {
            try
            {
                ValidateOperationState(OperationState.Preparing);

                if (option == null)
                    throw new ParserException("Option is null.");

                if (action == null)
                    throw new ParserException("Action is null.");

                if (option.Trim() == "")
                    throw new ParserException("Option is empty.");

                PotentialOption commandLineOption = new PotentialOption(type, action, description);
                _potentialActionList.Add(option, commandLineOption); //delegate

                string[] opt = option.Split(OptionSeparator);

                for (int inx = 0; inx < opt.Length; inx++)
                {
                    string item = ComposeKeyArgument(opt[inx].Trim(), inx);
                    _potentialArgumentList.Add(item, option);
                }
            }
            catch (Exception ex)
            {
                WriteFinalExceptionInfo(ex);
            }

            return this;
        }

        public Parser AddParameter(string parameter, string description)
        {
            try
            {
                ValidateOperationState(OperationState.Preparing);

                if (parameter == null)
                    throw new ParserException("Parameter is null.");

                if (parameter.Trim() == "")
                    throw new ParserException("Parameter is empty.");

                _potentialParameterList.Add(parameter, description);
            }
            catch (Exception ex)
            {
                WriteFinalExceptionInfo(ex);
            }

            return this;
        }

        public Parser AddHelpAppDescription(string description)
        {
            try
            {
                ValidateOperationState(OperationState.Preparing);

                _helpAppDescription = description;
            }
            catch (Exception ex)
            {
                WriteFinalExceptionInfo(ex);
            }

            return this;
        }

        public Parser AddHelpFooterDescription(string description)
        {
            try
            {
                ValidateOperationState(OperationState.Preparing);

                _helpFooterDescription = description;
            }
            catch (Exception ex)
            {
                WriteFinalExceptionInfo(ex);
            }

            return this;
        }

        public Parser Process(string[] args)
        {
            try
            {
                if (args == null)
                    return this;

                ValidateOperationState(OperationState.Preparing);

                SplitArgumentArray(args);
                ValidateHelpArguments();

                if (_operationState == OperationState.Preparing)
                {
                    ValidateFoundArgumentList();
                    FireValidateFoundArgumentList();
                    FireValidateFoundParameterList();
                    ExecuteActions();
                }

                if (_operationState == OperationState.ExitedAndHelp)
                {
                    ShowHelp();
                    Environment.Exit(1);
                }

                if (_operationState == OperationState.Exited)
                    Environment.Exit(0);
            }
            catch (Exception ex)
            {
                WriteFinalExceptionInfo(ex);
            }

            return this;
        }

        public Parser CatchOutputMessages(OutputMessageDelegate outputMessage)
        {
            try
            {
                ValidateOperationState(OperationState.Preparing);
                _outputMessage = outputMessage;
            }
            catch (Exception ex)
            {
                WriteFinalExceptionInfo(ex);
            }

            return this;
        }

        public Parser CatchValidateFoundParameters(DataViewDelegate validateFoundParameters)
        {
            try
            {
                ValidateOperationState(OperationState.Preparing);
                _validateFoundParameters = validateFoundParameters;

            }
            catch (Exception ex)
            {
                WriteFinalExceptionInfo(ex);
            }

            return this;
        }
        
        public Parser CatchValidateFoundArguments(DataViewDelegate validateFoundArguments)
        {
            try
            {
                ValidateOperationState(OperationState.Preparing);
                _validateFoundArguments = validateFoundArguments;

            }
            catch (Exception ex)
            {
                WriteFinalExceptionInfo(ex);
            }

            return this;
        }

        public Parser ResolveExceptions(bool internally)
        {
            try
            {
                ValidateOperationState(OperationState.Preparing);
                _resolveExceptions = internally;

            }
            catch (Exception ex)
            {
                WriteFinalExceptionInfo(ex);
            }
            
            return this;
        }

        #endregion

        #region Protected Methods 

        protected void AddFoundArgument(string key, string value, int order)
        {
            if (key == null)
                return;

            string nKey = ComposeKeyArgument(key, order);
            if (_foundArgumentList.ContainsKey(nKey))
                throw new ParserException("Duplicate argument found. [" + nKey + "]");

            _foundArgumentList.Add(nKey, value);
        }

        protected void AddFoundParameter(string parameter)
        {
            string temp = parameter;

            if (parameter == null)
                return;

            int len = parameter.Length;
            if (len > 0)
            {
                if (parameter.IndexOfAny(IniEndChars) == 0)
                {
                    string chrI = parameter.Substring(0, IniEndLenght);
                    string chrE = parameter.Substring((len - IniEndLenght), IniEndLenght);
                    if (chrE == chrI)
                        temp = parameter.Substring(1, (len - IniEndLenght));
                    else
                        throw new ParserException("Begin and end of parameter not match. [" + parameter + "]");
                }
            }

            _foundParameterlist.Add(temp);
        }

        protected string GetPotentialArgumentByKey(string key)
        {
            if (!_potentialArgumentList.ContainsKey(key))
                throw new ParserException("Duplicate argument found in potencial option. [" + key + "]"); 

            return _potentialArgumentList[key];
        }

        protected List<string> GetPotentialArgumentKeyList(string value)
        {
            //linq
            List<string> keyList = (from entry in _potentialArgumentList
                                    where entry.Value == value
                                    select entry.Key)
                                    .ToList();

            ////lambda
            //List<string> keyList = potentialArgumentList
            //    .Where(x => x.Value == value)
            //    .Select(x => x.Key)
            //    .ToList();

            //V1
            //List<string> keyList = new List<string>();
            //foreach (KeyValuePair<string, string> item in potentialArgumentList)
            //{
            //    if (item.Value == value)
            //        keyList.Add(item.Key);
            //}

            return keyList;
        }

        protected PotentialOption GetPotentialActionByKey(string key)
        {
            if (_potentialActionList.ContainsKey(key))
                return _potentialActionList[key];
            else
                return null;
        }

        protected void ShowHelp()
        {
            try 
            {
                string appName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

                FireOutputMessage("Usage: " + appName + " [OPTIONS] PARAMS");

                FireOutputMessage(_helpAppDescription);

                FireOutputMessage("");
                FireOutputMessage("Options:");
                ExportPotentialOptions();

                FireOutputMessage("");
                FireOutputMessage("Parameters:");
                ExportPotentialParameters();

                FireOutputMessage("");

                FireOutputMessage(_helpFooterDescription);

            }
            catch (Exception ex)
            {
                WriteFinalExceptionInfo(ex);
            }
        }

        protected void AddHelpArgument(string helpArgument)
        {
            _helpArgumentList.Add(helpArgument);
        }

        protected int FindIndexOfArray(string source, string[] anyOf)
        {
            int pos;
            foreach (string item in anyOf)
            {
                pos = source.IndexOf(item);
                if (pos != -1)
                    return pos;
            }

            return -1;
        }

        protected string StringArrayToString(string[] array)
        {
            return String.Join(OptionSeparator, array);
        }

        #endregion

        #region Private Methods

        private void ValidateHelpArguments()
        {
            foreach (KeyValuePair<string, string> item in _foundArgumentList)
            {
                if (_helpArgumentList.Contains(item.Key))
                {
                    _operationState = OperationState.ExitedAndHelp;
                    break;
                }
            }
        }

        private void SplitArgumentArray(string[] args)
        {
            string previousKey = null;
            foreach (string item in args)
            {
                if (item.Length == 0)
                    continue;
                
                previousKey = SplitArgument(item, previousKey);
            }

            if (previousKey != null) 
                throw new ParserException("Option: [" + previousKey + "] need argument.");
        }

        private void ValidateFoundArgumentList()
        {
            foreach (KeyValuePair<string, string> item in _foundArgumentList)
                ValidateFoundArgument(item);
        }

        private void ValidateFoundArgument(KeyValuePair<string, string> argument)
        {
            if (!_potentialArgumentList.ContainsKey(argument.Key))
                throw new ParserException("Invalid option:" + argument.Key);

            string optionId = _potentialArgumentList[argument.Key];
            PotentialOption commandLineAction = _potentialActionList[optionId];

            if (commandLineAction.IsActivated)
                throw new ParserException("Duplicate argument found: [" + argument.Key + "] Option: [" + optionId + "]");

            ValidateArgumentContent(argument.Key, argument.Value, commandLineAction.Type);

            commandLineAction.IsActivated = true;
        }

        private void ExecuteActions()
        {
            _operationState = OperationState.Running;

            //loop from original order
            foreach (KeyValuePair<string, string> item in _potentialArgumentList)
            {
                if (_foundArgumentList.ContainsKey(item.Key))
                {
                    PotentialOption commandLineAction = _potentialActionList[item.Value];
                    if (!commandLineAction.IsBlocked)
                    {
                        string value = _foundArgumentList[item.Key];
                        commandLineAction.Action(value, _cycleController);
                    }
                }

                if (_operationState != OperationState.Running)
                    break;
            }

            if (_operationState == OperationState.Running)
                _operationState = OperationState.Finished;
        }

        private void ExportPotentialOptions()
        {
            foreach (KeyValuePair<string, PotentialOption> item in _potentialActionList)
            {
                string lineOut = ExportPotentialOption(item.Key, item.Value);
                FireOutputMessage("  " + lineOut);
            }
        }

        private void ExportPotentialParameters()
        {
            foreach (KeyValuePair<string, string> item in _potentialParameterList)
                FireOutputMessage("  " + (item.Key + ",").PadRight(40) + item.Value);
        }

        private void ValidateOperationState(OperationState operationState)
        {
            if (_operationState != operationState)
                throw new ParserException("Operation State is not: " + operationState.ToString());
        }

        private void WriteFinalExceptionInfo(Exception ex)//aqui
        {
            if (!_resolveExceptions)
                throw ex;

            try
            {
                string appName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

                FireOutputMessage("Error: " + ex.Message);

                string help = string.Join(", ", _helpArgumentList);
                FireOutputMessage("Try '" + appName + " " + help + "' for more information.");
            }
            catch { }

            Environment.Exit(1);
        }

        #endregion

        #region Events

        private void FireOutputMessage(string value)
        {
            if (value == null)
                return;

            if (_outputMessage == null)
            {
                 Console.WriteLine(value);
            }
            else
                _outputMessage(value);
        }

        private void FireValidateFoundParameterList()
        {
            if (_validateFoundParameters != null)
                _validateFoundParameters(_dataViewController);
        }

        private void FireValidateFoundArgumentList()
        {
            if (_validateFoundArguments != null)
                _validateFoundArguments(_dataViewController);
        }
        
        #endregion

        #region Abstract

        protected abstract string ComposeKeyArgument(string argument, int order);

        protected abstract string SplitArgument(string argument, string previousKey);

        protected abstract void ValidateArgumentContent(string key, string value, System.Enum type);

        protected abstract string ExportPotentialOption(string key, PotentialOption action);

        #endregion

        #region Private Methods and Properties For CycleController

        private void BlockOption(string option)
        {
            if (!_potentialActionList.ContainsKey(option))
                throw new ParserException("BlockOption error: 'Option Id' not found. : [" + option + "]");

            PotentialOption commandLineAction = _potentialActionList[option];
            commandLineAction.IsBlocked = true;
        }

        private void Exit()
        {
            _operationState = OperationState.Exited;
        }

        private void ExitAndHelp()
        {
            _operationState = OperationState.ExitedAndHelp;
        }

        private void Break()
        {
            _operationState = OperationState.Breaked;
        }

        #endregion

        #region Private Methods and Properties For DataViewController

        private Dictionary<string, string> FoundArguments
        {
            get
            {
                return _foundArgumentList;
            }
        }

        private List<string> FoundParameters
        {
            get
            {
                return _foundParameterlist;
            }
        }

        private Dictionary<string, string> potentialArguments
        {
            get
            {
                return _potentialArgumentList;
            }
        }

        private Dictionary<string, PotentialOption> PotentialActions
        {
            get
            {
                return _potentialActionList;
            }
        }

        #endregion
    }
}
