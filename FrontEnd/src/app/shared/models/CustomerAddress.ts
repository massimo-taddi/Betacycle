import { Address } from "./Address";

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

export class AddressPost {
  myAddress: Address | null = null;
  myCustomerAddress: CustomerAddress | null= null;

  constructor(add: Address, custAdd: CustomerAddress){
    this.myAddress = add;
    this.myCustomerAddress = custAdd;
  }
}
