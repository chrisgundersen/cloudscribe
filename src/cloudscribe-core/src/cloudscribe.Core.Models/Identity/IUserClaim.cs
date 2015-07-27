﻿// Copyright (c) Source Tree Solutions, LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
namespace cloudscribe.Core.Models
{
    public interface IUserClaim
    {
        string ClaimType { get; set; }
        string ClaimValue { get; set; }
        int Id { get; set; }
        string UserId { get; set; }
    }
}