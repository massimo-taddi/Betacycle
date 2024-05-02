export class SalesOrderHeader {
  SalesOrderid: number = 0;
  RevisionNumber: number = 0;
  OrderDate: Date | null = null;
  DueDate: Date | null = null;
  ShipDate: Date | null = null;
  Status: number = 0;
  OnlineOrderFlag: boolean = false;

  constructor() {}
}
