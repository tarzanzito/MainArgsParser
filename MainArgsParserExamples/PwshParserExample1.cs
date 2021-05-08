using System;
using Candal.MainArgumentsParser.Pwsh;
using Candal.MainArgumentsParser.Base;

namespace Candal
{
    public sealed class PwshParserExample1
    {
        public void Execute(string[] args)
        {
            PwshParser commandLinePwshParser;

            try
            {
                commandLinePwshParser = (PwshParser) new PwshParser()
                    .AddOption("EncodedCommand|e|ec", PwshOptionType.ArgumentNo, "create new config.", (v, c) => Create(v, c))
                    .AddOption("name-item|n|ni", PwshOptionType.ArgumentYes, "add or array names to pool.", (v, c) => AddName(v))
                    .AddOption("fill|f", PwshOptionType.ArgumentNo, "fill config.", (v, c) => Fill(c))
                    .AddOption("r|refresh", PwshOptionType.ArgumentYes, "refresh config.", (v, c) => Refresh())
                    .AddOption("i|install", PwshOptionType.ArgumentNo, "install new config.", (v, c) => Install())
                    .AddOption("goal|go", PwshOptionType.ArgumentNo, "show goals.", (v, c) => Goal())
                    .AddOption("kapa", PwshOptionType.ArgumentYes, "K2O3", (v, c) => Kapa(v, c))
                    .AddOption("delete-item", PwshOptionType.ArgumentYes, "delete record.", (v, c) => DeleteItem(v))
                    .AddParameter("parm1", "input file")
                    .AddParameter("parm2", "output file")
                    .CatchOutputMessages(OutputMessages)
                    .CatchValidateFoundArguments(ValidateFoundArguments)
                    .CatchValidateFoundParameters(ValidateFoundParameters)
                    .AddHelpAppDescription("this is a demo program. Parse and Process main args.")
                    .AddHelpFooterDescription("Note:\n bla bla bla\nbooo")
                    .ResolveExceptions(true)
                    .Process(args);
            }
            catch (Exception ex) //if ResolveExceptions(false)
            {
                Environment.Exit(1);
            }
        }

        #region Delegate Methods

        private void ValidateFoundArguments(DataViewController controller)
        {
            //if (controller.FoundArguments == null || controller.FoundArguments.Count == 0)
            //    throw new CommandLineParserException("No arguments found");
        }

        private void ValidateFoundParameters(DataViewController controller)
        {
            //if (controller.FoundParameters == null || controller.FoundParameters.Count == 0)
            //    throw new CommandLineParserException("No parameters found");
        }

        private void OutputMessages(string value)
        {
            try
            {
                Console.WriteLine(value);
            }
            catch { }
        }

        private void Create(string value, CycleController controller)
        {
            OutputMessages("option [create] executed: " + value);
            controller.BlockOption("i|install");
        }

        private void AddName(string value)
        {
            OutputMessages("option [addName] executed: " + value);
        }

        private void Fill(CycleController controller)
        {
            OutputMessages("option [fill] executed.");
        }

        private void Refresh()
        {
            OutputMessages("option [refresh] executed.");
        }

        private void Install()
        {
            OutputMessages("option [install] executed.");
        }

        private void Goal()
        {
            OutputMessages("option [goal] executed.");
        }

        private void Kapa(string value, CycleController controller)
        {
            OutputMessages("option [kapa] executed: " + value);
            //controller.Exit();
            //controller.ExitAndHelp();
            //controller.Break();
            //throw new CommandLineParserException("Something is wrong.");
        }

        private void DeleteItem(string value)
        {
            OutputMessages("option [deleteItem] executed: " + value);
        }

        #endregion
    }
}
