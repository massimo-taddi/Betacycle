import { Address } from "./Address";
import { SalesOrderDetail } from "./SalesOrderDetail";

export class SalesOrderHeader {
  salesOrderID: number = 0;
  revisionNumber: number = 0;
  orderDate: Date | null = null;
  dueDate: Date | null = null;
  shipDate: Date | null = null;
  status: number = 0;
  onlineOrderFlag: boolean = false;
  salesOrderNumber: string = '';
  purchaseOrderNumber: string = '';
  shipToAddressID: number = 0;
  shipMethod: string = '';
  subTotal: number = 0;
  taxAmt: number = 0;
  freight: number = 0;
  totalDue: number = 0;
  comment: string = '';
  salesOrderDetails: SalesOrderDetail[] = [];
  shipToAddress: Address | null = null;

  constructor() {}
}



