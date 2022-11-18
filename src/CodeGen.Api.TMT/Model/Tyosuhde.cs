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
    /// **fi:** Työsuhteen tiedot, jos palvelussuhteen tyyppi on &#39;*01*&#39; | **en:** Information of employment, if type of relationship is &#39;*01*&#39;
    /// </summary>
    [DataContract(Name = "Tyosuhde")]
    public partial class Tyosuhde : IEquatable<Tyosuhde>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tyosuhde" /> class.
        /// </summary>
        /// <param name="oppisopimus">**fi:** Oppisopimus| **en:** Apprenticeship.</param>
        /// <param name="rekrytointiToimeksiantaja">**fi:** Rekrytointitoimeksiannon antaneen yrityksen nimi | **en:** Name of recruitment commission company.</param>
        /// <param name="rekrytointiToimeksianto">**fi:** Rekrytointitoimeksianto | **en:** Recruitment commission.</param>
        /// <param name="tyoharjoittelu">**fi:** Työharjoittelu | **en:** Practical work training.</param>
        /// <param name="vuokratyo">**fi:** Vuokratyö | **en:** Temporary agency work.</param>
        /// <param name="vuokratyoToimeksiantaja">**fi:** Vuokratyön toimeksiantajan nimi | **en:** Name of temporary agency work company.</param>
        /// <param name="vuorotteluvapaanSijaisuus">**fi:** Vuorotteluvapaan sijaisuus | **en:** Substitution for alternate leave.</param>
        public Tyosuhde(bool oppisopimus = default(bool), string rekrytointiToimeksiantaja = default(string), bool rekrytointiToimeksianto = default(bool), bool tyoharjoittelu = default(bool), bool vuokratyo = default(bool), string vuokratyoToimeksiantaja = default(string), bool vuorotteluvapaanSijaisuus = default(bool))
        {
            this.Oppisopimus = oppisopimus;
            this.RekrytointiToimeksiantaja = rekrytointiToimeksiantaja;
            this.RekrytointiToimeksianto = rekrytointiToimeksianto;
            this.Tyoharjoittelu = tyoharjoittelu;
            this.Vuokratyo = vuokratyo;
            this.VuokratyoToimeksiantaja = vuokratyoToimeksiantaja;
            this.VuorotteluvapaanSijaisuus = vuorotteluvapaanSijaisuus;
        }

        /// <summary>
        /// **fi:** Oppisopimus| **en:** Apprenticeship
        /// </summary>
        /// <value>**fi:** Oppisopimus| **en:** Apprenticeship</value>
        [DataMember(Name = "oppisopimus", EmitDefaultValue = true)]
        public bool Oppisopimus { get; set; }

        /// <summary>
        /// **fi:** Rekrytointitoimeksiannon antaneen yrityksen nimi | **en:** Name of recruitment commission company
        /// </summary>
        /// <value>**fi:** Rekrytointitoimeksiannon antaneen yrityksen nimi | **en:** Name of recruitment commission company</value>
        [DataMember(Name = "rekrytointiToimeksiantaja", EmitDefaultValue = false)]
        public string RekrytointiToimeksiantaja { get; set; }

        /// <summary>
        /// **fi:** Rekrytointitoimeksianto | **en:** Recruitment commission
        /// </summary>
        /// <value>**fi:** Rekrytointitoimeksianto | **en:** Recruitment commission</value>
        [DataMember(Name = "rekrytointiToimeksianto", EmitDefaultValue = true)]
        public bool RekrytointiToimeksianto { get; set; }

        /// <summary>
        /// **fi:** Työharjoittelu | **en:** Practical work training
        /// </summary>
        /// <value>**fi:** Työharjoittelu | **en:** Practical work training</value>
        [DataMember(Name = "tyoharjoittelu", EmitDefaultValue = true)]
        public bool Tyoharjoittelu { get; set; }

        /// <summary>
        /// **fi:** Vuokratyö | **en:** Temporary agency work
        /// </summary>
        /// <value>**fi:** Vuokratyö | **en:** Temporary agency work</value>
        [DataMember(Name = "vuokratyo", EmitDefaultValue = true)]
        public bool Vuokratyo { get; set; }

        /// <summary>
        /// **fi:** Vuokratyön toimeksiantajan nimi | **en:** Name of temporary agency work company
        /// </summary>
        /// <value>**fi:** Vuokratyön toimeksiantajan nimi | **en:** Name of temporary agency work company</value>
        [DataMember(Name = "vuokratyoToimeksiantaja", EmitDefaultValue = false)]
        public string VuokratyoToimeksiantaja { get; set; }

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
            sb.Append("class Tyosuhde {\n");
            sb.Append("  Oppisopimus: ").Append(Oppisopimus).Append("\n");
            sb.Append("  RekrytointiToimeksiantaja: ").Append(RekrytointiToimeksiantaja).Append("\n");
            sb.Append("  RekrytointiToimeksianto: ").Append(RekrytointiToimeksianto).Append("\n");
            sb.Append("  Tyoharjoittelu: ").Append(Tyoharjoittelu).Append("\n");
            sb.Append("  Vuokratyo: ").Append(Vuokratyo).Append("\n");
            sb.Append("  VuokratyoToimeksiantaja: ").Append(VuokratyoToimeksiantaja).Append("\n");
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
            return this.Equals(input as Tyosuhde);
        }

        /// <summary>
        /// Returns true if Tyosuhde instances are equal
        /// </summary>
        /// <param name="input">Instance of Tyosuhde to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Tyosuhde input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.Oppisopimus == input.Oppisopimus ||
                    this.Oppisopimus.Equals(input.Oppisopimus)
                ) && 
                (
                    this.RekrytointiToimeksiantaja == input.RekrytointiToimeksiantaja ||
                    (this.RekrytointiToimeksiantaja != null &&
                    this.RekrytointiToimeksiantaja.Equals(input.RekrytointiToimeksiantaja))
                ) && 
                (
                    this.RekrytointiToimeksianto == input.RekrytointiToimeksianto ||
                    this.RekrytointiToimeksianto.Equals(input.RekrytointiToimeksianto)
                ) && 
                (
                    this.Tyoharjoittelu == input.Tyoharjoittelu ||
                    this.Tyoharjoittelu.Equals(input.Tyoharjoittelu)
                ) && 
                (
                    this.Vuokratyo == input.Vuokratyo ||
                    this.Vuokratyo.Equals(input.Vuokratyo)
                ) && 
                (
                    this.VuokratyoToimeksiantaja == input.VuokratyoToimeksiantaja ||
                    (this.VuokratyoToimeksiantaja != null &&
                    this.VuokratyoToimeksiantaja.Equals(input.VuokratyoToimeksiantaja))
                ) && 
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
                hashCode = (hashCode * 59) + this.Oppisopimus.GetHashCode();
                if (this.RekrytointiToimeksiantaja != null)
                {
                    hashCode = (hashCode * 59) + this.RekrytointiToimeksiantaja.GetHashCode();
                }
                hashCode = (hashCode * 59) + this.RekrytointiToimeksianto.GetHashCode();
                hashCode = (hashCode * 59) + this.Tyoharjoittelu.GetHashCode();
                hashCode = (hashCode * 59) + this.Vuokratyo.GetHashCode();
                if (this.VuokratyoToimeksiantaja != null)
                {
                    hashCode = (hashCode * 59) + this.VuokratyoToimeksiantaja.GetHashCode();
                }
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