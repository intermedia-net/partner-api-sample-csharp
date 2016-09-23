// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2016. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer
{
    /// <summary>
    /// The page model.
    /// </summary>
    /// <typeparam name="TItem">
    /// The item type.
    /// </typeparam>
    internal sealed class PageModel<TItem>
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public TItem[] Items { get; set; }
    }
}
