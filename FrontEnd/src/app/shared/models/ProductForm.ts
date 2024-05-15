export class ProductForm {
    name!: string;
    productNumber!: string;
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
    thumbNailPhoto: string | null = null;
    thumbnailPhotoFileName: string | null = null;
    modifiedDate: Date = new Date(Date.now());
    largePhoto: string | null = null;
    largePhotoFileName: string | null = null;
    onSale: boolean = false;
}