
export class CustomerAddress {
  customerID: number = 0;
  addressID: number = 0;
  addressType: string ='';
  rowguid: string| null = null;
  modifiedDate: Date | null = null;

  constructor(
    addressType: string,
  ) {
    this.addressType = addressType;
    this.modifiedDate = new Date(Date.now());
  }
}

