export class ProductForm {
    name: string | null = null!;
    productNumber: string | null = null;
    color: string | null = null;
    standardCost!: number;
    listPrice!: number;
    size: string | null = null;
    weight: number | null = null;
    productCategoryId: number | null = null;
    productModelId: number | null = null;
    sellStartDate!: Date;
    sellEndDate: Date | null = null;
    discontinuedDate: Date | null = null;
    thumbNailPhoto: Blob | null = null;
    thumbnailPhotoFileName: string | null = null;
    modifiedDate: Date = new Date(Date.now());
}