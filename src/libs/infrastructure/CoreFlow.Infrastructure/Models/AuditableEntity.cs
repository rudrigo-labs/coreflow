using System.ComponentModel.DataAnnotations;

namespace CoreFlow.Infrastructure.Models
{
	public abstract class AuditableEntity
	{
		public bool IsActive { get; set; }

		/// <summary>
		/// A data em que a entidade foi criada
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// A data em que a entidade foi modificada pela última vez
		/// </summary>
		public DateTime? DateModified { get; set; }

        /// <summary>
        /// O usuário que criou a entidade
        /// </summary>
        [MaxLength(200)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// O usuário que modificou a entidade pela última vez
        /// </summary>
        [MaxLength(200)]
        public string? ModifiedBy { get; set; }
    }
}
