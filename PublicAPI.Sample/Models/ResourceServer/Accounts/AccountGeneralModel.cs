// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountGeneralModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts
{
    /// <summary>
    /// The account general model.
    /// </summary>
    internal sealed class AccountGeneralModel
    {
        /// <summary>
        /// Gets or sets the account user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the account password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the account parent account id.
        /// </summary>
        public string ParentAccountID { get; set; }

        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the account email.
        /// </summary>
        public string Email { get; set; }
    }
}