﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel.Compiler.Architectures.x86_32
{
    /// <summary>
    /// See base class documentation.
    /// </summary>
    public class Ret : ILOps.Ret
    {
        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="anILOpInfo">See base class documentation.</param>
        /// <param name="aScannerState">See base class documentation.</param>
        /// <returns>See base class documentation.</returns>
        public override string Convert(ILOpInfo anILOpInfo, ILScannerState aScannerState)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("ret");

            return result.ToString().Trim();
        }
    }
}
