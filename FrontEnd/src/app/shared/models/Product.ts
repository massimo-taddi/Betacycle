import { ProductModel } from './ProductModel';

export class Product {
  productId: number = 0;
  name: string = '';
  productNumber: string = '';
  color: string = '';
  standardCost: number = 0;
  listPrice: number = 0;
  size: string = '';
  weight: number = 0;
  productCategoryid: number = 0;
  productModelid: number = 0;
  sellStartDate: Date | null = null;
  sellEndDate: Date | null = null;
  discontinuedDate: Date | null = null;
  thumbNailPhoto: string = '';
  thumbNailPhotoFileName: string = '';
  rowguid: string = '';
  modifiedDate: Date | null = null;
  productModel: ProductModel | null = null;
}
