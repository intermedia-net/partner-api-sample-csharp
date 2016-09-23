/* 
 * REST API v1
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: v1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Hosting.PublicAPI.Sample.Generated.Models.Accounts
{
    /// <summary>
    /// The account&#39;s leaving model.
    /// </summary>
    [DataContract]
    public partial class AccountLeavingV1Model :  IEquatable<AccountLeavingV1Model>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountLeavingV1Model" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected AccountLeavingV1Model() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountLeavingV1Model" /> class.
        /// </summary>
        /// <param name="Reason">Gets or sets the account&#39;s leaving reason. (required).</param>
        /// <param name="Comments">Gets or sets the account&#39;s leaving comments..</param>
        public AccountLeavingV1Model(string Reason = null, string Comments = null)
        {
            // to ensure "Reason" is required (not null)
            if (Reason == null)
            {
                throw new InvalidDataException("Reason is a required property for AccountLeavingV1Model and cannot be null");
            }
            else
            {
                this.Reason = Reason;
            }
            this.Comments = Comments;
        }
        
        /// <summary>
        /// Gets or sets the account&#39;s leaving reason.
        /// </summary>
        /// <value>Gets or sets the account&#39;s leaving reason.</value>
        [DataMember(Name="reason", EmitDefaultValue=false)]
        public string Reason { get; set; }
        /// <summary>
        /// Gets or sets the account&#39;s leaving comments.
        /// </summary>
        /// <value>Gets or sets the account&#39;s leaving comments.</value>
        [DataMember(Name="comments", EmitDefaultValue=false)]
        public string Comments { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AccountLeavingV1Model {\n");
            sb.Append("  Reason: ").Append(Reason).Append("\n");
            sb.Append("  Comments: ").Append(Comments).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            return this.Equals(obj as AccountLeavingV1Model);
        }

        /// <summary>
        /// Returns true if AccountLeavingV1Model instances are equal
        /// </summary>
        /// <param name="other">Instance of AccountLeavingV1Model to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AccountLeavingV1Model other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.Reason == other.Reason ||
                    this.Reason != null &&
                    this.Reason.Equals(other.Reason)
                ) && 
                (
                    this.Comments == other.Comments ||
                    this.Comments != null &&
                    this.Comments.Equals(other.Comments)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                if (this.Reason != null)
                    hash = hash * 59 + this.Reason.GetHashCode();
                if (this.Comments != null)
                    hash = hash * 59 + this.Comments.GetHashCode();
                return hash;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        { 
            // Reason (string) maxLength
            if(this.Reason != null && this.Reason.Length > 512)
            {
                yield return new ValidationResult("Invalid value for Reason, length must be less than 512.", new [] { "Reason" });
            }

            // Reason (string) minLength
            if(this.Reason != null && this.Reason.Length < 0)
            {
                yield return new ValidationResult("Invalid value for Reason, length must be greater than 0.", new [] { "Reason" });
            }

            // Comments (string) maxLength
            if(this.Comments != null && this.Comments.Length > 512)
            {
                yield return new ValidationResult("Invalid value for Comments, length must be less than 512.", new [] { "Comments" });
            }

            // Comments (string) minLength
            if(this.Comments != null && this.Comments.Length < 0)
            {
                yield return new ValidationResult("Invalid value for Comments, length must be greater than 0.", new [] { "Comments" });
            }

            yield break;
        }
    }

}
