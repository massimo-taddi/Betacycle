import { Address } from "./Address";
import { SalesOrderDetail } from "./SalesOrderDetail";

export class SalesOrderHeader {
  SalesOrderID: number = 0;
  RevisionNumber: number = 0;
  OrderDate: Date | null = null;
  DueDate: Date | null = null;
  ShipDate: Date | null = null;
  Status: number = 0;
  OnlineOrderFlag: boolean = false;
  SalesOrderNumber: string = '';
  PurchaseOrderNumber: string = '';
  ShipToAddressID: number = 0;
  ShipMethod: string = '';
  SubTotal: number = 0;
  TaxAmt: number = 0;
  Freight: number = 0;
  TotalDue: number = 0;
  Comment: string = '';
  SalesOrderDetails: SalesOrderDetail[] = [];
  ShipToAddress: Address | null = null;

  constructor() {}
}



