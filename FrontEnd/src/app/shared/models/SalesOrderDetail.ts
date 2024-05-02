export class SalesOrderDetail {
  salesOrderid: number = 0;
  salesOrderDetailid: number = 0;
  orderQty: number = 0;
  productid: number = 0;
  unitPrice: number = 0;
  unitPrioceDiscount: number = 0;
  lineTotal: number = 0;
  rowguid: string = '';
  modifiedDate: Date | null = null;
}
