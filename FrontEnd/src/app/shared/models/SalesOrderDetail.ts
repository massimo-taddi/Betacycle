import { Product } from "./Product";

export class SalesOrderDetail {
  salesOrderid: number = 0;
  salesOrderDetailId: number = 0;
  orderQty: number = 0;
  productId: number = 0;
  unitPrice: number = 0;
  unitPriceDiscount: number = 0;
  lineTotal: number = 0;
  rowguid: string = '';
  modifiedDate: Date | null = null;
  product: Product | null = null;
}
