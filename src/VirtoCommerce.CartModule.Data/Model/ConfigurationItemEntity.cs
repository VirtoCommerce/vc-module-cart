using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model;

public class ConfigurationItemEntity : AuditableEntity
{
    [StringLength(128)]
    public string LineItemId { get; set; }
    public LineItemEntity LineItem { get; set; }

    [StringLength(128)]
    public string ProductId { get; set; }

    [StringLength(128)]
    public string SectionId { get; set; }

    [StringLength(1024)]
    public string Name { get; set; }

    [StringLength(128)]
    public string Sku { get; set; }

    public int Quantity { get; set; }

    [StringLength(1028)]
    public string ImageUrl { get; set; }

    [StringLength(128)]
    public string CatalogId { get; set; }

    [StringLength(128)]
    public string CategoryId { get; set; }

    [Required]
    public byte SectionType { get; set; }

    [StringLength(255)]
    public string CustomText { get; set; }

    public virtual ConfigurationItem ToModel(ConfigurationItem configurationItem)
    {
        System.ArgumentNullException.ThrowIfNull(configurationItem);

        configurationItem.Id = Id;
        configurationItem.CreatedBy = CreatedBy;
        configurationItem.CreatedDate = CreatedDate;
        configurationItem.ModifiedBy = ModifiedBy;
        configurationItem.ModifiedDate = ModifiedDate;

        configurationItem.LineItemId = LineItemId;
        configurationItem.ProductId = ProductId;
        configurationItem.SectionId = SectionId;
        configurationItem.Name = Name;
        configurationItem.Sku = Sku;
        configurationItem.Quantity = Quantity;
        configurationItem.ImageUrl = ImageUrl;
        configurationItem.CatalogId = CatalogId;
        configurationItem.CategoryId = CategoryId;
        configurationItem.SectionType = EnumUtility.SafeParse(SectionType.ToString(), CartConfigurationSectionType.Product);
        configurationItem.CustomText = CustomText;

        return configurationItem;
    }

    public virtual ConfigurationItemEntity FromModel(ConfigurationItem configurationItem, PrimaryKeyResolvingMap pkMap)
    {
        System.ArgumentNullException.ThrowIfNull(configurationItem);

        pkMap.AddPair(configurationItem, this);

        Id = configurationItem.Id;
        CreatedBy = configurationItem.CreatedBy;
        CreatedDate = configurationItem.CreatedDate;
        ModifiedBy = configurationItem.ModifiedBy;
        ModifiedDate = configurationItem.ModifiedDate;

        LineItemId = configurationItem.LineItemId;
        ProductId = configurationItem.ProductId;
        SectionId = configurationItem.SectionId;
        Name = configurationItem.Name;
        Sku = configurationItem.Sku;
        Quantity = configurationItem.Quantity;
        ImageUrl = configurationItem.ImageUrl;
        CatalogId = configurationItem.CatalogId;
        CategoryId = configurationItem.CategoryId;
        SectionType = (byte)configurationItem.SectionType;
        CustomText = configurationItem.CustomText;

        return this;
    }

    public virtual void Patch(ConfigurationItemEntity target)
    {
        target.LineItemId = LineItemId;
        target.ProductId = ProductId;
        target.SectionId = SectionId;
        target.Name = Name;
        target.Sku = Sku;
        target.Quantity = Quantity;
        target.ImageUrl = ImageUrl;
        target.CatalogId = CatalogId;
        target.CategoryId = CategoryId;
        target.SectionType = SectionType;
        target.CustomText = CustomText;
    }
}
