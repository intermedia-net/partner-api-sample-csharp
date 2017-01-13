// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactGetModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Contacts
{
    /// <summary>
    /// The account contact get model.
    /// </summary>
    internal sealed class ContactGetModel : ContactModel
    {
        /// <summary>
        /// Gets or sets the account contact id.
        /// </summary>
        public string ContactID { get; set; }
    }
}