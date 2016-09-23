// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LimitModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2016. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Limits
{
    /// <summary>
    /// The account limit model.
    /// </summary>
    internal sealed class LimitModel : LimitUpdateModel
    {
        /// <summary>
        /// Gets or sets the account limit name.
        /// </summary>
        public string Name { get; set; }
    }
}
