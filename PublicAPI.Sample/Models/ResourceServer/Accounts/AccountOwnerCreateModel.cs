// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountOwnerCreateModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts
{
    using Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Contacts;

    /// <summary>
    /// The account owner create model.
    /// </summary>
    public sealed class AccountOwnerCreateModel : ContactModelBase
    {
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }
    }
}
