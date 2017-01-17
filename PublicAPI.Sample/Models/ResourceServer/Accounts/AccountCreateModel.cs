// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountCreateModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts
{
    using Company;
    using Payment;

    /// <summary>
    /// The account create model.
    /// </summary>
    internal sealed class AccountCreateModel
    {
        /// <summary>
        /// Gets or sets the account programs.
        /// </summary>
        public AccountProgramModel[] Programs { get; set; }

        /// <summary>
        /// Gets or sets the account general data.
        /// </summary>
        public AccountGeneralModel General { get; set; }

        /// <summary>
        /// Gets or sets the account company data.
        /// </summary>
        public CompanyModel Company { get; set; }

        /// <summary>
        /// Gets or sets the account payment data.
        /// </summary>
        public PaymentModel Payment { get; set; }

        /// <summary>
        /// Gets or sets the account plan name.
        /// </summary>
        public string PlanName { get; set; }
    }
}