using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Entity.AzureSqlRepository.DO
{
    [Table("EntityType", Schema = "Meta")]
    public class EntityType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int EntityTypeId { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("Prefix", TypeName = "varchar")]
        [Index("UQ_EntityTypePrefix", IsClustered = false, IsUnique = true)]
        public virtual string Prefix { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Name", TypeName = "varchar")]
        [Index("UQ_EntityTypeName",IsClustered = false, IsUnique = true)]
        public virtual string Name { get; set; }

        [Required]
        [Column("IsAllowedAsAssetNode", TypeName = "bit")]
        public virtual bool IsAllowedAsAssetNode { get; set; }

        [Required]
        [Column("IsAllowedAsServiceNode", TypeName = "bit")]
        public virtual bool IsAllowedAsServiceNode { get; set; }

        [Required]
        [Column("IsAllowedSameDescendantNode", TypeName = "bit")]
        public virtual bool IsAllowedSameDescendantNode { get; set; }

    }
}
