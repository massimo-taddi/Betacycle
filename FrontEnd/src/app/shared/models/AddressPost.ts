import { Address } from "./Address";
import { CustomerAddress } from "./CustomerAddress";

export class AddressPost {
    myAddress: Address | null = null;
    myCustomerAddress: CustomerAddress | null= null;
  
    constructor(){
      this.myAddress = null;
      this.myCustomerAddress = null;
    }
  }