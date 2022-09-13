/*
 * TMT - Työpaikkailmoituksien hakurajapinta | Get Job postings
 *
 * <p>Tämän rajapinnan avulla voit hakea kaikki Työmarkkinatorilla julkaistut työpaikkailmoitukset.</p> <p>With this rest interface, you can retrieve all Job postings published in Job market Finland.</p>
 *
 * The version of the OpenAPI document: v1
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
using OpenAPIDateConverter = CodeGen.Api.TMT.Client.OpenAPIDateConverter;

namespace CodeGen.Api.TMT.Model
{
    /// <summary>
    /// **fi:** Rajapinnan palauttama virheviesti | **en:** Error response for request
    /// </summary>
    [DataContract(Name = "Virhe")]
    public partial class Virhe : IEquatable<Virhe>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Virhe" /> class.
        /// </summary>
        /// <param name="aikaleima">**fi:** Virheen aikaleima | **en:** Timestamp of error.</param>
        /// <param name="status">**fi:** Http status koodi | **en:** Http status code.</param>
        /// <param name="virheviesti">**fi:** Virhettä kuvaava viesti | **en:** General error message.</param>
        /// <param name="virheviestit">virheviestit.</param>
        public Virhe(DateTime aikaleima = default(DateTime), int status = default(int), string virheviesti = default(string), List<VirheViesti> virheviestit = default(List<VirheViesti>))
        {
            this.Aikaleima = aikaleima;
            this.Status = status;
            this.Virheviesti = virheviesti;
            this.Virheviestit = virheviestit;
        }

        /// <summary>
        /// **fi:** Virheen aikaleima | **en:** Timestamp of error
        /// </summary>
        /// <value>**fi:** Virheen aikaleima | **en:** Timestamp of error</value>
        [DataMember(Name = "aikaleima", EmitDefaultValue = false)]
        public DateTime Aikaleima { get; set; }

        /// <summary>
        /// **fi:** Http status koodi | **en:** Http status code
        /// </summary>
        /// <value>**fi:** Http status koodi | **en:** Http status code</value>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public int Status { get; set; }

        /// <summary>
        /// **fi:** Virhettä kuvaava viesti | **en:** General error message
        /// </summary>
        /// <value>**fi:** Virhettä kuvaava viesti | **en:** General error message</value>
        [DataMember(Name = "virheviesti", EmitDefaultValue = false)]
        public string Virheviesti { get; set; }

        /// <summary>
        /// Gets or Sets Virheviestit
        /// </summary>
        [DataMember(Name = "virheviestit", EmitDefaultValue = false)]
        public List<VirheViesti> Virheviestit { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class Virhe {\n");
            sb.Append("  Aikaleima: ").Append(Aikaleima).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  Virheviesti: ").Append(Virheviesti).Append("\n");
            sb.Append("  Virheviestit: ").Append(Virheviestit).Append("\n");
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
            return this.Equals(input as Virhe);
        }

        /// <summary>
        /// Returns true if Virhe instances are equal
        /// </summary>
        /// <param name="input">Instance of Virhe to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Virhe input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.Aikaleima == input.Aikaleima ||
                    (this.Aikaleima != null &&
                    this.Aikaleima.Equals(input.Aikaleima))
                ) && 
                (
                    this.Status == input.Status ||
                    this.Status.Equals(input.Status)
                ) && 
                (
                    this.Virheviesti == input.Virheviesti ||
                    (this.Virheviesti != null &&
                    this.Virheviesti.Equals(input.Virheviesti))
                ) && 
                (
                    this.Virheviestit == input.Virheviestit ||
                    this.Virheviestit != null &&
                    input.Virheviestit != null &&
                    this.Virheviestit.SequenceEqual(input.Virheviestit)
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
                if (this.Aikaleima != null)
                {
                    hashCode = (hashCode * 59) + this.Aikaleima.GetHashCode();
                }
                hashCode = (hashCode * 59) + this.Status.GetHashCode();
                if (this.Virheviesti != null)
                {
                    hashCode = (hashCode * 59) + this.Virheviesti.GetHashCode();
                }
                if (this.Virheviestit != null)
                {
                    hashCode = (hashCode * 59) + this.Virheviestit.GetHashCode();
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
