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
    /// **fi:** Kielitaito | **en:** Language skill
    /// </summary>
    [DataContract(Name = "Kielitaito")]
    public partial class Kielitaito : IEquatable<Kielitaito>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Kielitaito" /> class.
        /// </summary>
        /// <param name="kielitaidonLisatieto">**fi:** Kielitaidon lisätieto | **en:** Additional information for language.</param>
        /// <param name="kielitaidonTaso">**fi:** Kielitaidon taso (CERF) | **en:** Language skills level (CERF)&lt;details&gt;&lt;summary&gt;Koodit | Codes&lt;/summary&gt;&lt;pre&gt;A1 &#x3D; Alkeet | Basics B1 &#x3D; Tyydyttävä | Fair B2 &#x3D; Hyvä  | Good C1 &#x3D; Erittäin hyvä | Very good L1 &#x3D; Äidinkieli | Mother tongue&lt;/pre&gt;&lt;/details&gt;.</param>
        /// <param name="kielitaito">**fi:** Kielitaito - arvot on ESCO URI muodossa | **en:** ESCO uri of language skill.</param>
        public Kielitaito(List<LokalisoituArvo> kielitaidonLisatieto = default(List<LokalisoituArvo>), string kielitaidonTaso = default(string), string kielitaito = default(string))
        {
            this.KielitaidonLisatieto = kielitaidonLisatieto;
            this.KielitaidonTaso = kielitaidonTaso;
            this._Kielitaito = kielitaito;
        }

        public Kielitaito()
        {
        }

        /// <summary>
        /// **fi:** Kielitaidon lisätieto | **en:** Additional information for language
        /// </summary>
        /// <value>**fi:** Kielitaidon lisätieto | **en:** Additional information for language</value>
        [DataMember(Name = "kielitaidonLisatieto", EmitDefaultValue = false)]
        public List<LokalisoituArvo> KielitaidonLisatieto { get; set; }

        /// <summary>
        /// **fi:** Kielitaidon taso (CERF) | **en:** Language skills level (CERF)&lt;details&gt;&lt;summary&gt;Koodit | Codes&lt;/summary&gt;&lt;pre&gt;A1 &#x3D; Alkeet | Basics B1 &#x3D; Tyydyttävä | Fair B2 &#x3D; Hyvä  | Good C1 &#x3D; Erittäin hyvä | Very good L1 &#x3D; Äidinkieli | Mother tongue&lt;/pre&gt;&lt;/details&gt;
        /// </summary>
        /// <value>**fi:** Kielitaidon taso (CERF) | **en:** Language skills level (CERF)&lt;details&gt;&lt;summary&gt;Koodit | Codes&lt;/summary&gt;&lt;pre&gt;A1 &#x3D; Alkeet | Basics B1 &#x3D; Tyydyttävä | Fair B2 &#x3D; Hyvä  | Good C1 &#x3D; Erittäin hyvä | Very good L1 &#x3D; Äidinkieli | Mother tongue&lt;/pre&gt;&lt;/details&gt;</value>
        [DataMember(Name = "kielitaidonTaso", EmitDefaultValue = false)]
        public string KielitaidonTaso { get; set; }

        /// <summary>
        /// **fi:** Kielitaito - arvot on ESCO URI muodossa | **en:** ESCO uri of language skill
        /// </summary>
        /// <value>**fi:** Kielitaito - arvot on ESCO URI muodossa | **en:** ESCO uri of language skill</value>
        [DataMember(Name = "kielitaito", EmitDefaultValue = false)]
        public string _Kielitaito { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class Kielitaito {\n");
            sb.Append("  KielitaidonLisatieto: ").Append(KielitaidonLisatieto).Append("\n");
            sb.Append("  KielitaidonTaso: ").Append(KielitaidonTaso).Append("\n");
            sb.Append("  _Kielitaito: ").Append(_Kielitaito).Append("\n");
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
            return this.Equals(input as Kielitaito);
        }

        /// <summary>
        /// Returns true if Kielitaito instances are equal
        /// </summary>
        /// <param name="input">Instance of Kielitaito to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Kielitaito input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.KielitaidonLisatieto == input.KielitaidonLisatieto ||
                    this.KielitaidonLisatieto != null &&
                    input.KielitaidonLisatieto != null &&
                    this.KielitaidonLisatieto.SequenceEqual(input.KielitaidonLisatieto)
                ) && 
                (
                    this.KielitaidonTaso == input.KielitaidonTaso ||
                    (this.KielitaidonTaso != null &&
                    this.KielitaidonTaso.Equals(input.KielitaidonTaso))
                ) && 
                (
                    this._Kielitaito == input._Kielitaito ||
                    (this._Kielitaito != null &&
                    this._Kielitaito.Equals(input._Kielitaito))
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
                if (this.KielitaidonLisatieto != null)
                {
                    hashCode = (hashCode * 59) + this.KielitaidonLisatieto.GetHashCode();
                }
                if (this.KielitaidonTaso != null)
                {
                    hashCode = (hashCode * 59) + this.KielitaidonTaso.GetHashCode();
                }
                if (this._Kielitaito != null)
                {
                    hashCode = (hashCode * 59) + this._Kielitaito.GetHashCode();
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
