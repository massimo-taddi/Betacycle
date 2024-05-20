import { ProductModelProductDescription } from "./ProductModelProductDescription";

export class ProductModel {
  productModelId: number = 0;
  name: string = '';
  modifiedDate: string = '';
  discontinued: boolean = false;
  productModelProductDescriptions: ProductModelProductDescription [] | null = null;
}
