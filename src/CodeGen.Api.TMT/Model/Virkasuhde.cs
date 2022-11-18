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
    /// **fi:** Virkasuhteen tiedot, jos palvelussuhteen tyyppi on &#39;*02*&#39; | **en:** Information of public service relationship, if type of relationship is &#39;*02*&#39;
    /// </summary>
    [DataContract(Name = "Virkasuhde")]
    public partial class Virkasuhde : IEquatable<Virkasuhde>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Virkasuhde" /> class.
        /// </summary>
        /// <param name="vuorotteluvapaanSijaisuus">**fi:** Vuorotteluvapaan sijaisuus | **en:** Substitution for alternate leave.</param>
        public Virkasuhde(bool vuorotteluvapaanSijaisuus = default(bool))
        {
            this.VuorotteluvapaanSijaisuus = vuorotteluvapaanSijaisuus;
        }

        /// <summary>
        /// **fi:** Vuorotteluvapaan sijaisuus | **en:** Substitution for alternate leave
        /// </summary>
        /// <value>**fi:** Vuorotteluvapaan sijaisuus | **en:** Substitution for alternate leave</value>
        [DataMember(Name = "vuorotteluvapaanSijaisuus", EmitDefaultValue = true)]
        public bool VuorotteluvapaanSijaisuus { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class Virkasuhde {\n");
            sb.Append("  VuorotteluvapaanSijaisuus: ").Append(VuorotteluvapaanSijaisuus).Append("\n");
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
            return this.Equals(input as Virkasuhde);
        }

        /// <summary>
        /// Returns true if Virkasuhde instances are equal
        /// </summary>
        /// <param name="input">Instance of Virkasuhde to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Virkasuhde input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.VuorotteluvapaanSijaisuus == input.VuorotteluvapaanSijaisuus ||
                    this.VuorotteluvapaanSijaisuus.Equals(input.VuorotteluvapaanSijaisuus)
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
                hashCode = (hashCode * 59) + this.VuorotteluvapaanSijaisuus.GetHashCode();
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