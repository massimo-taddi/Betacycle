export class Product {
  ProductID: number = 0;
  Name: string = '';
  ProductNumber: string = '';
  Color: string = '';
  StandardCost: number = 0;
  ListPrice: number = 0;
  Size: string = '';
  Weight: number = 0;
  ProductCategoryid: number = 0;
  ProductModelid: number = 0;
  SellStartDate: Date | null = null;
  SellEndDate: Date | null = null;
  DiscontinuedDate: Date | null = null;
  ThumbNailPhoto: any[] = [];
  ThumbNailPhotoFileName: string = '';
  rowguid: string = '';
  ModifiedDate: Date | null = null;
}
