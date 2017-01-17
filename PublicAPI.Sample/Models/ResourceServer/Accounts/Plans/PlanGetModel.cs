// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlanGetModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Plans
{
    /// <summary>
    /// The account plan get model.
    /// </summary>
    internal sealed class PlanGetModel : PlanUpdateModel
    {
        /// <summary>
        /// Gets or sets the plan pretty name.
        /// </summary>
        public string PrettyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether plan is current.
        /// </summary>
        public bool? IsCurrent { get; set; }
    }
}
