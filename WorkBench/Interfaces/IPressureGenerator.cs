using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;

namespace WorkBench.Interfaces
{
    public interface IPressureGenerator : IGenerator
    {
        PressureControllerOperationMode OperationMode { get; set; }
        bool CanGeneratePressureForGivenScale(Scale scale);
        bool PrepareForGenerationOnGivenScale(Scale scale);

    }
}
