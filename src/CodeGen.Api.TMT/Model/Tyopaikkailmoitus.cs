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
    /// **fi:** Työpaikkailmoitus | **en:** Job posting
    /// </summary>
    [DataContract(Name = "Tyopaikkailmoitus")]
    public partial class Tyopaikkailmoitus : IEquatable<Tyopaikkailmoitus>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tyopaikkailmoitus" /> class.
        /// </summary>
        /// <param name="hakeminen">hakeminen.</param>
        /// <param name="ilmoittajanNimi">**fi:** Työpaikkailmoituksen ilmoittajan nimi, kentässä on arvo vain jos tyyppi on &#39;*01*&#39; | **en:** Name of the company that owns the job posting (field has value only if job posting type is &#39;**01**&#39;).</param>
        /// <param name="ilmoittajanYTunnus">**fi:** Työpaikkailmoituksen tehneen yrityksen y-tunnus, kentässä on arvo vain jos tyyppi on &#39;*01*&#39; | **en:** Business ID of the company that owns the job posting (field has value only if job posting type is &#39;*01*&#39;).</param>
        /// <param name="ilmoituksenID">**fi:** Työpaikkailmoituksen tunniste | **en:** Unique ID of the job posting.</param>
        /// <param name="ilmoituksenKielet">ilmoituksenKielet.</param>
        /// <param name="ilmoituksenOhjaus">**fi:** Ilmoituksen ohjaus hakulomakkeeseen Työmarkkinatorin ulkopuolelle | **en:** Redirection to application form outside Job Market Finland.</param>
        /// <param name="julkaisupvm">**fi:** Julkaistuaikaleima | **en:** Published timestamp.</param>
        /// <param name="kotisivut">**fi:** Yrityksen kotisivu, kentässä on arvo vain jos tyyppi on &#39;**01**&#39; | **en:** Homepage of company (field has value only if job posting type is &#39;*01*&#39;).</param>
        /// <param name="kotitaloudenNimi">**fi:** Työpaikkailmoituksen ilmoittajan nimi, kentässä on arvo vain jos tyyppi on &#39;*02*&#39; | **en:** Name of the household that owns the job posting (field has value only if job posting type is &#39;*02*&#39;).</param>
        /// <param name="luontipvm">**fi:** Luontiaikaleima | **en:** Created timestamp.</param>
        /// <param name="markkinointikuvaus">**fi:** Työpaikkailmoitukseen lisätty yrityksen yleinen markkinointikuvaus, kentässä on arvo vain jos tyyppi on &#39;*01*&#39; | **en:** Marketing description of company (field has value only if job posting type is &#39;*01*&#39;).</param>
        /// <param name="muokattupvm">**fi:** Muokattu viimeksi | **en:** Last modified timestamp.</param>
        /// <param name="osaamisvaatimukset">osaamisvaatimukset.</param>
        /// <param name="perustiedot">perustiedot.</param>
        /// <param name="sijainti">sijainti.</param>
        /// <param name="tyollistaja">**fi:** Työpaikkailmoituksen tyyppi | **en:** Type of the job posting&lt;details&gt;&lt;summary&gt;Koodit | Codes&lt;/summary&gt;&lt;pre&gt;01 &#x3D; Yritys | Organization  02 &#x3D; Kotitalous | Household&lt;/pre&gt;&lt;/details&gt;.</param>
        public Tyopaikkailmoitus(Hakeminen hakeminen = default(Hakeminen), List<LokalisoituArvo> ilmoittajanNimi = default(List<LokalisoituArvo>), string ilmoittajanYTunnus = default(string), string ilmoituksenID = default(string), List<string> ilmoituksenKielet = default(List<string>), bool ilmoituksenOhjaus = default(bool), DateTime julkaisupvm = default(DateTime), string kotisivut = default(string), string kotitaloudenNimi = default(string), DateTime luontipvm = default(DateTime), List<LokalisoituArvo> markkinointikuvaus = default(List<LokalisoituArvo>), DateTime muokattupvm = default(DateTime), Osaamisvaatimukset osaamisvaatimukset = default(Osaamisvaatimukset), Perustiedot perustiedot = default(Perustiedot), Sijainti sijainti = default(Sijainti), string tyollistaja = default(string))
        {
            this.Hakeminen = hakeminen;
            this.IlmoittajanNimi = ilmoittajanNimi;
            this.IlmoittajanYTunnus = ilmoittajanYTunnus;
            this.IlmoituksenID = ilmoituksenID;
            this.IlmoituksenKielet = ilmoituksenKielet;
            this.IlmoituksenOhjaus = ilmoituksenOhjaus;
            this.Julkaisupvm = julkaisupvm;
            this.Kotisivut = kotisivut;
            this.KotitaloudenNimi = kotitaloudenNimi;
            this.Luontipvm = luontipvm;
            this.Markkinointikuvaus = markkinointikuvaus;
            this.Muokattupvm = muokattupvm;
            this.Osaamisvaatimukset = osaamisvaatimukset;
            this.Perustiedot = perustiedot;
            this.Sijainti = sijainti;
            this.Tyollistaja = tyollistaja;
        }

        /// <summary>
        /// Gets or Sets Hakeminen
        /// </summary>
        [DataMember(Name = "hakeminen", EmitDefaultValue = false)]
        public Hakeminen Hakeminen { get; set; }

        /// <summary>
        /// **fi:** Työpaikkailmoituksen ilmoittajan nimi, kentässä on arvo vain jos tyyppi on &#39;*01*&#39; | **en:** Name of the company that owns the job posting (field has value only if job posting type is &#39;**01**&#39;)
        /// </summary>
        /// <value>**fi:** Työpaikkailmoituksen ilmoittajan nimi, kentässä on arvo vain jos tyyppi on &#39;*01*&#39; | **en:** Name of the company that owns the job posting (field has value only if job posting type is &#39;**01**&#39;)</value>
        [DataMember(Name = "ilmoittajanNimi", EmitDefaultValue = false)]
        public List<LokalisoituArvo> IlmoittajanNimi { get; set; }

        /// <summary>
        /// **fi:** Työpaikkailmoituksen tehneen yrityksen y-tunnus, kentässä on arvo vain jos tyyppi on &#39;*01*&#39; | **en:** Business ID of the company that owns the job posting (field has value only if job posting type is &#39;*01*&#39;)
        /// </summary>
        /// <value>**fi:** Työpaikkailmoituksen tehneen yrityksen y-tunnus, kentässä on arvo vain jos tyyppi on &#39;*01*&#39; | **en:** Business ID of the company that owns the job posting (field has value only if job posting type is &#39;*01*&#39;)</value>
        [DataMember(Name = "ilmoittajanYTunnus", EmitDefaultValue = false)]
        public string IlmoittajanYTunnus { get; set; }

        /// <summary>
        /// **fi:** Työpaikkailmoituksen tunniste | **en:** Unique ID of the job posting
        /// </summary>
        /// <value>**fi:** Työpaikkailmoituksen tunniste | **en:** Unique ID of the job posting</value>
        [DataMember(Name = "ilmoituksenID", EmitDefaultValue = false)]
        public string IlmoituksenID { get; set; }

        /// <summary>
        /// Gets or Sets IlmoituksenKielet
        /// </summary>
        [DataMember(Name = "ilmoituksenKielet", EmitDefaultValue = false)]
        public List<string> IlmoituksenKielet { get; set; }

        /// <summary>
        /// **fi:** Ilmoituksen ohjaus hakulomakkeeseen Työmarkkinatorin ulkopuolelle | **en:** Redirection to application form outside Job Market Finland
        /// </summary>
        /// <value>**fi:** Ilmoituksen ohjaus hakulomakkeeseen Työmarkkinatorin ulkopuolelle | **en:** Redirection to application form outside Job Market Finland</value>
        [DataMember(Name = "ilmoituksenOhjaus", EmitDefaultValue = true)]
        public bool IlmoituksenOhjaus { get; set; }

        /// <summary>
        /// **fi:** Julkaistuaikaleima | **en:** Published timestamp
        /// </summary>
        /// <value>**fi:** Julkaistuaikaleima | **en:** Published timestamp</value>
        [DataMember(Name = "julkaisupvm", EmitDefaultValue = false)]
        public DateTime Julkaisupvm { get; set; }

        /// <summary>
        /// **fi:** Yrityksen kotisivu, kentässä on arvo vain jos tyyppi on &#39;**01**&#39; | **en:** Homepage of company (field has value only if job posting type is &#39;*01*&#39;)
        /// </summary>
        /// <value>**fi:** Yrityksen kotisivu, kentässä on arvo vain jos tyyppi on &#39;**01**&#39; | **en:** Homepage of company (field has value only if job posting type is &#39;*01*&#39;)</value>
        [DataMember(Name = "kotisivut", EmitDefaultValue = false)]
        public string Kotisivut { get; set; }

        /// <summary>
        /// **fi:** Työpaikkailmoituksen ilmoittajan nimi, kentässä on arvo vain jos tyyppi on &#39;*02*&#39; | **en:** Name of the household that owns the job posting (field has value only if job posting type is &#39;*02*&#39;)
        /// </summary>
        /// <value>**fi:** Työpaikkailmoituksen ilmoittajan nimi, kentässä on arvo vain jos tyyppi on &#39;*02*&#39; | **en:** Name of the household that owns the job posting (field has value only if job posting type is &#39;*02*&#39;)</value>
        [DataMember(Name = "kotitaloudenNimi", EmitDefaultValue = false)]
        public string KotitaloudenNimi { get; set; }

        /// <summary>
        /// **fi:** Luontiaikaleima | **en:** Created timestamp
        /// </summary>
        /// <value>**fi:** Luontiaikaleima | **en:** Created timestamp</value>
        [DataMember(Name = "luontipvm", EmitDefaultValue = false)]
        public DateTime Luontipvm { get; set; }

        /// <summary>
        /// **fi:** Työpaikkailmoitukseen lisätty yrityksen yleinen markkinointikuvaus, kentässä on arvo vain jos tyyppi on &#39;*01*&#39; | **en:** Marketing description of company (field has value only if job posting type is &#39;*01*&#39;)
        /// </summary>
        /// <value>**fi:** Työpaikkailmoitukseen lisätty yrityksen yleinen markkinointikuvaus, kentässä on arvo vain jos tyyppi on &#39;*01*&#39; | **en:** Marketing description of company (field has value only if job posting type is &#39;*01*&#39;)</value>
        [DataMember(Name = "markkinointikuvaus", EmitDefaultValue = false)]
        public List<LokalisoituArvo> Markkinointikuvaus { get; set; }

        /// <summary>
        /// **fi:** Muokattu viimeksi | **en:** Last modified timestamp
        /// </summary>
        /// <value>**fi:** Muokattu viimeksi | **en:** Last modified timestamp</value>
        [DataMember(Name = "muokattupvm", EmitDefaultValue = false)]
        public DateTime Muokattupvm { get; set; }

        /// <summary>
        /// Gets or Sets Osaamisvaatimukset
        /// </summary>
        [DataMember(Name = "osaamisvaatimukset", EmitDefaultValue = false)]
        public Osaamisvaatimukset Osaamisvaatimukset { get; set; }

        /// <summary>
        /// Gets or Sets Perustiedot
        /// </summary>
        [DataMember(Name = "perustiedot", EmitDefaultValue = false)]
        public Perustiedot Perustiedot { get; set; }

        /// <summary>
        /// Gets or Sets Sijainti
        /// </summary>
        [DataMember(Name = "sijainti", EmitDefaultValue = false)]
        public Sijainti Sijainti { get; set; }

        /// <summary>
        /// **fi:** Työpaikkailmoituksen tyyppi | **en:** Type of the job posting&lt;details&gt;&lt;summary&gt;Koodit | Codes&lt;/summary&gt;&lt;pre&gt;01 &#x3D; Yritys | Organization  02 &#x3D; Kotitalous | Household&lt;/pre&gt;&lt;/details&gt;
        /// </summary>
        /// <value>**fi:** Työpaikkailmoituksen tyyppi | **en:** Type of the job posting&lt;details&gt;&lt;summary&gt;Koodit | Codes&lt;/summary&gt;&lt;pre&gt;01 &#x3D; Yritys | Organization  02 &#x3D; Kotitalous | Household&lt;/pre&gt;&lt;/details&gt;</value>
        [DataMember(Name = "tyollistaja", EmitDefaultValue = false)]
        public string Tyollistaja { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class Tyopaikkailmoitus {\n");
            sb.Append("  Hakeminen: ").Append(Hakeminen).Append("\n");
            sb.Append("  IlmoittajanNimi: ").Append(IlmoittajanNimi).Append("\n");
            sb.Append("  IlmoittajanYTunnus: ").Append(IlmoittajanYTunnus).Append("\n");
            sb.Append("  IlmoituksenID: ").Append(IlmoituksenID).Append("\n");
            sb.Append("  IlmoituksenKielet: ").Append(IlmoituksenKielet).Append("\n");
            sb.Append("  IlmoituksenOhjaus: ").Append(IlmoituksenOhjaus).Append("\n");
            sb.Append("  Julkaisupvm: ").Append(Julkaisupvm).Append("\n");
            sb.Append("  Kotisivut: ").Append(Kotisivut).Append("\n");
            sb.Append("  KotitaloudenNimi: ").Append(KotitaloudenNimi).Append("\n");
            sb.Append("  Luontipvm: ").Append(Luontipvm).Append("\n");
            sb.Append("  Markkinointikuvaus: ").Append(Markkinointikuvaus).Append("\n");
            sb.Append("  Muokattupvm: ").Append(Muokattupvm).Append("\n");
            sb.Append("  Osaamisvaatimukset: ").Append(Osaamisvaatimukset).Append("\n");
            sb.Append("  Perustiedot: ").Append(Perustiedot).Append("\n");
            sb.Append("  Sijainti: ").Append(Sijainti).Append("\n");
            sb.Append("  Tyollistaja: ").Append(Tyollistaja).Append("\n");
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
            return this.Equals(input as Tyopaikkailmoitus);
        }

        /// <summary>
        /// Returns true if Tyopaikkailmoitus instances are equal
        /// </summary>
        /// <param name="input">Instance of Tyopaikkailmoitus to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Tyopaikkailmoitus input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.Hakeminen == input.Hakeminen ||
                    (this.Hakeminen != null &&
                    this.Hakeminen.Equals(input.Hakeminen))
                ) && 
                (
                    this.IlmoittajanNimi == input.IlmoittajanNimi ||
                    this.IlmoittajanNimi != null &&
                    input.IlmoittajanNimi != null &&
                    this.IlmoittajanNimi.SequenceEqual(input.IlmoittajanNimi)
                ) && 
                (
                    this.IlmoittajanYTunnus == input.IlmoittajanYTunnus ||
                    (this.IlmoittajanYTunnus != null &&
                    this.IlmoittajanYTunnus.Equals(input.IlmoittajanYTunnus))
                ) && 
                (
                    this.IlmoituksenID == input.IlmoituksenID ||
                    (this.IlmoituksenID != null &&
                    this.IlmoituksenID.Equals(input.IlmoituksenID))
                ) && 
                (
                    this.IlmoituksenKielet == input.IlmoituksenKielet ||
                    this.IlmoituksenKielet != null &&
                    input.IlmoituksenKielet != null &&
                    this.IlmoituksenKielet.SequenceEqual(input.IlmoituksenKielet)
                ) && 
                (
                    this.IlmoituksenOhjaus == input.IlmoituksenOhjaus ||
                    this.IlmoituksenOhjaus.Equals(input.IlmoituksenOhjaus)
                ) && 
                (
                    this.Julkaisupvm == input.Julkaisupvm ||
                    (this.Julkaisupvm != null &&
                    this.Julkaisupvm.Equals(input.Julkaisupvm))
                ) && 
                (
                    this.Kotisivut == input.Kotisivut ||
                    (this.Kotisivut != null &&
                    this.Kotisivut.Equals(input.Kotisivut))
                ) && 
                (
                    this.KotitaloudenNimi == input.KotitaloudenNimi ||
                    (this.KotitaloudenNimi != null &&
                    this.KotitaloudenNimi.Equals(input.KotitaloudenNimi))
                ) && 
                (
                    this.Luontipvm == input.Luontipvm ||
                    (this.Luontipvm != null &&
                    this.Luontipvm.Equals(input.Luontipvm))
                ) && 
                (
                    this.Markkinointikuvaus == input.Markkinointikuvaus ||
                    (this.Markkinointikuvaus != null &&
                    this.Markkinointikuvaus.Equals(input.Markkinointikuvaus))
                ) && 
                (
                    this.Muokattupvm == input.Muokattupvm ||
                    (this.Muokattupvm != null &&
                    this.Muokattupvm.Equals(input.Muokattupvm))
                ) && 
                (
                    this.Osaamisvaatimukset == input.Osaamisvaatimukset ||
                    (this.Osaamisvaatimukset != null &&
                    this.Osaamisvaatimukset.Equals(input.Osaamisvaatimukset))
                ) && 
                (
                    this.Perustiedot == input.Perustiedot ||
                    (this.Perustiedot != null &&
                    this.Perustiedot.Equals(input.Perustiedot))
                ) && 
                (
                    this.Sijainti == input.Sijainti ||
                    (this.Sijainti != null &&
                    this.Sijainti.Equals(input.Sijainti))
                ) && 
                (
                    this.Tyollistaja == input.Tyollistaja ||
                    (this.Tyollistaja != null &&
                    this.Tyollistaja.Equals(input.Tyollistaja))
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
                if (this.Hakeminen != null)
                {
                    hashCode = (hashCode * 59) + this.Hakeminen.GetHashCode();
                }
                if (this.IlmoittajanNimi != null)
                {
                    hashCode = (hashCode * 59) + this.IlmoittajanNimi.GetHashCode();
                }
                if (this.IlmoittajanYTunnus != null)
                {
                    hashCode = (hashCode * 59) + this.IlmoittajanYTunnus.GetHashCode();
                }
                if (this.IlmoituksenID != null)
                {
                    hashCode = (hashCode * 59) + this.IlmoituksenID.GetHashCode();
                }
                if (this.IlmoituksenKielet != null)
                {
                    hashCode = (hashCode * 59) + this.IlmoituksenKielet.GetHashCode();
                }
                hashCode = (hashCode * 59) + this.IlmoituksenOhjaus.GetHashCode();
                if (this.Julkaisupvm != null)
                {
                    hashCode = (hashCode * 59) + this.Julkaisupvm.GetHashCode();
                }
                if (this.Kotisivut != null)
                {
                    hashCode = (hashCode * 59) + this.Kotisivut.GetHashCode();
                }
                if (this.KotitaloudenNimi != null)
                {
                    hashCode = (hashCode * 59) + this.KotitaloudenNimi.GetHashCode();
                }
                if (this.Luontipvm != null)
                {
                    hashCode = (hashCode * 59) + this.Luontipvm.GetHashCode();
                }
                if (this.Markkinointikuvaus != null)
                {
                    hashCode = (hashCode * 59) + this.Markkinointikuvaus.GetHashCode();
                }
                if (this.Muokattupvm != null)
                {
                    hashCode = (hashCode * 59) + this.Muokattupvm.GetHashCode();
                }
                if (this.Osaamisvaatimukset != null)
                {
                    hashCode = (hashCode * 59) + this.Osaamisvaatimukset.GetHashCode();
                }
                if (this.Perustiedot != null)
                {
                    hashCode = (hashCode * 59) + this.Perustiedot.GetHashCode();
                }
                if (this.Sijainti != null)
                {
                    hashCode = (hashCode * 59) + this.Sijainti.GetHashCode();
                }
                if (this.Tyollistaja != null)
                {
                    hashCode = (hashCode * 59) + this.Tyollistaja.GetHashCode();
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
