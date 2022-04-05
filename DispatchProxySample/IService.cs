using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatchProxySample
{
    public interface IService
    {
        public Task<string> GetAsync(string name);
    }
}
