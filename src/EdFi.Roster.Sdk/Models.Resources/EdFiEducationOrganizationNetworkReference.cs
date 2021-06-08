/*
 * Ed-Fi Operational Data Store API
 *
 * The Ed-Fi ODS / API enables applications to read and write education data stored in an Ed-Fi ODS through a secure REST interface.  ***  > *Note: Consumers of ODS / API information should sanitize all data for display and storage. The ODS / API provides reasonable safeguards against cross-site scripting attacks and other malicious content, but the platform does not and cannot guarantee that the data it contains is free of all potentially harmful content.*  *** 
 *
 * The version of the OpenAPI document: 3
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
using OpenAPIDateConverter = EdFi.Roster.Sdk.Client.OpenAPIDateConverter;

namespace EdFi.Roster.Sdk.Models.Resources
{
    /// <summary>
    /// EdFiEducationOrganizationNetworkReference
    /// </summary>
    [DataContract(Name = "edFi_educationOrganizationNetworkReference")]
    public partial class EdFiEducationOrganizationNetworkReference : IEquatable<EdFiEducationOrganizationNetworkReference>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiEducationOrganizationNetworkReference" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected EdFiEducationOrganizationNetworkReference() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiEducationOrganizationNetworkReference" /> class.
        /// </summary>
        /// <param name="educationOrganizationNetworkId">The identifier assigned to a network of education organizations. (required).</param>
        /// <param name="link">link.</param>
        public EdFiEducationOrganizationNetworkReference(int educationOrganizationNetworkId = default(int), Link link = default(Link))
        {
            this.EducationOrganizationNetworkId = educationOrganizationNetworkId;
            this.Link = link;
        }

        /// <summary>
        /// The identifier assigned to a network of education organizations.
        /// </summary>
        /// <value>The identifier assigned to a network of education organizations.</value>
        [DataMember(Name = "educationOrganizationNetworkId", IsRequired = true, EmitDefaultValue = false)]
        public int EducationOrganizationNetworkId { get; set; }

        /// <summary>
        /// Gets or Sets Link
        /// </summary>
        [DataMember(Name = "link", EmitDefaultValue = false)]
        public Link Link { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EdFiEducationOrganizationNetworkReference {\n");
            sb.Append("  EducationOrganizationNetworkId: ").Append(EducationOrganizationNetworkId).Append("\n");
            sb.Append("  Link: ").Append(Link).Append("\n");
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
            return this.Equals(input as EdFiEducationOrganizationNetworkReference);
        }

        /// <summary>
        /// Returns true if EdFiEducationOrganizationNetworkReference instances are equal
        /// </summary>
        /// <param name="input">Instance of EdFiEducationOrganizationNetworkReference to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(EdFiEducationOrganizationNetworkReference input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.EducationOrganizationNetworkId == input.EducationOrganizationNetworkId ||
                    this.EducationOrganizationNetworkId.Equals(input.EducationOrganizationNetworkId)
                ) && 
                (
                    this.Link == input.Link ||
                    (this.Link != null &&
                    this.Link.Equals(input.Link))
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
                hashCode = hashCode * 59 + this.EducationOrganizationNetworkId.GetHashCode();
                if (this.Link != null)
                    hashCode = hashCode * 59 + this.Link.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}