import { Address } from "./Address";
import { SalesOrderDetail } from "./SalesOrderDetail";

export class SalesOrderHeader {
  salesOrderId: number = 0; //no
  revisionNumber: number = 0; 
  orderDate: Date | null = null;
  dueDate: Date | null = null; //no
  shipDate: Date | null = null; //no
  status: number = 0; //1 deafult
  onlineOrderFlag: boolean = true; //true
  salesOrderNumber: string | null = null; //no
  purchaseOrderNumber: string | null = null; //no
  shipToAddressId: number = 0;
  billToAddressId: number = 0;
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



