import { Address } from "./Address";
import { SalesOrderDetail } from "./SalesOrderDetail";

export class SalesOrderHeader {
  salesOrderID: number = 0; //no
  revisionNumber: number = 0; 
  orderDate: Date | null = null;
  dueDate: Date | null = null; //no
  shipDate: Date | null = null; //no
  status: number = 0; //1 deafult
  onlineOrderFlag: boolean = true; //true
  salesOrderNumber: string = ''; //no
  purchaseOrderNumber: string = ''; //no
  shipToAddressID: number = 0;
  billToAddressID: number = 0;
  shipMethod: string = ''; // UPS - FedEx - USPS - DHL
  
  subTotal: number = 0; //backend
  taxAmt: number = 0; // passato in percentuale
  freight: number = 0; //backend
  totalDue: number = 0; //backend
  comment: string | null = null; //
  salesOrderDetails: SalesOrderDetail[] = [];
  shipToAddress: Address | null = null;

  constructor() {}
}



