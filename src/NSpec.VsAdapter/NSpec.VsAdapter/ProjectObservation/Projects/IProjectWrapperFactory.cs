﻿using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSpec.VsAdapter.ProjectObservation.Projects
{
    public interface IProjectWrapperFactory
    {
        IProjectWrapper Create(IVsHierarchy projectHierarchy);
    }
}