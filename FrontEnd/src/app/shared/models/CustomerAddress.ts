
export class CustomerAddress {
  customerID: number = 0;
  addressID: number = 0;
  addressType: string ='';
  modifiedDate: Date;

  constructor(
    addressType: string,
  ) {
    this.addressType = addressType;
    this.modifiedDate = new Date(Date.now());
  }
}

