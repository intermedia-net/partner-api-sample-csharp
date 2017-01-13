// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagingModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer
{
    /// <summary>
    /// The paging model.
    /// </summary>
    internal class PagingModel
    {
        /// <summary>
        /// Gets or sets the item count to skip.
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the item count to take.
        /// </summary>
        public int? Take { get; set; }
    }
}
