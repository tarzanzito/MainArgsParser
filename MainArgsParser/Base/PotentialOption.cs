using System;

namespace Candal.MainArgumentsParser.Base
{
    public sealed class PotentialOption
    {
        public System.Enum Type { get; private set; }
        public PotentialActionDelegate Action { get; private set; }
        public string Descrition { get; private set; }
        public bool IsActivated { get; set; }
        public bool IsBlocked { get; set; }

        public PotentialOption(Enum type, PotentialActionDelegate action, string description)
        {
            this.Type = type;
            this.Action = action;
            this.Descrition = description;
            this.IsActivated = false;
            this.IsBlocked = false;
        }
    }
}
