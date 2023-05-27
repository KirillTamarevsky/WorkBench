using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using WBGUIWPF.viewmodels;

namespace WBGUIWPF.Interfaces
{
    public interface IBenchComponent
    {
        JsonObject GetConfig();
        static abstract void InitVMFromJsonElement(JsonElement element, BenchConfigurationVM benchConfigurationVM);
    }
}
