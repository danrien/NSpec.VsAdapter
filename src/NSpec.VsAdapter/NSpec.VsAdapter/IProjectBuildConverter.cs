﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSpec.VsAdapter
{
    public interface IProjectBuildConverter
    {
        string ToTestDllPath(ProjectBuildInfo projectBuildInfo);
    }
}
