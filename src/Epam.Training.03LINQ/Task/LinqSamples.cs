// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System.Collections.Generic;
using SampleSupport;
using Task.Data;

namespace Task
{
	[Title("LINQ Module")]
	[Prefix("Linq")]
	public partial class LinqSamples : SampleHarness
	{
		private readonly DataSource _dataSource = new DataSource();

	    private void Dump(IEnumerable<object> obj)
	    {
            foreach (var o in obj)
            {
                ObjectDumper.Write(o);
            }
        }
	}
}
