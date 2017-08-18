// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactModelBase.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Contacts
{
    /// <summary>
    /// The contact model base.
    /// </summary>
    public abstract class ContactModelBase
    {
        /// <summary>
        /// Gets or sets the account contact login.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the account contact name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the account contact email.
        /// </summary>
        public string Email { get; set; }
    }
}
