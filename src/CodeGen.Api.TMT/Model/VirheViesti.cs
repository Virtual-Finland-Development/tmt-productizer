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
    /// **fi:** Virhettä koskeva tarkempi kenttä tai syöte | **en:** More detailed field or input about the error
    /// </summary>
    [DataContract(Name = "VirheViesti")]
    public partial class VirheViesti : IEquatable<VirheViesti>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirheViesti" /> class.
        /// </summary>
        /// <param name="arvo">**fi:** Rajapintaan annettu syöte | **en:** The input entered in the field.</param>
        /// <param name="kentt">**fi:** Virheellinen kenttä | **en:** Invalid field.</param>
        /// <param name="virhe">**fi:** Kuvaus | **en:** Description.</param>
        public VirheViesti(Object arvo = default(Object), string kentt = default(string), string virhe = default(string))
        {
            this.Arvo = arvo;
            this.Kentt = kentt;
            this.Virhe = virhe;
        }

        /// <summary>
        /// **fi:** Rajapintaan annettu syöte | **en:** The input entered in the field
        /// </summary>
        /// <value>**fi:** Rajapintaan annettu syöte | **en:** The input entered in the field</value>
        [DataMember(Name = "arvo", EmitDefaultValue = false)]
        public Object Arvo { get; set; }

        /// <summary>
        /// **fi:** Virheellinen kenttä | **en:** Invalid field
        /// </summary>
        /// <value>**fi:** Virheellinen kenttä | **en:** Invalid field</value>
        [DataMember(Name = "kenttä", EmitDefaultValue = false)]
        public string Kentt { get; set; }

        /// <summary>
        /// **fi:** Kuvaus | **en:** Description
        /// </summary>
        /// <value>**fi:** Kuvaus | **en:** Description</value>
        [DataMember(Name = "virhe", EmitDefaultValue = false)]
        public string Virhe { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class VirheViesti {\n");
            sb.Append("  Arvo: ").Append(Arvo).Append("\n");
            sb.Append("  Kentt: ").Append(Kentt).Append("\n");
            sb.Append("  Virhe: ").Append(Virhe).Append("\n");
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
            return this.Equals(input as VirheViesti);
        }

        /// <summary>
        /// Returns true if VirheViesti instances are equal
        /// </summary>
        /// <param name="input">Instance of VirheViesti to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(VirheViesti input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.Arvo == input.Arvo ||
                    (this.Arvo != null &&
                    this.Arvo.Equals(input.Arvo))
                ) && 
                (
                    this.Kentt == input.Kentt ||
                    (this.Kentt != null &&
                    this.Kentt.Equals(input.Kentt))
                ) && 
                (
                    this.Virhe == input.Virhe ||
                    (this.Virhe != null &&
                    this.Virhe.Equals(input.Virhe))
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
                if (this.Arvo != null)
                {
                    hashCode = (hashCode * 59) + this.Arvo.GetHashCode();
                }
                if (this.Kentt != null)
                {
                    hashCode = (hashCode * 59) + this.Kentt.GetHashCode();
                }
                if (this.Virhe != null)
                {
                    hashCode = (hashCode * 59) + this.Virhe.GetHashCode();
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
