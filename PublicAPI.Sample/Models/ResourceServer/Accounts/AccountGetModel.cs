// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountGetModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
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
        /// Gets or sets the account programs.
        /// </summary>
        public AccountProgramModel[] Programs { get; set; }

        /// <summary>
        /// Gets or sets the customer id.
        /// </summary>
        public string CustomerID { get; set; }

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