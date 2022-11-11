/*
 * Job Posting
 *
 * Data product for Job Posting
 *
 * The version of the OpenAPI document: 1.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = CodeGen.Api.Testbed.Client.OpenAPIDateConverter;

namespace CodeGen.Api.Testbed.Model
{
    /// <summary>
    /// JobPostingLocation
    /// </summary>
    [DataContract(Name = "JobPosting_location")]
    public partial class JobPostingLocation : IEquatable<JobPostingLocation>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobPostingLocation" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected JobPostingLocation() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JobPostingLocation" /> class.
        /// </summary>
        /// <param name="municipality">municipality (required).</param>
        /// <param name="postcode">postcode.</param>
        public JobPostingLocation(string municipality = default(string), string postcode = default(string))
        {
            // to ensure "municipality" is required (not null)
            if (municipality == null)
            {
                throw new ArgumentNullException("municipality is a required property for JobPostingLocation and cannot be null");
            }
            this.Municipality = municipality;
            this.Postcode = postcode;
        }

        /// <summary>
        /// Gets or Sets Municipality
        /// </summary>
        [DataMember(Name = "municipality", IsRequired = true, EmitDefaultValue = true)]
        public string Municipality { get; set; }

        /// <summary>
        /// Gets or Sets Postcode
        /// </summary>
        [DataMember(Name = "postcode", EmitDefaultValue = false)]
        public string Postcode { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class JobPostingLocation {\n");
            sb.Append("  Municipality: ").Append(Municipality).Append("\n");
            sb.Append("  Postcode: ").Append(Postcode).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as JobPostingLocation);
        }

        /// <summary>
        /// Returns true if JobPostingLocation instances are equal
        /// </summary>
        /// <param name="input">Instance of JobPostingLocation to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(JobPostingLocation input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.Municipality == input.Municipality ||
                    (this.Municipality != null &&
                    this.Municipality.Equals(input.Municipality))
                ) && 
                (
                    this.Postcode == input.Postcode ||
                    (this.Postcode != null &&
                    this.Postcode.Equals(input.Postcode))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Municipality != null)
                {
                    hashCode = (hashCode * 59) + this.Municipality.GetHashCode();
                }
                if (this.Postcode != null)
                {
                    hashCode = (hashCode * 59) + this.Postcode.GetHashCode();
                }
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        public IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
