using System;
using Candal.MainArgumentsParser.Lnx;
using Candal.MainArgumentsParser.Base;

namespace Candal
{
    public sealed class LnxParserExample1
    {
        public void Execute(string[] args)
        {
            LnxParser commandLineLnxParser = null;

            try
            {
                commandLineLnxParser = (LnxParser)new LnxParser()
                    .AddOption("c|create", LnxOptionType.Equal, "create new config.", (v, c) => Create(v, c))
                    .AddOption("n|name", LnxOptionType.Equal, "add or array names to pool.", (v, c) => AddName(v))
                    .AddOption("f|fill", LnxOptionType.None, "fill config.", (v, c) => Fill(c))
                    .AddOption("r|refresh", LnxOptionType.Signal, "refresh config.", (v, c) => Refresh())
                    .AddOption("i|install", LnxOptionType.None, "install new config.", (v, c) => Install())
                    .AddOption("g|goal", LnxOptionType.None, "show goals.", (v, c) => Goal())
                    .AddOption("k|kapa", LnxOptionType.Signal, "K2O3", (v, c) => Kapa(v, c))
                    .AddOption("d|delete-item", LnxOptionType.Equal, "delete config.", (v, c) => DeleteItem(v))
                    .AddParameter("parm1", "input file")
                    .AddParameter("parm2", "output file")
                    .CatchOutputMessages(OutputMessages)
                    .CatchValidateFoundArguments(ValidateFoundArguments)
                    .CatchValidateFoundParameters(ValidateFoundParameters)
                    .AddHelpAppDescription("this is a demo args .......")
                    .AddHelpFooterDescription("Note:\n bla bla bla\nbooo")
                    .ResolveExceptions(true)
                    .Process(args);
            }
            catch (Exception ex)  //if ResolveExceptions(false)
            {
                Environment.Exit(1);
            }
        }

        #region Delegate Methods

        private void OutputMessages(string value)
        {
            try
            {
                Console.WriteLine(value);
            }
            catch { }
        }

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
