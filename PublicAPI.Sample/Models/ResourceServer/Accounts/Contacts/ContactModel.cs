// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Contacts
{
    /// <summary>
    /// The account contact model.
    /// </summary>
    public abstract class ContactModel
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

        /// <summary>
        /// Gets or sets the account contact alternative email.
        /// </summary>
        public string AlternativeEmail { get; set; }

        /// <summary>
        /// Gets or sets the account contact phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the account contact cellular phone.
        /// </summary>
        public string CellularPhone { get; set; }

        /// <summary>
        /// Gets or sets the access role names.
        /// </summary>
        public string[] AccessRoleNames { get; set; }
    }
}