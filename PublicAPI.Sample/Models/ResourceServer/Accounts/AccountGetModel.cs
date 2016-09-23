// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountGetModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2016. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts
{
    using System;

    /// <summary>
    /// The account get model.
    /// </summary>
    internal sealed class AccountGetModel
    {
        /// <summary>
        /// Gets or sets the account type.
        /// </summary>
        public AccountTypeModel Type { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the purchase date.
        /// </summary>
        public DateTimeOffset PurchaseDate { get; set; }

        /// <summary>
        /// Gets or sets the parent account id.
        /// </summary>
        public string ParentAccountID { get; set; }

        /// <summary>
        /// Gets or sets the account plan name.
        /// </summary>
        public string PlanName { get; set; }
    }
}