export class CustomerAddress {
  CustomerID?: number;
  AddressID?: number;
  AddressType?: string;
  Rowguid?: string;
  ModifiedDate: Date | null = null;

  constructor(
    AddressType: string,
  ) {
    this.AddressType = AddressType;
    this.ModifiedDate = new Date(Date.now());
  }
}
