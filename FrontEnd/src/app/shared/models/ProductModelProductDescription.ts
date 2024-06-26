import { ProductDescription } from "./ProductDescription";

export class ProductModelProductDescription {
  productModelId: number = 0;
  productDescriptionid: number = 0;
  culture: string = '';
  rowguid: string = '';
  modifiedDate: Date | null = null;
  productDescription: ProductDescription | null = null;
}
