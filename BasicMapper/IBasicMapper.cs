﻿using System.Collections.Generic;

namespace Approval.Core.Utility
{
    public interface IBasicMapper
    {
        T MapFields<T>(object from);
        T MapFields<T>(object from, List<string> skip);
        T MapFields<T>(object from, object to);
        T MapFields<T>(object from, object to, List<string> skip);
    }
}