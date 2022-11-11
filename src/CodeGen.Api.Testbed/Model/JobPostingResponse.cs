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
    /// JobPostingResponse
    /// </summary>
    [DataContract(Name = "JobPostingResponse")]
    public partial class JobPostingResponse : IEquatable<JobPostingResponse>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobPostingResponse" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected JobPostingResponse() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JobPostingResponse" /> class.
        /// </summary>
        /// <param name="results">results (required).</param>
        /// <param name="totalCount">Total count of job postings (required).</param>
        public JobPostingResponse(List<JobPosting> results = default(List<JobPosting>), int totalCount = default(int))
        {
            // to ensure "results" is required (not null)
            if (results == null)
            {
                throw new ArgumentNullException("results is a required property for JobPostingResponse and cannot be null");
            }
            this.Results = results;
            this.TotalCount = totalCount;
        }

        /// <summary>
        /// Gets or Sets Results
        /// </summary>
        [DataMember(Name = "results", IsRequired = true, EmitDefaultValue = true)]
        public List<JobPosting> Results { get; set; }

        /// <summary>
        /// Total count of job postings
        /// </summary>
        /// <value>Total count of job postings</value>
        [DataMember(Name = "totalCount", IsRequired = true, EmitDefaultValue = true)]
        public int TotalCount { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class JobPostingResponse {\n");
            sb.Append("  Results: ").Append(Results).Append("\n");
            sb.Append("  TotalCount: ").Append(TotalCount).Append("\n");
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
            return this.Equals(input as JobPostingResponse);
        }

        /// <summary>
        /// Returns true if JobPostingResponse instances are equal
        /// </summary>
        /// <param name="input">Instance of JobPostingResponse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(JobPostingResponse input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.Results == input.Results ||
                    this.Results != null &&
                    input.Results != null &&
                    this.Results.SequenceEqual(input.Results)
                ) && 
                (
                    this.TotalCount == input.TotalCount ||
                    this.TotalCount.Equals(input.TotalCount)
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
                if (this.Results != null)
                {
                    hashCode = (hashCode * 59) + this.Results.GetHashCode();
                }
                hashCode = (hashCode * 59) + this.TotalCount.GetHashCode();
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
