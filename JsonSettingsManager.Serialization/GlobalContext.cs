using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace JsonSettingsManager.Serialization
{
    public class GlobalContext
    {
        private int _id = -1;

        public int GetFileId() => Interlocked.Increment(ref _id);
    }
}
