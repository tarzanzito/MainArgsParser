
using System.Collections.Generic;

namespace Candal.MainArgumentsParser.Base
{
    public delegate void PotentialActionDelegate(string value, CycleController controller);
    public delegate void OutputMessageDelegate(string value);
    public delegate void DataViewDelegate(DataViewController controller);
}
