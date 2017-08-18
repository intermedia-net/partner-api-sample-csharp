// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountOwnerModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts
{
    /// <summary>
    /// The account owner model.
    /// </summary>
    public sealed class AccountOwnerModel
    {
        /// <summary>
        /// Gets or sets the contact id.
        /// </summary>
        public string ContactID { get; set; }

        /// <summary>
        /// Gets or sets the contact.
        /// </summary>
        public AccountOwnerCreateModel Contact { get; set; }
    }
}
