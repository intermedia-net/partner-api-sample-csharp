// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactCreateModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Contacts
{
    /// <summary>
    /// The account contact create model.
    /// </summary>
    internal sealed class ContactCreateModel : ContactModel
    {
        /// <summary>
        /// Gets or sets the account contact password.
        /// </summary>
        public string Password { get; set; }
    }
}