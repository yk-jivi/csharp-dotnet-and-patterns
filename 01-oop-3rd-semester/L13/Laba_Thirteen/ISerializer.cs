using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laba_Thirteen
{
    public interface ISerializer
    {   
        void Serialize<T>(T obj, string file);
        T Deserialize<T>(string file);
    }
}
