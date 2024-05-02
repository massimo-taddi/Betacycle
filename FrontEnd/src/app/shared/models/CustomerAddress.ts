export class CustomerAddress {
  customerID?: number;
  addressID?: number;
  addressType?: string;
  rowguid?: string;
  modifiedDate: Date | null = null;

  constructor(
    addressType: string,
  ) {
    this.addressType = addressType;
    this.modifiedDate = new Date(Date.now());
  }
}
