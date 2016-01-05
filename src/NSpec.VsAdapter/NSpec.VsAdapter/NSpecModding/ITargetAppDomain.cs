﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSpec.VsAdapter.NSpecModding
{
    public interface ITargetAppDomain
    {
        Object CreateInstanceAndUnwrap(string marshalingAssemblyName, string marshalingTypeName);

        void Unload();
    }
}
