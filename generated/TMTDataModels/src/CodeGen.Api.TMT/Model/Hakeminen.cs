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
    /// **fi:** Työpaikkaan hakeminen | **en:** Applying for the job
    /// </summary>
    [DataContract(Name = "Hakeminen")]
    public partial class Hakeminen : IEquatable<Hakeminen>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hakeminen" /> class.
        /// </summary>
        /// <param name="hakemuksenUrl">**fi:** Linkki hakulomakkeelle | **en:** Link to application form.</param>
        /// <param name="hakuaikaPaattyy">**fi:** Hakuaika päättyy | **en:** Application end date.</param>
        /// <param name="hakuohjeet">**fi:** Hakuohjeet | **en:** Application instructions.</param>
        /// <param name="ilmoittajanYhteystiedot">ilmoittajanYhteystiedot.</param>
        public Hakeminen(string hakemuksenUrl = default(string), DateTime hakuaikaPaattyy = default(DateTime), List<LokalisoituArvo> hakuohjeet = default(List<LokalisoituArvo>), IlmoittajanYhteystieto ilmoittajanYhteystiedot = default(IlmoittajanYhteystieto))
        {
            this.HakemuksenUrl = hakemuksenUrl;
            this.HakuaikaPaattyy = hakuaikaPaattyy;
            this.Hakuohjeet = hakuohjeet;
            this.IlmoittajanYhteystiedot = ilmoittajanYhteystiedot;
        }

        /// <summary>
        /// **fi:** Linkki hakulomakkeelle | **en:** Link to application form
        /// </summary>
        /// <value>**fi:** Linkki hakulomakkeelle | **en:** Link to application form</value>
        [DataMember(Name = "hakemuksenUrl", EmitDefaultValue = false)]
        public string HakemuksenUrl { get; set; }

        /// <summary>
        /// **fi:** Hakuaika päättyy | **en:** Application end date
        /// </summary>
        /// <value>**fi:** Hakuaika päättyy | **en:** Application end date</value>
        [DataMember(Name = "hakuaikaPaattyy", EmitDefaultValue = false)]
        public DateTime HakuaikaPaattyy { get; set; }

        /// <summary>
        /// **fi:** Hakuohjeet | **en:** Application instructions
        /// </summary>
        /// <value>**fi:** Hakuohjeet | **en:** Application instructions</value>
        [DataMember(Name = "hakuohjeet", EmitDefaultValue = false)]
        public List<LokalisoituArvo> Hakuohjeet { get; set; }

        /// <summary>
        /// Gets or Sets IlmoittajanYhteystiedot
        /// </summary>
        [DataMember(Name = "ilmoittajanYhteystiedot", EmitDefaultValue = false)]
        public IlmoittajanYhteystieto IlmoittajanYhteystiedot { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class Hakeminen {\n");
            sb.Append("  HakemuksenUrl: ").Append(HakemuksenUrl).Append("\n");
            sb.Append("  HakuaikaPaattyy: ").Append(HakuaikaPaattyy).Append("\n");
            sb.Append("  Hakuohjeet: ").Append(Hakuohjeet).Append("\n");
            sb.Append("  IlmoittajanYhteystiedot: ").Append(IlmoittajanYhteystiedot).Append("\n");
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
            return this.Equals(input as Hakeminen);
        }

        /// <summary>
        /// Returns true if Hakeminen instances are equal
        /// </summary>
        /// <param name="input">Instance of Hakeminen to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Hakeminen input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.HakemuksenUrl == input.HakemuksenUrl ||
                    (this.HakemuksenUrl != null &&
                    this.HakemuksenUrl.Equals(input.HakemuksenUrl))
                ) && 
                (
                    this.HakuaikaPaattyy == input.HakuaikaPaattyy ||
                    (this.HakuaikaPaattyy != null &&
                    this.HakuaikaPaattyy.Equals(input.HakuaikaPaattyy))
                ) && 
                (
                    this.Hakuohjeet == input.Hakuohjeet ||
                    this.Hakuohjeet != null &&
                    input.Hakuohjeet != null &&
                    this.Hakuohjeet.SequenceEqual(input.Hakuohjeet)
                ) && 
                (
                    this.IlmoittajanYhteystiedot == input.IlmoittajanYhteystiedot ||
                    (this.IlmoittajanYhteystiedot != null &&
                    this.IlmoittajanYhteystiedot.Equals(input.IlmoittajanYhteystiedot))
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
                if (this.HakemuksenUrl != null)
                {
                    hashCode = (hashCode * 59) + this.HakemuksenUrl.GetHashCode();
                }
                if (this.HakuaikaPaattyy != null)
                {
                    hashCode = (hashCode * 59) + this.HakuaikaPaattyy.GetHashCode();
                }
                if (this.Hakuohjeet != null)
                {
                    hashCode = (hashCode * 59) + this.Hakuohjeet.GetHashCode();
                }
                if (this.IlmoittajanYhteystiedot != null)
                {
                    hashCode = (hashCode * 59) + this.IlmoittajanYhteystiedot.GetHashCode();
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
