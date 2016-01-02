﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSpec.VsAdapter
{
    public interface IProjectBuildNotifier
    {
        IObservable<IEnumerable<ProjectBuildInfo>> BuildStream { get; }
    }
}
