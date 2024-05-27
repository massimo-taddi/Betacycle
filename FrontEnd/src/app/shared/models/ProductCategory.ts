export class ProductCategory {
  productCategoryId: number = 0;
  parentProductCategoryId?: number;
  name: string = '';
  rowguid: string = '';
  modifiedDate: Date | null = null;
}
